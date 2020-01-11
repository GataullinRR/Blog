using Blog.Controllers;
using Blog.Services.Models;
using DBModels;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Blog.Services
{
    public enum ActivationLinkAction : byte
    {
        REGISTER_AS_MODERATOR = 0,
        REGISTER_AS_OWNER = 50,
    }

    public class ActivationLinkArguments
    {
        public User Provider { get; }
        public ActivationLinkAction Action { get; }

        public ActivationLinkArguments(User provider, ActivationLinkAction action)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Action = action;
        }
    }

    public class ActivationLinkGeneratorService : TokensBaseService
    {
        protected override TimeSpan _tokenLifespan { get; } = TimeSpan.FromMinutes(30);
        protected override ControllerAction _targetAction { get; } 
            = new ControllerAction(nameof(AccountController), nameof(AccountController.ApplyActivationLinkAsync));

        public ActivationLinkGeneratorService(ServicesLocator services) 
            : base(services, 
                   services.ProtectionProvider.CreateProtector("ActivationLinks"))
        {

        }

        public async Task<string> GenerateLink(User provider, ActivationLinkAction activationLinkAction)
        {
            return await getTokenedLinkAsync(provider, writer =>
            {
                writer.Write((byte)activationLinkAction);

                return Task.CompletedTask;
            });
        }

        public async Task<(ActivationLinkArguments Data, TokenValidity Validity)> ParseAsync(string token)
        {
            return await parseAsync(token, (reader, header) =>
            {
                var action = EnumUtils.CastSafe<ActivationLinkAction>(reader.ReadByte());

                return Task.FromResult(new ActivationLinkArguments(header.ProvderOrTarget, action));
            });
        }
    }
}
