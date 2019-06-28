using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.Models
{
    public class HistoryUserRolesDbSeeder
    {
        public static void Initialize(UsersDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any tasks.
            if (context.HistoryUserRoles.Any())
            {
                return;   // DB has been seeded
            }

            context.HistoryUserRoles.AddRange(
                new HistoryUserRole
                {
                    UserId = 1,
                    UserRoleId = 2,
                    StartTime = DateTime.Now,
                    EndTime = null
                   
                },

                new HistoryUserRole
                {
                    UserId = 2,
                    UserRoleId = 3,
                    StartTime = DateTime.Now,
                    EndTime = null

                }
            );
            context.SaveChanges(); // commit transaction
        }
    }
}
