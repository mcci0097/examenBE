using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<HistoryUserRole> HistoryUserRole { get; set; }

        public DateTime DateRegister { get; set; }
    }
}
