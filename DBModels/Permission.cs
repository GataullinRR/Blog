using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    /// <summary>
    /// The permissions are not "final". E.g. if user is not active actual permissions' value can be false
    /// </summary>
    public class Permission : IDbEntity
    {
        [Key] public int Id { get; set; }
        public bool CanEditPostWithoutModeration { get; set; }
        public bool CanCreatePostWithoutModeration { get; set; }
    }
}
