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
			public List<Stock> Stocks = new List<Stock>();
		}

		[Serializable]
		public class Server : IDisposable
		{
			public static Server Instance;

			public World world;
			public World oldworld; //When the world is updated save the old world to see what did change and only send the changed stuff
			private bool disposedValue;

			public Server()
			{
				Instance = this;
				TimeOfDay.OnDayPassed += UpdateClients;

				//Create World
				if (Networking.Server.hasAI)
				{
					Logging.Info("[GameWorld] Populate Gameworld with AI companies");
					Logging.Warn("[GameWorld] AI companies not included in this version!");

				}
				else
				{
					Logging.Info("[GameWorld] Removing all AI companies");
					MarketSimulation.Active.Companies.Clear(); //Remove all AI companies (DEACTIVATE IN CONSOLE MODE!)
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
					Helpers.TcpGameWorld gwm = new Helpers.TcpGameWorld(world, true);
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

			void SendGameWorldChanges(params Helpers.User[] users)
			{
				Helpers.TcpGameWorld add = new Helpers.TcpGameWorld(CompareWorlds(true), true);
				Helpers.TcpGameWorld remove = new Helpers.TcpGameWorld(CompareWorlds(false), false);

				if(users.Length < 1)
				{
					//If users aren't set, it will send it to all users connected to the server
					users = Networking.Server.Users.ToArray();
				}

				foreach(Helpers.User user in users)
				{
					Networking.Server.Send(user.ID, add);
					Networking.Server.Send(user.ID, remove);
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

				if(oldworld == null)
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
						if (!world.UserCompanies.Contains(c))
							tmpworld.UserCompanies.Add(c);

					foreach (Company c in oldworld.AICompanies)
						if (!world.AICompanies.Contains(c))
							tmpworld.AICompanies.Add(c);

					foreach (Stock s in oldworld.Stocks)
						if (!world.Stocks.Contains(s))
							tmpworld.Stocks.Add(s);

					Logging.Info($"[GameWorld] {tmpworld.UserCompanies.Count + tmpworld.AICompanies.Count + tmpworld.Stocks.Count} objects were removed");
					return tmpworld;
				}

				//ELSE CHECK IF SOMETHING WAS ADDED
				Logging.Info("[GameWorld] Check if something was added to servers gameworld");

				//Compare UserCompanies
				foreach (Helpers.UserCompany c in world.UserCompanies)
				{
					if (!oldworld.UserCompanies.Contains(c))
						tmpworld.UserCompanies.Add(c);
				}
				//Compare AICompanies
				foreach (Company c in world.AICompanies)
				{
					if (!oldworld.AICompanies.Contains(c))
						tmpworld.AICompanies.Add(c);
				}
				//Compare Stocks
				foreach (Stock s in world.Stocks)
				{
					if (!oldworld.Stocks.Contains(s))
						tmpworld.Stocks.Add(s);
				}
				Logging.Info($"[GameWorld] {tmpworld.UserCompanies.Count + tmpworld.AICompanies.Count + tmpworld.Stocks.Count} objects were added");
				return tmpworld;
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

			public World world;
			private bool disposedValue;

			public Client(World gworld = null)
			{
				if (gworld == null)
					world = new World();
				else
					world = gworld;

				Instance = this;
			}

			public void UpdateWorld(World servercontent, bool adds)
			{
				if (adds)
				{
					Logging.Info("[GameWorld] Will add new content to the GameWorld");
					Helpers.TcpGameWorld gwc = new Helpers.TcpGameWorld(servercontent, true);
					Networking.Client.Send(gwc);
				}
				else
				{
					Logging.Info("[GameWorld] Will remove content from the GameWorld");
					Helpers.TcpGameWorld gwc = new Helpers.TcpGameWorld(servercontent, false);
					Networking.Client.Send(gwc);
				}
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						// TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
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