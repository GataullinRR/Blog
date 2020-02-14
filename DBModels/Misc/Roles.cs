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
        [Description(Roles.USER)]
        USER = 0,
        [Description(Roles.MODERATOR)]
        MODERATOR = 1,
        [Description(Roles.ADMINISTRATOR)]
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

        public static string[] GetAllNotLess(this string role)
        {
            var i = AllRoles.Find(r => r == role).Index;

            return AllRoles.GetRangeTill(i, AllRoles.Count()).ToArray();
        }

        public static Role GetRole(this string role)
        {
            switch (role)
            {
                case USER:
                    return Role.USER;
                case MODERATOR:
                    return Role.USER;
                case ADMINISTRATOR:
                    return Role.USER;
             
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
