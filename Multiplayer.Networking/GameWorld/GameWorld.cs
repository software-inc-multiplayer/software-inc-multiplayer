using Multiplayer.Debugging;
using System;
using System.Collections.Generic;

namespace Multiplayer.Networking
{
    namespace GameWorld
    {
        [Serializable]
        public class World
        {
            public SDateTime dateTime;
            public List<Helpers.UserCompany> UserCompanies = new List<Helpers.UserCompany>();
            public List<Company> AICompanies = new List<Company>();
            public List<StockMarket> StockMarkets = new List<StockMarket>();
            public List<SoftwareProduct> SoftwareProducts = new List<SoftwareProduct>();

            public void ForceUpdate(World content)
            {
                UserCompanies = content.UserCompanies;
                AICompanies = content.AICompanies;
                StockMarkets = content.StockMarkets;
                SoftwareProducts = content.SoftwareProducts;
            }
            public void UpdateData(World content, bool addition)
            {
                Logging.Info($"[GameWorld] UpdateData({addition})");
                try
                {
                    if (addition)
                    {
                        UserCompanies.AddRange(content.UserCompanies);
                        AICompanies.AddRange(content.AICompanies);
                        StockMarkets.AddRange(content.StockMarkets);
                        SoftwareProducts.AddRange(content.SoftwareProducts);
                    }
                    else
                    {
                        foreach (Helpers.UserCompany c in content.UserCompanies)
                        {
                            UserCompanies.RemoveAll(x => x.ID == c.ID);
                        }

                        foreach (Company c in content.AICompanies)
                        {
                            AICompanies.RemoveAll(x => x.ID == c.ID);
                        }

                        foreach (StockMarket s in content.StockMarkets)
                        {
                            StockMarkets.RemoveAll(x => x.Name == s.Name);
                        }

                        foreach (SoftwareProduct s in content.SoftwareProducts)
                        {
                            SoftwareProducts.RemoveAll(x => x.Name == s.Name);
                        }
                    }
                    RefreshData();
                }
                catch (Exception ex)
                {
                    Logging.Error(ex.Message);
                }

                Logging.Info($"[GameWorld] UpdateData() done");
            }

            /// <summary>
            /// Refreshs the ingame data
            /// </summary>
            public void RefreshData()
            {
                Logging.Info("[GameWorld] Refreshing data");
                Logging.Info($"AI Companies: {AICompanies.Count}", $"Player Companies: {UserCompanies.Count}", $"Stockmarkets: {StockMarkets.Count}");
                //Clear all stuff from the client first.
                GameSettings.Instance.StockMarkets.Clear();
                List<Company> tmpcompanies = new List<Company>();
                tmpcompanies.AddRange(MarketSimulation.Active.GetAllCompanies());
                foreach (Company c in tmpcompanies)
                {
                    if (!c.Player)
                    {
                        MarketSimulation.Active.RemoveCompany(c);
                    }
                }

                GameSettings.Instance.StockMarkets.AddRange(StockMarkets); //Add stockmarkets
                MarketSimulation.Active.FixStocks();
                foreach (Company c in AICompanies)
                {
                    MarketSimulation.Active.AddCompany(c, true); //Add AI Companies
                }

                foreach (Helpers.UserCompany c in UserCompanies)
                {
                    if (!c.Player)
                    {
                        MarketSimulation.Active.AddCompany(c.OwnerCompany, true); //Add User Companies
                    }
                }
            }
        }

        [Serializable]
        public class Server : IDisposable
        {
            public static Server Instance;

            public World world = new World();
            public World oldworld = new World(); //When the world is updated save the old world to see what did change and only send the changed stuff
            private bool disposedValue;

            public Server()
            {
                Instance = this;
                TimeOfDay.OnDayPassed += UpdateClients;

                //Create World
                if (Networking.Server.hasAI)
                {
                    Logging.Info("[GameWorld] Populate Gameworld with AI companies");
                    foreach (KeyValuePair<uint, SimulatedCompany> c in MarketSimulation.Active.Companies)
                    {
                        c.Value.Autonomous = false; //Sets the AI companies to do nothing, should be in client only
                        try
                        {

                            world.AICompanies.Add(c.Value);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex.Message, ex.StackTrace);
                        }
                    }

                }
                else
                {
                    Logging.Info("[GameWorld] Removing all AI companies");
                    List<Company> tmpcompanies = new List<Company>();
                    tmpcompanies.AddRange(MarketSimulation.Active.GetAllCompanies());
                    foreach (Company c in tmpcompanies)
                    {
                        if (!c.Player)
                        {
                            MarketSimulation.Active.RemoveCompany(c);
                        }
                    }
                }
            }

            private void UpdateClients(object sender, EventArgs e)
            {
                if (oldworld != null)
                {
                    Logging.Info("[GameWorld] Updating GameWorld");
                    SendGameWorldChanges();
                }
                else
                {
                    Logging.Info("[GameWorld] Sending GameWorld because oldworld is null!");
                    TcpGameWorld gwm = new TcpGameWorld(world, true);
                    Networking.Server.Send(gwm);
                }

                oldworld = world; //Set oldworld to world to see if anything did change
            }

            public void UpdateClient(Helpers.User user)
            {
                if (user == null)
                {
                    Logging.Error("User is null =(");
                    return;
                }
                Logging.Info("[GameWorld] Updating Client " + user.Username);
                SendGameWorldChanges(user);
            }

            private void SendGameWorldChanges(params Helpers.User[] users)
            {
                try
                {
                    TcpGameWorld add = new TcpGameWorld(CompareWorlds(true), true);
                    TcpGameWorld remove = new TcpGameWorld(CompareWorlds(false), false);

                    Logging.Info("[Debug] SendGameWorldChanges() => Add: " + add.Serialize().Length + " Remove: " + remove.Serialize().Length);

                    if (users.Length < 1)
                    {
                        //If users aren't set, it will send it to all users connected to the server
                        foreach (Helpers.User u in Networking.Server.Users.ToArray())
                        {
                            Networking.Server.Send(u.ID, add);
                            Networking.Server.Send(u.ID, remove);
                        }
                        return;
                    }

                    foreach (Helpers.User user in users)
                    {
                        Networking.Server.Send(user.ID, add);
                        Networking.Server.Send(user.ID, remove);
                    }

                }
                catch (Exception ex)
                {
                    Logging.Error(ex.Message, ex.StackTrace);
                }

            }

            /// <summary>
            /// Compares the 'world' gameworld with the 'oldworld gameworld and checks if there is any differences.
            /// If isAdded is false it will check if 'world' has anything removed compared to 'oldworld'
            /// </summary>
            /// <param name="isAdded">If is true (default) it will check if there are any things added into the 'world', if false it will check if there are things removed in the 'world'</param>
            /// <returns>A World object with the changes</returns>
            private World CompareWorlds(bool isAdded)
            {
                World tmpworld = new World();

                if (oldworld == null)
                {
                    Logging.Warn("[GameWorld] oldworld is null but CompareWorlds() is called!");
                    return world;
                }

                //Compare dateTime (Do this with Added & Removed ones. Will probably change it in the future to sync the gametime
                if (world.dateTime != oldworld.dateTime)
                {
                    tmpworld.dateTime = world.dateTime;
                }

                //CHECK IF SOMETHING WAS REMOVED AND RETURN THE TMPWORLD WITH THE REMOVED CONTENT
                if (!isAdded)
                {
                    Logging.Info("[GameWorld] Check if something was removed from servers gameworld");
                    foreach (Helpers.UserCompany c in oldworld.UserCompanies)
                    {
                        if (!world.UserCompanies.Contains(c))
                        {
                            tmpworld.UserCompanies.Add(c);
                        }
                    }

                    foreach (Company c in oldworld.AICompanies)
                    {
                        if (!world.AICompanies.Contains(c))
                        {
                            tmpworld.AICompanies.Add(c);
                        }
                    }

                    foreach (StockMarket s in oldworld.StockMarkets)
                    {
                        if (!world.StockMarkets.Contains(s))
                        {
                            tmpworld.StockMarkets.Add(s);
                        }
                    }

                    Logging.Info($"[GameWorld] {tmpworld.UserCompanies.Count + tmpworld.AICompanies.Count + tmpworld.StockMarkets.Count} objects were removed");
                    return tmpworld;
                }

                //ELSE CHECK IF SOMETHING WAS ADDED
                Logging.Info("[GameWorld] Check if something was added to servers gameworld");

                //Compare UserCompanies
                foreach (Helpers.UserCompany c in world.UserCompanies)
                {
                    if (!oldworld.UserCompanies.Contains(c))
                    {
                        tmpworld.UserCompanies.Add(c);
                    }
                }
                //Compare AICompanies
                foreach (Company c in world.AICompanies)
                {
                    if (!oldworld.AICompanies.Contains(c))
                    {
                        tmpworld.AICompanies.Add(c);
                    }
                }
                //Compare Stocks
                foreach (StockMarket s in world.StockMarkets)
                {
                    if (!oldworld.StockMarkets.Contains(s))
                    {
                        tmpworld.StockMarkets.Add(s);
                    }
                }
                Logging.Info($"[GameWorld] {tmpworld.UserCompanies.Count + tmpworld.AICompanies.Count + tmpworld.StockMarkets.Count} objects were added");
                return tmpworld;
            }

            public void UpdateWorld(World servercontent, bool adds)
            {
                if (adds)
                {
                    Logging.Info("[GameWorld] Will add new content to the clients GameWorld");
                    TcpGameWorld gwc = new TcpGameWorld(servercontent, true);
                    Networking.Server.Send(gwc);
                }
                else
                {
                    Logging.Info("[GameWorld] Will remove content from the clients GameWorld");
                    TcpGameWorld gwc = new TcpGameWorld(servercontent, false);
                    Networking.Server.Send(gwc);
                }
            }

            public void UpdateLocalWorld(World servercontent, bool adds)
            {
                if (adds)
                {
                    Logging.Info("[GameWorld] Will add new content to the servers GameWorld");
                    world.UpdateData(servercontent, adds);
                    world.RefreshData();
                }
                else
                {
                    Logging.Info("[GameWorld] Will remove content from the servers GameWorld");
                    world.UpdateData(servercontent, adds);
                    world.RefreshData();
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        TimeOfDay.OnDayPassed -= UpdateClients;
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        [Serializable]
        public class Client : IDisposable
        {
            public static Client Instance;

            public World world = new World();
            private bool disposedValue;

            public Client(World gworld = null)
            {
                if (gworld == null)
                {
                    world = new World();
                }
                else
                {
                    world = gworld;
                }

                Instance = this;
            }

            public void UpdateWorld(World servercontent, bool adds)
            {
                if (adds)
                {
                    Logging.Info("[GameWorld] Will add new content to the GameWorld");
                    TcpGameWorld gwc = new TcpGameWorld(servercontent, true);
                    Networking.Client.Send(gwc);
                }
                else
                {
                    Logging.Info("[GameWorld] Will remove content from the GameWorld");
                    TcpGameWorld gwc = new TcpGameWorld(servercontent, false);
                    Networking.Client.Send(gwc);
                }
            }

            public void UpdateLocalWorld(World servercontent, bool adds)
            {
                if (adds)
                {
                    Logging.Info("[GameWorld] Client will add new content to the local GameWorld");
                    world.UpdateData(servercontent, adds);
                    world.RefreshData();
                }
                else
                {
                    Logging.Info("[GameWorld] Client will remove content from the local GameWorld");
                    world.UpdateData(servercontent, adds);
                    world.RefreshData();
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {

                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}