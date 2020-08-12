using Multiplayer.Debugging;
using Multiplayer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
				ServerClass server = ServerClass.Instance;
				if(server.hasAI)
				{
					Logging.Info("[GameWorld] Populate Gameworld with AI companies");
					Logging.Warn("[GameWorld] AI companies not included in this version!");

				}
				else
				{
					Logging.Info("[GameWorld] Removing all AI companies");
					MarketSimulation.Active.Companies.Clear(); //Remove all AI companies
				}
			}

			private void UpdateClients(object sender, EventArgs e)
			{
				//TODO Update the clients so the gameworld is the same as on the server
				if(oldworld != null)
				{
					Helpers.GameWorldMessage gwm = new Helpers.GameWorldMessage(CompareWorlds());
					ServerClass.Instance.SendGameWorld(gwm);
				}
				else
				{
					Helpers.GameWorldMessage gwm = new Helpers.GameWorldMessage(world);
					ServerClass.Instance.SendGameWorld(gwm);
				}
				
				oldworld = world; //Set oldworld to world to see if anything did change
			}

			public void UpdateClient(Helpers.User user)
			{
				Helpers.GameWorldMessage gwm = new Helpers.GameWorldMessage(world);
				ServerClass.Instance.SendGameWorld(gwm, user);
			}

			private World CompareWorlds()
			{
				World tmpworld = new World();
				//Compare dateTime
				if(world.dateTime != oldworld.dateTime)
				{
					tmpworld.dateTime = world.dateTime;
				}
				//Compare UserCompanies
				foreach(Helpers.UserCompany c in world.UserCompanies)
				{
					if (!oldworld.UserCompanies.Contains(c))
						tmpworld.UserCompanies.Add(c);
				}
				//Compare AICompanies
				foreach(Company c in world.AICompanies)
				{
					if (!oldworld.AICompanies.Contains(c))
						tmpworld.AICompanies.Add(c);
				}
				//Compare Stocks
				foreach(Stock s in world.Stocks)
				{
					if (!oldworld.Stocks.Contains(s))
						tmpworld.Stocks.Add(s);
				}
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

		public class Client
		{
			public static Client Instance;

			public World world;

			public Client()
			{
				Instance = this;
			}
		}
	}
}