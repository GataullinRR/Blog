using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;

namespace Blog.Pages
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public abstract class PostCRUDPageModel : PageModelBase
    {
        [BindProperty(), Required(), MinLength(8), MaxLength(100)]
        public string Title { get; set; }
        [BindProperty(), Required(), MinLength(500), MaxLength(10000000)]
        public string Body { get; set; }
        [BindProperty]
        public int PostId { get; set; }

        public PostCRUDPageModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected async Task<string> getEscapedPostBodyAsync()
        {
            return await S.Sanitizer.SanitizePostBodyAsync(Body);
        }
        protected string getPostBodyPreview(string escapedBody)
        {
            return S.Sanitizer.IgnoreNonTextNodes(escapedBody.Take(500).Aggregate());
        }
    }
}