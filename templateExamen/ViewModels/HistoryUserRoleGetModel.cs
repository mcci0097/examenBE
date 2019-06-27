using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;

namespace templateExamen.ViewModels
{
    public class HistoryUserRoleGetModel
    {

        public string Username { get; set; }
        public string UserRoleName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }


        public static HistoryUserRoleGetModel FromHistoryUserRole(HistoryUserRole historyUserRole)
        {

            return new HistoryUserRoleGetModel
            {

                Username = historyUserRole.User.Username,
                UserRoleName = historyUserRole.UserRole.Name,
                StartTime = historyUserRole.StartTime,
                EndTime = historyUserRole.EndTime


            };
        }
    }
}

