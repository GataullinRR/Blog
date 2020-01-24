﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Blog.Controllers
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class CommentaryController : ControllerBase
    {
        public CommentaryController(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        [HttpGet()]
        public async Task<IActionResult> GetCommentaryEditAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var commentary = await S.Db.Commentaries.FirstAsync(c => c.Id == id);
                await S.Permissions.ValidateEditCommentaryAsync(commentary);

                return PartialView("_CommentaryEdit", commentary);
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet(), AllowAnonymous()]
        public async Task<IActionResult> GetCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var commentary = await S.Db.Commentaries.FirstAsync(c => c.Id == id);
                var user = await S.UserManager.GetUserAsync(User);

                return PartialView("_Commentary", new CommentaryModel(user, commentary, S.Permissions));
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpPost()]
        public async Task<IActionResult> UpdateCommentaryAsync([Required]int id, [Required]string body, [Required]string editReason)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.UserManager.GetUserAsync(User);
                var commentary = await S.Db.Commentaries.FirstAsync(c => c.Id == id);
                await S.Permissions.ValidateEditCommentaryAsync(commentary);

                var user = await S.UserManager.GetUserAsync(User);
                commentary.Body = body;
                commentary.Edits.Add(new CommentaryEdit(user, editReason, DateTime.UtcNow));
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.COMMENTARY_EDIT, commentary));
                await S.Db.SaveChangesAsync();
            }
            else
            {
                if (editReason == null)
                {
                    LayoutModel.AddMessage("Edit reason is required");
                }
                else if (body == null)
                {
                    LayoutModel.AddMessage("Commentary body is required");
                }
                else
                {
                    throw new Exception();
                }
            }

            return Redirect(S.History.GetLastURL());
        }

        [HttpGet()]
        public async Task<IActionResult> DeleteCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var commentary = await S.Db.Commentaries.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateDeleteCommentaryAsync(commentary);
               
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.COMMENTARY_DELETE, commentary));
                commentary.IsDeleted = true;
                await S.Db.SaveChangesAsync();

                return Redirect(S.History.GetLastURL());
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet()]
        public async Task<IActionResult> UndeleteCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var commentary = await S.Db.Commentaries.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateUndeleteCommentaryAsync(commentary);
               
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.COMMENTARY_UNDELETE, commentary));
                commentary.IsDeleted = false;
                await S.Db.SaveChangesAsync();

                return Redirect(S.History.GetLastURL());
            }
            else
            {
                throw new Exception();
            }
        }
    }
}