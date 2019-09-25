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
                    return "User";
                case Role.ADMIN:
                    return "Admin";

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
