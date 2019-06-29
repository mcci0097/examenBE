using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;

namespace templateExamen.ViewModels
{
    public class PacketGetModel
    {
        public int Id { get; set; }
        public String countryOrigin { get; set; }
        public String sender { get; set; }
        public String countryDestination { get; set; }
        public String addressDestination { get; set; }
        public double cost { get; set; }
        public String awb { get; set; }

        public static PacketGetModel FromPacket(Packet packet)
        {
            return new PacketGetModel
            {
                Id = packet.id,
                countryOrigin = packet.countryOrigin,
                sender = packet.sender,
                countryDestination = packet.countryDestination,
                addressDestination = packet.addressDestination,
                cost = packet.cost,
                awb = packet.awb
            };
        }

    }
}
