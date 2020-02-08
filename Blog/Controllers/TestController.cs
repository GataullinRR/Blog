using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;

namespace Blog.Controllers
{
    public class TestController : Controller
    {
        public async Task<IActionResult> CachedTime()
        {
            await Task.Delay(3000);
            return Content(DateTime.Now.ToString() + 10000.Range().Select(i => i.ToString()).Aggregate(""));
        }
    }
}