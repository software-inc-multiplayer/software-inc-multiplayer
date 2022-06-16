using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;
using Multiplayer.Packets;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Shared.Managers
{
    public enum RegisterType
    {
        Client,
        Server
    }

    public class RegisterManager : Attribute
    {
        public static readonly Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>> ClientPacketHandlersCache = new Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>>();
        public static readonly Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>> ServerPacketHandlersCache = new Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>>();
        
        private RegisterType type;
        private GamePacket.PacketOneofCase[] catchers;
        
        public virtual RegisterType Type { get => this.type; set => type = value; }
        public virtual GamePacket.PacketOneofCase[] Catchers { get => this.catchers; set => catchers = value; }

        public RegisterManager(RegisterType type, params GamePacket.PacketOneofCase[] catcher)
        {
            this.type = type;
            this.catchers = catcher;
        }
        
        public static void LoadInstances(ILogger logger, GameClient? client, GameServer? server)
        {
            logger.Info("Loading Handlers @ " + (client != null ? "Client" : "Server"));
            foreach(var t in Assembly.GetCallingAssembly().GetTypes())
            {
                var f = t.GetCustomAttributes(typeof(RegisterManager), true);
                
                if (f.Length <= 0) 
                    continue;
                
                logger.Info("Found class with RegisterManager: " + t.Name);
                
                var manager = (RegisterManager)f[0];
                
                logger.Info("RegisterManager at " + t.Name + " catches: " + string.Join(" - ", manager.catchers.Select(s => s.ToString()).ToArray()));
                
                var ctor = t.GetConstructor(new[] { manager.type == RegisterType.Client ? typeof(GameClient) : typeof(GameServer) });
                
                foreach (var catcher in manager.catchers )
                {
                    if (ctor == null)
                        break;

                    if (manager.type == RegisterType.Client && client != null) 
                    {
                        logger.Info("Registered Catcher - " + catcher );
                        if (ClientPacketHandlersCache[catcher] != null)
                        {
                            ClientPacketHandlersCache[catcher].Add((IPacketHandler) ctor.Invoke(new object[] { client }));
                        }
                        else
                        {
                            ClientPacketHandlersCache[catcher] = new List<IPacketHandler> { (IPacketHandler)ctor.Invoke(new object[] { client }) };
                        }
                        
                    }
                    if (manager.type != RegisterType.Server && server != null)
                    {
                        if (ServerPacketHandlersCache[catcher] != null)
                        {
                            ServerPacketHandlersCache[catcher].Add((IPacketHandler) ctor.Invoke(new object[] { server }));
                        }
                        else
                        {
                            ServerPacketHandlersCache[catcher] = new List<IPacketHandler> { (IPacketHandler)ctor.Invoke(new object[] { server }) };
                        }
                    }
                }
                
                
            }
        }
    }
}