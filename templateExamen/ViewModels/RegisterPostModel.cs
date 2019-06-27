using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.ViewModels
{
    public class RegisterPostModel
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }
        public DateTime DateRegister { get; set; }
        //  public IEnumerable<HistoryUserRole> HistoryUserRoles { get; set; }
    }
}
