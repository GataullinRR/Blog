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

namespace Blog.Services
{
    public enum AccountOperation
    {
        EMAIL_CONFIRMATION = 100,
        PASSWORD_RESET = 10000,
    }

    public class ConfirmationTokenService
    {
        readonly IUrlHelper _urlHelper;

        public ConfirmationTokenService(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        public string GetToken(User user, AccountOperation accountOperation)
        {
            var randomSeed = user.UserName.Select(c => (int)c).Sum() + (int)accountOperation;
            var rnd = new Random(randomSeed);
            var shaked = user.Email.Shake(rnd).Aggregate();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(shaked)).ToBase64();
            }
        }

        public string GenerateLink(HttpContext context, string token, string controller, string controllerAction)
        {
            return _urlHelper.Action(controllerAction, controller.Replace("Controller", ""), new { confirmationToken = token }, context.Request.Scheme);
        }

        public string GenerateLink(HttpContext context, string token, string controller, string controllerAction, object values)
        {
            values = ObjectExtensions.AddProperty(values, "confirmationToken", token);
            return _urlHelper.Action(controllerAction, controller.Replace("Controller", ""), values, context.Request.Scheme);
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
