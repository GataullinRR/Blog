using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Utilities.Extensions;

namespace DBModels
{
    public enum Role
    {
        [Description("User")]
        USER = 0,
        [Description("Moderator")]
        MODERATOR = 1,
        [Description("Administrator")]
        ADMINISTRATOR = 2
    }

    public static class Roles
    {
        public const string USER = "User";
        public const string MODERATOR = "Moderator";
        public const string ADMINISTRATOR = "Administrator";

        public const string NOT_RESTRICTED = USER + "," + MODERATOR + "," + ADMINISTRATOR;

        public static readonly IEnumerable<string> AllRoles = new string[]
        {
            USER,
            MODERATOR,
            ADMINISTRATOR
        };

        public static bool IsLess(this string role, string checkRole)
        {
            var r1 = AllRoles.Find(r => r == role).Index;
            var r2 = AllRoles.Find(checkRole).Index;

            return r1 < r2;
        }

        public static string[] GetAllNotLess(this string role)
        {
            var i = AllRoles.Find(r => r == role).Index;

            return AllRoles.GetRangeTill(i, AllRoles.Count()).ToArray();
        }
    }
}
