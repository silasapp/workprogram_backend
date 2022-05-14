using System.Collections.Generic;
using System.Linq;
using Musewall.Models;

namespace Backend_UMR_Work_Program.Services
{
    public static class ExtensionMethods
    {
        public static IEnumerable<UserToken> WithoutPasswords(this IEnumerable<UserToken> users) {
            return users.Select(x => x.WithoutPassword());
        }

        public static UserToken WithoutPassword(this UserToken user) {
            // user.Password = null;
            return user;
        }
    }
}