using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class PostingController : ControllerBase
    {
        public PostingController(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        [HttpPost]
        public async Task<string> GetPostBodyPreviewAsync([Required]string rawBody)
        {
            if (ModelState.IsValid)
            {
                return S.Sanitizer.AllowAllButNotExecutable.Sanitize(rawBody);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}