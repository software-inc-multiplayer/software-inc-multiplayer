#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Shared
{
    public enum UserRole
    {
        Host,
        Admin,
        Player,
        Guest
    }

    public interface IUserManager
    {
        event EventHandler<UserAddedEventArgs> UserAdded;
        event EventHandler<UserRemovedEventArgs> UserRemoved;

        bool HasUser(ulong userId);
        GameUser GetUser(ulong userId);
        GameUser GetOrAddUser(GameUser user);
        void RemoveUser(ulong userId);
        void RemoveUser(GameUser user);

        void Clear();
    }

    /// <summary>
    /// The UserManager holds information for all known users on a server
    /// </summary>
    public class UserManager : IUserManager
    {
        /*public event EventHandler<User> UserAdded;
        public event EventHandler<User> UserRemoved;
        public Dictionary<string, User> Users { get; set; }
        public int GuestCount = 0;
        public bool GetUser(string username, out User user)
        {
            return Users.TryGetValue(username, out user);
        }
        public void RemoveUser(int ID)
        {

        }
        public void RemoveUser(string username)
        {
            bool exists = Users.TryGetValue(username, out User user);
            if (!exists) return;
            UserRemoved.Invoke(this, user);
            Users.Remove(username);
        }
        public void AddUser(User user) {
            string username = "";
            if(!user.IsLoggedIn)
            {
                GuestCount++;
                // TODO fixme
                //username = $"{user.Username} + {GuestCount}";
            }
            Users.Add(username, user);
            UserAdded.Invoke(this, user);
        }*/

        #region Events
        public event EventHandler<UserAddedEventArgs> UserAdded;
        public event EventHandler<UserRemovedEventArgs> UserRemoved;
        #endregion

        private readonly Dictionary<ulong, GameUser> userIdToUser = new Dictionary<ulong, GameUser>();
        private readonly Dictionary<ulong, BanInformation> bannedUsers = new Dictionary<ulong, BanInformation>();

        public UserManager()
        {
            // TODO implement persistance
        }

        public bool HasUser(ulong userId)
        {
            return this.userIdToUser.ContainsKey(userId);
        }

        public GameUser GetUser(ulong userId)
        {
            if (this.userIdToUser.TryGetValue(userId, out var user))
                return user;
            return null;
        }

        public GameUser GetOrAddUser(GameUser user)
        {
            if (!this.userIdToUser.TryGetValue(user.Id, out var oldUser))
            {
                this.userIdToUser.Add(user.Id, user);

                this.UserAdded?.Invoke(this, new UserAddedEventArgs(user));
                return user;
            }

            return oldUser;
        }

        
        public BanInformation? CheckBanned(GameUser user)
        {
            return this.bannedUsers.TryGetValue(user.Id, out var banInfo) ? banInfo : null;
        }

        public void RemoveUser(ulong userId)
        {
            if (!this.userIdToUser.TryGetValue(userId, out var removeUser))
                return;

            this.userIdToUser.Remove(removeUser.Id);

            this.UserRemoved?.Invoke(this, new UserRemovedEventArgs(removeUser));
        }

        public void RemoveUser(GameUser user)
        {
            this.RemoveUser(user.Id);
        }

        public void Clear()
        {
            foreach(var removedUser in this.userIdToUser.Select(x => x.Value).ToList())
            {
                this.userIdToUser.Remove(removedUser.Id);
                this.UserRemoved?.Invoke(this, new UserRemovedEventArgs(removedUser));
            }
        }
    }

    public class UserAddedEventArgs : EventArgs
    {

        public UserAddedEventArgs(GameUser user)
        {
            User = user;
        }

        public GameUser User { get; }
    }

    public class UserRemovedEventArgs : EventArgs
    {

        public UserRemovedEventArgs(GameUser user)
        {
            User = user;
        }

        public GameUser User { get; }
    }
}