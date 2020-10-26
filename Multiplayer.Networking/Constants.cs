
namespace Multiplayer.Networking
{
    public enum DisconnectReason
    {
        ServerStop,
        Leaving,
        InvalidPassword,
        Kicked,
        Banned,
        UnhandledPacket,
        InvalidHandshake
    }
}
