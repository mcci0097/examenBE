using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;
using templateExamen.ViewModels;

namespace templateExamen.Services
{
    public interface IPacketService
    {               
        IEnumerable<PacketGetModel> GetAll();
        Packet Create(PacketPostModel packetModel);
        PacketGetModel Upsert(int id, PacketPostModel packetPostModel);
        Packet Delete(int id);
        PacketGetModel GetById(int id);
    }
    public class PacketService : IPacketService
    {
        private UsersDbContext context;

        private readonly AppSettings appSettings;

        public PacketService(UsersDbContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }

        public IEnumerable<PacketGetModel> GetAll()
        {            
            return context.Packets.Select(packet => new PacketGetModel
            {
                Id = packet.id,
                countryOrigin = packet.countryOrigin,
                sender = packet.sender,
                countryDestination = packet.countryDestination,
                addressDestination = packet.addressDestination,
                cost = packet.cost,
                awb = packet.awb                
            });
        }

        public PacketGetModel GetById(int id)
        {
            Packet packet = context.Packets
                .FirstOrDefault(u => u.id == id);
            if (packet == null)
            {
                return null;
            }
            return PacketGetModel.FromPacket(packet);
        }
        public Packet Create(PacketPostModel packetModel)
        {           
                Packet toAdd = PacketPostModel.ToPacket(packetModel);

                context.Packets.Add(toAdd);
                context.SaveChanges();
                return toAdd;          
        }

        public PacketGetModel Upsert(int id, PacketPostModel user)
        {
            return null;
        }
        public Packet Delete(int id)
        {
            var existing = context.Packets.FirstOrDefault(u => u.id == id);
            if (existing == null)
            {
                return null;
            }
            context.Packets.Remove(existing);
            context.SaveChanges();
            return existing;
        }


    }
}
