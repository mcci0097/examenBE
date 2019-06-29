using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;

namespace templateExamen.ViewModels
{
    public class PacketPostModel
    {
        public String countryOrigin { get; set; }
        public String sender { get; set; }
        public String countryDestination { get; set; }
        public String addressDestination { get; set; }
        public double cost { get; set; }
        public String awb { get; set; }


        public static Packet ToPacket(PacketPostModel packetModel)
        {

            return new Packet
            {
                countryOrigin = packetModel.countryOrigin,
                sender = packetModel.sender,
                countryDestination = packetModel.countryDestination,
                addressDestination = packetModel.addressDestination,
                cost = packetModel.cost,
                awb = packetModel.awb,

            };
        }
    }
}
