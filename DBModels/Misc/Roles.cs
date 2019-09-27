using System;
using System.Collections.Generic;
using System.Text;

namespace DBModels
{
    public static class Roles
    {
        public const string RESTRICTED = "Unconfirmed";
        public const string USER = "User";
        public const string MODERATOR = "Moderator";
        public const string ADMIN = "Admin";

        public const string NOT_RESTRICTED = USER + "," + MODERATOR + "," + ADMIN;
    }
}
