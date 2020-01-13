using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class ProfilePageTabsController : ControllerBase
    {
        public ProfilePageTabsController(ServicesLocator serviceProvider) : base(serviceProvider)
        {
        }

        //public async Task<IActionResult> ActionsTabAsync()
        //{
        //    return 
        //}
    }
}