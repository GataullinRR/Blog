using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog
{
    /// <summary>
    /// Some kind of DRY
    /// </summary>
    public class Partials
    {
        public static string LINK_TO_USER = "_LinkToUser";
        public static string USERS_TABLE = "_UsersTable";

        public static class AdminPanel
        {
            public static string USERS_TAB = "AdminPanel/_UsersTable";
        }
    }
}
