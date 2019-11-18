using System;
using System.Collections.Generic;

namespace Blog.Models
{
    public enum CreationStage
    {
        JUST_CREATED = 0,
        GOING_TO_RENDER = 1,
        RENDERED = 2
    }

    [Serializable]
    public class JSHandledAction
    {
        public string Name { get; }
        public string FunctionName { get; }

        public JSHandledAction(string name, string functionName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            FunctionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
        }

        public static IEnumerable<JSHandledAction> Parse(string actions)
        {
            var separateActions = actions.Split(",");
            foreach (var action in separateActions)
            {
                var nameHandler = action.Split("=>");

                yield return new JSHandledAction(nameHandler[0], nameHandler[1]);
            }
        }
    }

    [Serializable]
    public class MessageModel
    {
        public MessageModel(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }
        public List<JSHandledAction> JSActions { get; } = new List<JSHandledAction>();
        internal CreationStage Stage { get; set; } = CreationStage.JUST_CREATED;
    }
}
