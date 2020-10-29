namespace Multiplayer.Networking.Shared
{
    public class GameUser
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public UserRole Role { get; set; }
    }
}