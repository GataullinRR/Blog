using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog
{
    public class Partials
    {
        public static string LINK_TO_USER = "_LinkToUser";
        public static string USERS_TABLE = "_UsersTable";
        public static string COMMENTARIES_SECTION = "_CommentarySection";
        public static string COMMENTARY = "_Commentary";
        public static string COMMENTARY_EDIT = "_CommentaryEdit";
        public static string PROFILE_IMAGE = "_ProfileImage";
        public static string INDEX_PAGE_BUTTON = "_IndexPageButton";

        public static class ModeratorPanel
        {
            public static string ASSIGN_ENTITIES = "ModeratorPanel/_AssignEntities";
            public static string MARK_AS_RESOLVED_BUTTON = "ModeratorPanel/_MarkAsResolvedButton";
        }

        public static class AdminPanel
        {
            public static string USERS_TAB = "AdminPanel/_UsersTab";
        }
    }
}
