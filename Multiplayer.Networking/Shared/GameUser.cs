namespace Multiplayer.Networking.Shared
{
    public class GameUser
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        public UserRole Role { get; set; }
    }
}