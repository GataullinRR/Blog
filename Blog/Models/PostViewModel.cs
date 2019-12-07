using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostViewModel
    {
        public Post Post { get; }
        public bool PreviewVersion { get; }

        public PostViewModel(Post post, bool previewVersion)
        {
            Post = post ?? throw new ArgumentNullException(nameof(post));
            PreviewVersion = previewVersion;
        }
    }
}
