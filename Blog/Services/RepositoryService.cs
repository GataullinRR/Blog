using Blog.Misc;
using Blog.Models;
using Utilities.Extensions;
using DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Attributes;
using Utilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class RepositoryService : ServiceBase
    {
        static readonly Expression<Func<Commentary, CommentaryModel>> _commentaryModelExpression = c => new CommentaryModel()
        {
            Author = c.Author.UserName,
            AuthorId = c.Author.Id,
            AuthorProfileImage = new ProfileImageModel()
            {
                RelativeUri = c.Author.Profile.Image
            },
            Body = c.Body,
            CommentaryId = c.Id,
            CreationTime = c.CreationTime,
            Edits = c.Edits.Select(e => new CommentaryEditModel()
            {
                Author = e.Author.UserName,
                AuthorId = e.Author.Id,
                Reason = e.Reason,
                Time = e.EditTime
            }).ToArray(),
            IsDeleted = c.IsDeleted,
            IsHidden = c.IsHidden,
            ViewStatistic = c.ViewStatistic,
        };

        public RepositoryService(ServiceLocator services) : base(services)
        {

        }

        public async Task AddUserActionAsync(User performer, UserAction action)
        {
            action.Author = performer;
            S.Db.UsersActions.Add(action);
        }

        public async Task<IQueryable<CommentaryModel>> GetCommentaryModelsQueryAsync(IQueryable<Commentary> commentaries)
        {
            return commentaries.Select(_commentaryModelExpression);
        }

        public async Task<List<CommentaryModel>> GetCommentaryModelsAsync(IQueryable<Commentary> commentaries)
        {
            var models = await commentaries.Select(_commentaryModelExpression).ToListAsync();
            var permissions = await await S.Permissions
                .GetCommentaryPermissionsAsync(models.Select(c => c.CommentaryId).ToArray())
                .ThenDo(async r => await r.ToListAsync());
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Permissions = permissions[i];
            }

            return models;
        }
    }
}
