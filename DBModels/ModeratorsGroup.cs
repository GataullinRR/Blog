using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.Types;

namespace DBModels
{
    public class ModeratorsGroup : IDbEntity
    {
        [Key] public int Id { get; set; }
        [InverseProperty(nameof(User.ModeratorsGroup))]
        public virtual List<User> Moderators { get; set; } = new List<User>();
        [InverseProperty(nameof(User.ModeratorsInChargeGroup))]
        public virtual List<User> TargetUsers { get; set; } = new List<User>();
        [Required, InverseProperty(nameof(ModeratorsGroupStatistic.Owner))] 
        public virtual ModeratorsGroupStatistic Statistic { get; set; } = new ModeratorsGroupStatistic();

        public virtual List<EntityToCheck<Commentary>> CommentariesToCheck { get; set; } = new List<EntityToCheck<Commentary>>();
        public virtual List<EntityToCheck<PostEdit>> PostEditsToCheck { get; set; } = new List<EntityToCheck<PostEdit>>();
        public virtual List<EntityToCheck<Post>> PostsToCheck { get; set; } = new List<EntityToCheck<Post>>();
        public virtual List<EntityToCheck<Profile>> ProfilesToCheck { get; set; } = new List<EntityToCheck<Profile>>();
        public IEnumerable<IEntityToCheck> EntitiesToCheck => new Enumerable<IEntityToCheck>
        {
            CommentariesToCheck,
            PostsToCheck,
            ProfilesToCheck,
            PostEditsToCheck
        };

        public ModeratorsGroup() { }

        public void AddEntityToCheck(IAuthored reportObject, CheckReason reason)
        {
            add((dynamic)reportObject, reason);
        }
        void add(PostEdit postEdit, CheckReason reason) => PostEditsToCheck.Add(new EntityToCheck<PostEdit>(postEdit) { CheckReason = reason });
        void add(Commentary commentary, CheckReason reason) => CommentariesToCheck.Add(new EntityToCheck<Commentary>(commentary) { CheckReason = reason });
        void add(Post post, CheckReason reason) => PostsToCheck.Add(new EntityToCheck<Post>(post) { CheckReason = reason });
        void add(Profile profile, CheckReason reason) => ProfilesToCheck.Add(new EntityToCheck<Profile>(profile) { CheckReason = reason });
    }
}
