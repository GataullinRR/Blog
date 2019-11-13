using DBModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;
using Blog.Controllers;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.DataProtection;
using System.IO.Compression;

namespace Blog.Services
{
    public enum AccountOperation : byte
    {
        [Description("EMailConfirmation")]
        EMAIL_CONFIRMATION = 0,
        [Description("PasswordReset")]
        PASSWORD_RESET = 100,
        [Description("EMailChange")]
        EMAIL_CHANGE = 200,
    }
    
    public class TokenData
    {
        public TokenData(string userId, AccountOperation operation, DateTime creationTime, string argument, bool isValid)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Operation = operation;
            CreationTime = creationTime;
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
            IsValid = isValid;
        }

        public string UserId { get; }
        public AccountOperation Operation { get; }
        public DateTime CreationTime { get; }
        public string Argument { get; }
        public bool IsValid { get; }
    }

    // Based on Levi's authentication sample
    internal static class StreamExtensions
    {
        internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        public static BinaryReader CreateReader(this Stream stream)
        {
            return new BinaryReader(stream, DefaultEncoding, true);
        }

        public static BinaryWriter CreateWriter(this Stream stream)
        {
            return new BinaryWriter(stream, DefaultEncoding, true);
        }

        public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
        {
            return new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
        }

        public static void Write(this BinaryWriter writer, DateTimeOffset value)
        {
            writer.Write(value.UtcTicks);
        }
    }

    public class ConfirmationLinksGeneratorService : ServiceBase
    {
        readonly static TimeSpan TokenLifespan = TimeSpan.FromMinutes(30);

        readonly IDataProtector _protector;

        public ConfirmationLinksGeneratorService(ServicesProvider services, IDataProtectionProvider protector) : base(services)
        {
            _protector = protector.CreateProtector("ConfirmationTokens");
        }

        public async Task<string> GetEMailChangeConfirmationLinkAsync(User user, string newEMail)
        {
            return await getConfirmationLinkAsync(user, AccountOperation.EMAIL_CHANGE, newEMail);
        }
        public async Task<string> GetEMailConfirmationLinkAsync(User user)
        {
            return await getConfirmationLinkAsync(user, AccountOperation.EMAIL_CONFIRMATION);
        }
        public async Task<string> GetPasswordResetConfirmationLinkAsync(User user)
        {
            return await getConfirmationLinkAsync(user, AccountOperation.PASSWORD_RESET);
        }
        async Task<string> getConfirmationLinkAsync(User user, AccountOperation accountOperation, string arguments = "")
        {
            var token = await generateAsync();
            return Services.LinkBuilder.GenerateLink(
                nameof(AccountController),
                nameof(AccountController.ConfirmAsync),
                new { token = token });

            async Task<string> generateAsync()
            {
                if (user == null)
                {
                    throw new ArgumentNullException("user");
                }
                var ms = new MemoryStream();
                using (var writer = ms.CreateWriter())
                {
                    writer.Write(DateTimeOffset.UtcNow);
                    writer.Write((byte)accountOperation);
                    writer.Write(Convert.ToString(user.Id, CultureInfo.InvariantCulture));
                    writer.Write(arguments ?? "");
                    string stamp = await Services.UserManager.GetSecurityStampAsync(user);
                    writer.Write(stamp);
                }
                var protectedBytes = _protector.Protect(ms.ToArray());
                return Convert.ToBase64String(protectedBytes);
            }
        }

        public async Task<TokenData> ParseAsync(string token)
        {
            var isValid = true;
            var unprotectedData = _protector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);
            using (var reader = ms.CreateReader())
            {
                var creationTime = reader.ReadDateTimeOffset();
                var operation = (AccountOperation)reader.ReadByte();
                var expirationTime = creationTime + TokenLifespan;
                if (expirationTime < DateTimeOffset.UtcNow)
                {
                    isValid &= false;
                }

                var userId = reader.ReadString();
                var argument = reader.ReadString();
                var stamp = reader.ReadString();
                if (reader.PeekChar() != -1)
                {
                    isValid &= false;
                }

                var user = await Services.UserManager.FindByIdAsync(userId);
                var expectedStamp = await Services.UserManager.GetSecurityStampAsync(user);
                isValid &= stamp == expectedStamp;

                return new TokenData(userId, operation, creationTime.UtcDateTime, argument, isValid);
            }
        }
    }
}
