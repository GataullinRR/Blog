using System;

namespace Blog.Models
{
    public enum CreationStage
    {
        JUST_CREATED = 0,
        GOING_TO_RENDER = 1,
        RENDERED = 2
    }

    [Serializable]
    public class MessageModel
    {
        public MessageModel(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }
        internal CreationStage Stage { get; set; } = CreationStage.JUST_CREATED;
    }
}
