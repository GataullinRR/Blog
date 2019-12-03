﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBModels;

namespace Blog.Pages
{
    public class MarkAsNotPassedModerationModel : PageModelBase
    {
        [BindProperty, BindRequired, Required]
        public int PostId { get; set; }
        [BindProperty, BindRequired, Required, MinLength(8), MaxLength(500)]
        public string Reason { get; set; }
        [BindProperty, BindRequired, Required]
        public bool LastEdit { get; set; }

        public MarkAsNotPassedModerationModel(ServicesProvider services) : base(services)
        {

        }

        public async Task OnGet([Required]int id, [Required]bool lastEdit)
        {
            if (ModelState.IsValid)
            {
                PostId = id;
                LastEdit = lastEdit;

                var entity = await getEntityAsync();
                await Permissions.ValidateMarkAsNotPassedModerationAsync(entity);
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var entity = await getEntityAsync();
                await Permissions.ValidateMarkAsNotPassedModerationAsync(entity);
                await S.Moderation.MarkAsNotPassedModerationAsync(entity, Reason);

                LayoutModel.AddMessage("The post has been marked as not passed moderation");

                return RedirectToPage("/Post", new { id = PostId, lastEdit = LastEdit });
            }
            else
            {
                throw new Exception();
            }
        }

        async Task<IModeratable> getEntityAsync()
        {
            var post = await S.Db.Posts.FirstOrDefaultByIdAsync(PostId);
            return LastEdit
                 ? (IModeratable)post.LastEdit
                 : post;
        }
    }
}