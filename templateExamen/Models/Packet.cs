using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace templateExamen.Models
{
    public class Packet
    {
        public int id { get; set; }
        public String countryOrigin { get; set; }
        public String sender { get; set; }
        public String countryDestination { get; set; }
        public String addressDestination { get; set; }
        public double cost { get; set; }
        public String awb { get; set; }
    }
}
