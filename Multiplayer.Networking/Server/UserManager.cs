using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telepathy;
using Multiplayer.Debugging;
using Multiplayer.Networking;
using Multiplayer.Extensions;

using UnityEngine;
using System.IO;
using System.Collections;

namespace Multiplayer.Networking
{
    public enum UserRole
    {
        Admin,
        Host,
        Player,
        Guest
    }

    [Serializable]
    public class User
    {
        public int ConnectionID { get; set; }
        public SIMM.Constants.Account SIMM_Account { get; }
        public string Username { get
            {
                if(!IsLoggedIn)
                {
                    return "Guest";
                }
                return SIMM_Account.username;
            } }
        public bool IsLoggedIn { get; set; }
        public string UniqueID { get
            {
                return Guid.NewGuid().ToString();
            } }

        public UserRole Role { get; set; }
        public User(bool DoNotFill)
        {

        }
        public User()
        {
            try
            {
                SIMM_Account = JsonUtility.FromJson<SIMM.Constants.Account>(File.ReadAllText(ModController.ModFolder + "/Multiplayer/account.json"));
                IsLoggedIn = true;
            } catch(Exception e)
            {
                Debug.Log("Had trouble parsing account.json: " + e.ToString());
                IsLoggedIn = false;
            }
        }
    }
    public class UserManager
    {
        public event EventHandler<User> UserAdded;
        public event EventHandler<User> UserRemoved;
        public Dictionary<string, User> Users { get; set; }
        public int GuestCount = 0;
        public bool GetUser(string username, out User user)
        {
            return Users.TryGetValue(username, out user);
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
                username = $"{user.Username} + {GuestCount}";
            }
            Users.Add(username, user);
            UserAdded.Invoke(this, user);
        }
    }
}