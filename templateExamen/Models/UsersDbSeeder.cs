using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.Models
{
    public class UsersDbSeeder
    {
        public static void Initialize(UsersDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any tasks.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            context.Users.AddRange(
                new User
                {
                  FirstName = "Pop",
                  LastName = "Alexandru",
                  Username ="alex69",
                  Email = "alexandru@yahoo.com",
                  Password = "alex123456",
                  DateRegister = DateTime.Now,
                  HistoryUserRole = null
                },

                new User
                {
                    FirstName = "Moldovan",
                    LastName = "Radu",
                    Username = "radu96",
                    Email = "radu@yahoo.com",
                    Password = "radu123456",
                    DateRegister = DateTime.Now,
                    HistoryUserRole = null
                }
            );
            context.SaveChanges(); // commit transaction
        }
    }
}