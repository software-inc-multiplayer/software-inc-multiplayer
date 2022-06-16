using System;
using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Shared.Managers
{
    public enum RegisterType
    {
        Client,
        Server
    }

    public class RegisterManager : Attribute
    {
        public static readonly Dictionary<GamePacket.PacketOneofCase, IPacketHandler> ClientPacketHandlersCache = new Dictionary<GamePacket.PacketOneofCase, IPacketHandler>();
        public static readonly Dictionary<GamePacket.PacketOneofCase, IPacketHandler> ServerPacketHandlersCache = new Dictionary<GamePacket.PacketOneofCase, IPacketHandler>();
        
        private RegisterType type;
        private GamePacket.PacketOneofCase[] catchers;
        
        public virtual RegisterType Type { get => this.type; set => type = value; }
        public virtual GamePacket.PacketOneofCase[] Catchers { get => this.catchers; set => catchers = value; }

        public RegisterManager(RegisterType type, params GamePacket.PacketOneofCase[] catcher)
        {
            this.type = type;
            this.catchers = catcher;
        }
        
        public static void LoadInstances(GameClient? client, GameServer? server)
        {
            foreach(var t in Assembly.GetCallingAssembly().GetTypes())
            {
                var f = t.GetCustomAttributes(typeof(RegisterManager), true);
                if (f.Length <= 0 && f[0].GetType() == typeof(RegisterManager)) 
                    continue;
                var manager = (RegisterManager)f[0];
                var ctor = t.GetConstructor(new[] { manager.type == RegisterType.Client ? typeof(GameClient) : typeof(GameServer) });
                
                foreach (var catcher in manager.catchers )
                {
                    if (manager.type == RegisterType.Client && ctor != null && client != null)
                    {
                        ClientPacketHandlersCache[catcher] = (IPacketHandler) ctor.Invoke(new object[] { client });
                    }
                    else if (ctor != null && server != null)
                    {
                        ServerPacketHandlersCache[catcher] =(IPacketHandler) ctor.Invoke(new object[] { server });
                    }
                }
                
                
            }
        }
    }
}