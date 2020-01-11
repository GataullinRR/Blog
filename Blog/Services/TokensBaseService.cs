using Blog.Controllers;
using Blog.Services.Models;
using DBModels;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public enum TokenValidity
    {
        VALID,
        [Description("Token has already been used or expired")]
        EXPIRED,
        [Description("Bad token")]
        INVALID
    }

    public abstract class TokensBaseService : ServiceBase
    {
        protected class TokenHeader
        {
            public TokenHeader(User provderOrTarget)
            {
                ProvderOrTarget = provderOrTarget ?? throw new ArgumentNullException(nameof(provderOrTarget));
            }

            public User ProvderOrTarget { get; }
        }

        readonly IDataProtector _protector;
        protected abstract TimeSpan _tokenLifespan { get; }
        protected abstract ControllerAction _targetAction { get; }

        public TokensBaseService(ServicesLocator services, IDataProtector protector) : base(services)
        {
            _protector = protector;
        }

        protected async Task<string> getTokenedLinkAsync(User targetOrProvider, Func<BinaryWriter, Task> writer)
        {
            var token = await generateAsync();
            return S.LinkBuilder.GenerateLink(
                _targetAction.ControllerName,
                _targetAction.ActionName,
                new { token = token });
            
            async Task<string> generateAsync()
            {
                var ms = new MemoryStream();
                using (var writerStream = ms.CreateWriter())
                {
                    var tokenMetadata = new TokenMetadata();
                    S.Db.TokenMetadatas.Add(tokenMetadata);
                    await S.Db.SaveChangesAsync();
                    writerStream.Write(tokenMetadata.Id);
                    writerStream.Write(DateTimeOffset.UtcNow);
                    writerStream.Write(Convert.ToString(targetOrProvider.Id, CultureInfo.InvariantCulture));
                    string stamp = await S.UserManager.GetSecurityStampAsync(targetOrProvider);
                    writerStream.Write(stamp);
                    await writer(writerStream);
                }
                var protectedBytes = _protector.Protect(ms.ToArray());
                return Convert.ToBase64String(protectedBytes);
            }
        }

        protected async Task<(T DTO, TokenValidity State)> parseAsync<T>(string token, Func<BinaryReader, TokenHeader, Task<T>> reader)
            where T : class
        {
            try
            {
                var unprotectedData = _protector.Unprotect(Convert.FromBase64String(token));
                var ms = new MemoryStream(unprotectedData);
                using (var readerStream = ms.CreateReader())
                {
                    var metadataId = readerStream.ReadInt32();
                    var metadata = await S.Db.TokenMetadatas.FirstOrDefaultByIdAsync(metadataId);
                    if (metadata == null)
                    {
                        return (null, TokenValidity.INVALID);
                    }
                    else if (metadata.IsUsedOrExpired)
                    {
                        return (null, TokenValidity.EXPIRED);
                    }
                    var creationTime = readerStream.ReadDateTimeOffset();
                    var expirationTime = creationTime + _tokenLifespan;
                    if (expirationTime < DateTimeOffset.UtcNow)
                    {
                        return (null, TokenValidity.EXPIRED);
                    }
                    var userId = readerStream.ReadString();
                    var user = await S.UserManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return (null, TokenValidity.INVALID);
                    }
                    var stamp = readerStream.ReadString();
                    var expectedStamp = await S.UserManager.GetSecurityStampAsync(user);
                    if (stamp != expectedStamp)
                    {
                        return (null, TokenValidity.INVALID);
                    }
                    var dto = await reader(readerStream, new TokenHeader(user));
                    return (dto, TokenValidity.VALID);
                }
            }
            catch
            {
                return (null, TokenValidity.INVALID);
            }
        }

        public async Task MarkAsUsedOrExpiredAsync(string token)
        {
            var unprotectedData = _protector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);
            using (var readerStream = ms.CreateReader())
            {
                var metadataId = readerStream.ReadInt32();
                var metadata = await S.Db.TokenMetadatas.FirstOrDefaultByIdAsync(metadataId);
                metadata.IsUsedOrExpired = true;
                await S.Db.SaveChangesAsync();
            }
        }
    }
}
