using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Utilities;

namespace Blog.Controllers
{
    public class ActivationLinksController : ControllerBase
    {
        public ActivationLinksController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<string> GenerateLink([Required]int actionRaw)
        {
            if (ModelState.IsValid)
            {
                var action = EnumUtils.CastSafe<ActivationLinkAction>(actionRaw);
                await S.Permissions.ValidateGenerateActivationLinkAsync(action);

                var currenUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                return await S.ActivationLinks.GenerateLink(currenUser, action);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}