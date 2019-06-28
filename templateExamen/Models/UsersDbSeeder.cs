using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                  Password = ComputeSha256Hash("alex123456"),
                  DateRegister = DateTime.Now,
                  HistoryUserRole = null
                },

                new User
                {
                    FirstName = "Moldovan",
                    LastName = "Radu",
                    Username = "radu96",
                    Email = "radu@yahoo.com",
                    Password = ComputeSha256Hash("radu123456"),
                    DateRegister = DateTime.Now,
                    HistoryUserRole = null
                   
                }

            );
            context.SaveChanges(); // commit transaction
        }

        private static string ComputeSha256Hash(String rawData)
        {
            // Create a SHA256   
            // TODO: also use salt
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}