using Blog.Services;
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
        public bool CanAccessAdminsPanel { get; private set; }
        public bool CanAccessModsPanel { get; private set; }
        public bool CanCreatePost { get; private set; }
        public string UserName { get; private set; }

        public List<MessageServerModel> Messages { get; } = new List<MessageServerModel>();

        public void AddMessage(string message)
        {
            Messages.Add(new MessageServerModel(message));
        }

        public void UpdateMessages()
        {
            Messages.RemoveAll(m => m.Stage >= CreationStage.RENDERED);
            foreach (var message in Messages)
            {
                message.Stage++;
            }
        }

        public static async Task<LayoutModel> LoadOrNewAsync(ServiceLocator services)
        {
            var model = services.HttpContext.Session.Keys.Contains(nameof(LayoutModel))
                ? services.HttpContext.Session.GetString(nameof(LayoutModel)).FromBase64().Deserialize<LayoutModel>()
                : new LayoutModel();
            var currentUser = await services.Utilities.GetCurrentUserModelAsync();
            model.CanAccessAdminsPanel = await services.Permissions.CanAccessAdminPanelAsync();
            model.CanAccessModsPanel = await services.Permissions.CanAccessModeratorsPanelAsync(currentUser);
            model.CanCreatePost = await services.Permissions.CanCreatePostAsync();
            model.UserName = currentUser?.UserName;

            return model;
        }

        public void Save(ISession session)
        {
            session.SetString(nameof(Models.LayoutModel), (this).Serialize().ToBase64());
        }
    }
}
