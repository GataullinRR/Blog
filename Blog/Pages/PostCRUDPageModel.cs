﻿using System;
using System.ComponentModel.DataAnnotations;
using Blog.Models;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Pages
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public abstract class PostCRUDPageModel : ExtendedPageModel
    {
        [BindProperty(), Required(), MinLength(8), MaxLength(100)]
        public string Title { get; set; }
        [BindProperty(), Required(), MinLength(500), MaxLength(100000)]
        public string Body { get; set; }
        [BindProperty]
        public int PostId { get; set; }

        public PostCRUDPageModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }
}