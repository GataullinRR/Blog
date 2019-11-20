﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DBModels
{
    public class Post : IDbEntity, IModeratableObject
    {
        [Key] public int Id { get; set; }
        [Required] public DateTime CreationTime { get; set; }
        [Required] public virtual User Author { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Body { get; set; }
        [Required] public virtual List<PostEdit> Edits { get; set; } = new List<PostEdit>();
        [Required] public virtual ViewStatistic ViewStatistic { get; set; }
        [InverseProperty(nameof(Report.PostObject))]
        [Required] public virtual List<Report> Reports { get; set; } = new List<Report>();
        [InverseProperty(nameof(Violation.PostObject))]
        [Required] public virtual List<Violation> Violations { get; set; } = new List<Violation>();

        public Post() 
        { 

        }

        public Post(DateTime creationTime, User author, string title, string body)
        {
            CreationTime = creationTime;
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            ViewStatistic = new ViewStatistic();
        }
    }
}
