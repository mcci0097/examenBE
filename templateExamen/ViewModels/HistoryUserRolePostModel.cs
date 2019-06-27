using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.ViewModels
{
    public class HistoryUserRolePostModel
    {
        public int UserId { get; set; }
        public string HistoryName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
