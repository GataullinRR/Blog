using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Extensions;

namespace DBModels
{
    public static class Roles
    {
        public const string USER = "User";
        public const string MODERATOR = "Moderator";
        public const string ADMIN = "Admin";
        public const string OWNER = "Owner";

        public const string NOT_RESTRICTED = USER + "," + MODERATOR + "," + ADMIN + "," + OWNER;

        public static readonly IEnumerable<string> AllRoles = new string[]
        {
            USER,
            MODERATOR,
            ADMIN,
            OWNER
        };

        public static bool IsLess(this string role, string checkRole)
        {
            var r1 = AllRoles.Find(r => r == role).Index;
            var r2 = AllRoles.Find(checkRole).Index;

            return r1 < r2;
        }
    }
}
