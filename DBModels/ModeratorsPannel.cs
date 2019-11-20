﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.Types;

namespace DBModels
{
    public class ModeratorPannel
    {
        public ModeratorPannel() { }

        [Key] public int Id { get; set; }
        /// <summary>
        /// Moderators who own this pannel
        /// </summary>
        public virtual List<User> Owners { get; set; } = new List<User>();
        public virtual List<EntityToCheck<Commentary>> CommentariesToCheck { get; set; } = new List<EntityToCheck<Commentary>>();
        public virtual List<EntityToCheck<Post>> PostsToCheck { get; set; } = new List<EntityToCheck<Post>>();
        public virtual List<EntityToCheck<Profile>> ProfilesToCheck { get; set; } = new List<EntityToCheck<Profile>>();
        public IEnumerable<IEntityToCheck> EntitiesToCheck => new Enumerable<IEntityToCheck>
        {
            CommentariesToCheck,
            PostsToCheck,
            ProfilesToCheck
        };

        public void AddEntityToCheck(IModeratableObject reportObject)
        {
            add((dynamic)reportObject);
        }
        void add(Commentary commentary) => CommentariesToCheck.Add(new EntityToCheck<Commentary>(commentary));
        void add(Post post) => PostsToCheck.Add(new EntityToCheck<Post>(post));
        void add(Profile profile) => ProfilesToCheck.Add(new EntityToCheck<Profile>(profile));
    }
}
