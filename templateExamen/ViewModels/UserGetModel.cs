using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;

namespace templateExamen.ViewModels
{
    public class UserGetModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserRole { get; set; }


        public static UserGetModel FromUser(User user)
        {
            return new UserGetModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                UserRole = user.HistoryUserRole.FirstOrDefault().UserRole.Name
                
            };
        }
    }
}

