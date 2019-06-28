using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Constants;
using templateExamen.Models;

namespace templateExamen.Models
{
    public class UserRolesDbSeeder
    {

        public static void Initialize(UsersDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any tasksfdsfs.
            if (context.UserRoles.Any())
            {
                return;   // DB has been seeded
            }

            context.UserRoles.AddRange(
                new UserRole
                {
                    Name = RoleConstants.REGULAR
                },

                new UserRole
                {
                    Name = RoleConstants.MODERATOR
                },

                new UserRole
                {
                    Name = RoleConstants.ADMIN
                }

            );
            context.SaveChanges(); // commit transaction
        }
    }
}

