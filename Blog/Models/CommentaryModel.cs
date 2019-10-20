using Blog.Services;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class CommentaryModel
    {
        public CommentaryModel(User currentUser, Commentary commentary, PermissionsService permissions)
        {
            CurrentUser = currentUser;
            Commentary = commentary ?? throw new ArgumentNullException(nameof(commentary));
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        public User CurrentUser { get; }
        public Commentary Commentary { get; }
        public PermissionsService Permissions { get; }
    }
}
