using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Blog
{
    public class PasswordResetErrorModel : PageModelBase
    {
        public string ContactEmail { get; set; }

        public PasswordResetErrorModel(ServicesLocator services) : base(services)
        {

        }

        public async Task OnGetAsync()
        {
            var moderators = await S.Db.ModeratorsGroups.Select(mg => mg.Moderators).Flatten().ToArrayAsync();
            ContactEmail = new Random().NextObjFrom(moderators).Email;
        }
    }
}