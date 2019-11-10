using System;

namespace Blog.Models
{
    [Serializable]
    public class MessageModel
    {
        public MessageModel(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }
        public bool JustCreated { get; set; } = true;
    }
}
