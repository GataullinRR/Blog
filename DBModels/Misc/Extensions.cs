using System;
using System.Collections.Generic;
using System.Text;

namespace DBModels
{
    public static class Extensions
    {
        public static string GetRoleName(this Role role)
        {
            switch (role)
            {
                case Role.USER:
                    return Roles.USER;
                case Role.ADMIN:
                    return Roles.ADMIN;
                case Role.MODERATOR:
                    return Roles.MODERATOR;
                case Role.UNCONFIRMED:
                    return Roles.RESTRICTED;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
