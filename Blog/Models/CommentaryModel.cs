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
        public CommentaryModel(Commentary commentary, PermissionsService permissions)
        {
            Commentary = commentary ?? throw new ArgumentNullException(nameof(commentary));
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        public Commentary Commentary { get; }
        public PermissionsService Permissions { get; }
    }
}
