using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Customer = "Customer";
        public static readonly string[] RoleListe = { "Admin", "User", "Customer" };
    }
}
