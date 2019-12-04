using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.TokenManagement
{
    public class TokenConstants
    {
        public static class Roles
        {
            public const string SuperAdmin = "superadmin";
            public const string Admin = "admin";
            public const string Shelf = "shelf";
            public const string Clerk = "clerk";
            public static Dictionary<string, int> ExpirationTime = new Dictionary<string, int>()
            {
                { Roles.SuperAdmin,30 },
                { Roles.Admin,30 },
                { Roles.Clerk,60 },
                {Roles.Shelf,600 }
            };
        }
    }
}
