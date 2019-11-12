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
using Blog.Misc;

namespace Blog.Services
{
    public enum AccountOperation
    {
        [Description("EMailConfirmation")]
        EMAIL_CONFIRMATION = 100,
        [Description("PasswordReset")]
        PASSWORD_RESET = 200,
        [Description("EMailChange")]
        EMAIL_CHANGE = 300,
    }

    public class ConfirmationLinksGeneratorService : ServiceBase
    {
        public ConfirmationLinksGeneratorService(ServicesProvider services) : base(services)
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
        async Task<string> getConfirmationLinkAsync(User user, AccountOperation accountOperation, string arguments = "")
        {
            var purpose = accountOperation.GetEnumValueDescription();
            var token = await Services.UserManager.GenerateUserTokenAsync(user, nameof(BasicTokenProvider), purpose);
            return Services.LinkBuilder.GenerateLink(
                nameof(AccountController),
                nameof(AccountController.ConfirmAsync),
                new { token = token, userId = user.Id, operation = (int)accountOperation, arguments = arguments });
        }

        public async Task<bool> VerifyTokenAsync(User user, AccountOperation accountOperation, string token)
        {
            var purpose = accountOperation.GetEnumValueDescription();

            return await Services.UserManager.VerifyUserTokenAsync(user, nameof(BasicTokenProvider), purpose, token);
        }

        static class ObjectExtensions
        {
            public static IDictionary<string, object> AddProperty(object obj, string name, object value)
            {
                var dictionary = ToDictionary(obj);
                dictionary.Add(name, value);
                return dictionary;
            }

            // helper
            public static IDictionary<string, object> ToDictionary(object obj)
            {
                IDictionary<string, object> result = new Dictionary<string, object>();
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
                foreach (PropertyDescriptor property in properties)
                {
                    result.Add(property.Name, property.GetValue(obj));
                }
                return result;
            }
        }
    }
}
