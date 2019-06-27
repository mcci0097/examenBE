using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<HistoryUserRole> HistoryUserRole { get; set; }
    }
}
