using System;

namespace Blog.Models
{
    [Serializable]
    public class LayoutModel
    {
        /// <summary>
        /// Null if not authentificated
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// Null if not authentificated
        /// </summary>
        public string userId { get; set; }
        public bool canCreatePost { get; set; }
        public bool canAccessModsPanel { get; set; }
        public bool canAccessAdminsPanel { get; set; }
        public MessageModel[] messages { get; set; }
    }

    [Serializable]
    public class MessageModel
    {
        public string text { get; set; }
        /// <summary>
        /// name=>js_function();,name2=>f2();, ...
        /// </summary>
        public string jsActions { get; set; }
    }
}
