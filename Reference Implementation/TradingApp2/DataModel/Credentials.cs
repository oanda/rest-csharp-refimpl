using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingApp2.DataModel
{
	class Credentials
	{
		public bool HasAccountServer()
		{
			return !string.IsNullOrEmpty(accountServer);
		}

		public bool HasRatesServer()
		{
			return !string.IsNullOrEmpty(ratesServer);
		}

		public bool HasStreamingRatesServer()
		{
			return !string.IsNullOrEmpty(streamingServer);
		}

		public bool HasStreamingNotificationsServer()
		{
			return !string.IsNullOrEmpty(streamingNotificationsServer);
		}

		public string accountServer;
		public string ratesServer;
		public string streamingServer;
		public string accessToken;
		public string streamingNotificationsServer;

		public string Server { 
			set 
			{ 
				accountServer = value;
				ratesServer = value;
			} 
		}

		private static Credentials _instance;
		public int defaultAccountId;

		public static Credentials GetDefaultCredentials()
		{
			if (_instance == null)
			{
				_instance = GetPracticeCredentials();
			}
			return _instance;
		}

		private static Credentials GetMaxCredentials()
		{
			return new Credentials()
			{
				defaultAccountId = 1,
				accountServer = "https://max:1342/v1/",
				//ratesServer = "http://max:1341/v1/",
				//streamingServer = "http://max:1342/v1/",
				streamingNotificationsServer = "http://max:9605/v1/",
				accessToken = "testusr-token",
			};
		}

		private static Credentials GetDevCredentials()
		{
			return new Credentials()
			{
				Server = "https://oanda-cs-dev:1342/v1/",
				streamingServer = "https://oanda-cs-dev:1342/v1/",
				accessToken = "b75fc8bfad973495bbfd89dbb3d3c1a9-372182bf5fea53924ce3b5de2a0f12ce",
			};
		}

		private static Credentials GetSandboxCredentials()
		{
			return new Credentials()
				{
					defaultAccountId = 9304262,
					Server = "http://api-sandbox.oanda.com/",
				};
		}

		private static Credentials GetPracticeCredentials()
		{
			return new Credentials()
				{
					defaultAccountId = 963080,
					Server = " https://api-fxpractice.oanda.com/v1/",
					streamingServer = "https://stream-fxpractice.oanda.com/v1/",
					accessToken = "7cd55fd46f12de2da0bc575fd87fb442-25a371bf739f584337f85e0579b7a4ea",
				};
		}

		private static Credentials GetPracticeCredentialsOC()
		{
			return new Credentials()
			{
				defaultAccountId = 621396,
				Server = " https://api-fxpractice.oanda.com/v1/",
				streamingServer = "https://stream-fxpractice.oanda.com/v1/",
				accessToken = "adcd061177955da97ce40eb91dda28fa-611fe3596a0c6110a75b84f129860fd1",
			};
		}

		private static Credentials GetLiveCredentials()
		{
			throw new NotImplementedException();
		}
	}
}
