using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Models
{
    [Serializable()]
    public class LayoutModel
    {
        public List<MessageModel> Messages { get; } = new List<MessageModel>();

        public void AddMessage(string message)
        {
            Messages.Add(new MessageModel(message));
        }

        public void UpdateMessages()
        {
            Messages.RemoveAll(m => m.Stage >= CreationStage.RENDERED);
            foreach (var message in Messages)
            {
                message.Stage++;
            }
        }

        public static LayoutModel LoadOrNew(ISession session)
        {
            return session.Keys.Contains(nameof(LayoutModel))
                ? session.GetString(nameof(LayoutModel)).FromBase64().Deserialize<LayoutModel>()
                : new LayoutModel();
        }

        public void Save(ISession session)
        {
            session.SetString(nameof(LayoutModel), this.Serialize().ToBase64());
        }
    }
}
