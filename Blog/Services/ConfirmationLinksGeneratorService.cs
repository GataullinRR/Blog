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
using Blog.Services.Models;
using Blog.Attributes;
using ASPCoreUtilities.Types;

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
    
    public class ConfirmationToken
    {
        public User Target { get; }
        public AccountOperation Operation { get; }
        public string Argument { get; }

        public ConfirmationToken(User target, AccountOperation operation, string argument)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Operation = operation;
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }
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

    [Service(ServiceType.SCOPED)]
    public class ConfirmationLinksGeneratorService : TokensBaseService
    {
        protected override TimeSpan _tokenLifespan { get; } = TimeSpan.FromMinutes(30);
        protected override ControllerAction _targetAction { get; } 
            = new ControllerAction(nameof(AccountController), nameof(AccountController.ConfirmAsync));

        public ConfirmationLinksGeneratorService(ServiceLocator services, IDataProtectionProvider protector) : base(
            services, 
            protector.CreateProtector("ConfirmationTokens"))
        {

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
        async Task<string> getConfirmationLinkAsync(User target, AccountOperation accountOperation, string arguments = "")
        {
            if (target == null)
            {
                throw new ArgumentNullException("user");
            }

            return await getTokenedLinkAsync(target, writer =>
            {
                writer.Write(accountOperation.To<byte>());
                writer.Write(arguments ?? "");

                return Task.CompletedTask;
            });
        }

        public async Task<(ConfirmationToken Data, TokenValidity Validity)> ParseAsync(string token)
        {
            return await parseAsync(token, async (reader, data) =>
            {
                var operation = EnumUtils.CastSafe<AccountOperation>(reader.ReadByte());
                var argument = reader.ReadString();

                return await Task.FromResult<ConfirmationToken>(new ConfirmationToken(data.ProvderOrTarget, operation, argument));
            });
        }
    }
}
