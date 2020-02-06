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
    public class ServerLayoutModel
    {
        public List<MessageServerModel> Messages { get; } = new List<MessageServerModel>();

        public LayoutModel LayoutModel { get; } = new LayoutModel();

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

        public static async Task<ServerLayoutModel> LoadOrNewAsync(ServicesLocator services)
        {
            var model = services.HttpContext.Session.Keys.Contains(nameof(ServerLayoutModel))
                ? services.HttpContext.Session.GetString(nameof(ServerLayoutModel)).FromBase64().Deserialize<ServerLayoutModel>()
                : new ServerLayoutModel();
            var currentUser = await services.Utilities.GetCurrentUserModelAsync();
            model.LayoutModel.canAccessAdminsPanel = await services.Permissions.CanAccessAdminPanelAsync();
            model.LayoutModel.canAccessModsPanel = await services.Permissions.CanAccessModeratorsPanelAsync(currentUser);
            model.LayoutModel.canCreatePost = await services.Permissions.CanCreatePostAsync();
            model.LayoutModel.messages = model.Messages.Select(m => new MessageModel()
            {
                text = m.Text,
                jsActions = m.JSActions.Select(a => $"{a.Name}=>{a.FunctionName}").Aggregate(",")
            }).ToArray();
            model.LayoutModel.userName = currentUser?.UserName;
            model.LayoutModel.userId = currentUser?.Id;

            return model;
        }

        public void Save(ISession session)
        {
            session.SetString(nameof(ServerLayoutModel), this.Serialize().ToBase64());
        }
    }
}
