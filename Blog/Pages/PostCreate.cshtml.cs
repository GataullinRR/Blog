﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class PostCreateModel : PostCRUDPageModel
    {
        public PostCreateModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task OnGetAsync()
        {
            await Services.Permissions.ValidateCreatePostAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await Services.Permissions.ValidateCreatePostAsync();

            if (ModelState.IsValid)
            {
                if (await Services.Db.Posts.FirstOrDefaultAsync(p => p.Title == Title) != null)
                {
                    ModelState.AddModelError("", "Post with this name already exists");

                    return Page();
                }
                else
                {
                    var author = await GetCurrentUserModelOrThrowAsync();
                    var post = new Post(DateTime.UtcNow, author, Title, Body);
                    Services.Db.Posts.Add(post);
                    await Services.Db.SaveChangesAsync();
                    author.Actions.Add(new UserAction(ActionType.POST_CREATE, post));
                    await Services.Db.SaveChangesAsync();

                    return RedirectToPage("/Post", new { id = post.Id });
                }
            }
            else
            {
                return Page();
            }
        }
    }
}