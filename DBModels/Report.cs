using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public abstract class Report
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual User Reporter { get; set; }

        public Report() { }

        public Report(User reporter)
        {
            Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
        }
    }

    public abstract class Report<T> : Report 
        where T : class
    {
        //todo make required
        public virtual T Object { get; set; }

        public Report() { }

        public Report(User reporter, T @object) : base(reporter)
        {
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
        }
    }

    public class CommentaryReport : Report<Commentary>
    {
        public CommentaryReport()
        {
        }

        public CommentaryReport(User reporter, Commentary @object) : base(reporter, @object)
        {

        }
    }

    public class ProfileReport : Report<ProfileInfo>
    {
        public ProfileReport()
        {
        }

        public ProfileReport(User reporter, ProfileInfo @object) : base(reporter, @object)
        {
        }
    }

    public class PostReport : Report<Post>
    {
        public PostReport()
        {
        }

        public PostReport(User reporter, Post @object) : base(reporter, @object)
        {
        }
    }
}
