using Multiplayer.Networking.Shared;

namespace Multiplayer.Networking
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public bool HasPassword { get => string.IsNullOrEmpty(this.Password); }
        public UserRole DefaultRole { get; set; }
        public int Port { get; set; }
    }
}