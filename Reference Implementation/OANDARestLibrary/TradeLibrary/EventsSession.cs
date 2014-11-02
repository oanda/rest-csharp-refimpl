using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary.TradeLibrary.DataTypes;

namespace OANDARestLibrary.TradeLibrary
{
	public class EventsSession : StreamSession<Event>
	{
		public EventsSession(int accountId) : base(accountId)
		{
		}

		protected override async Task<WebResponse> GetSession()
		{
			return await Rest.StartEventsSession(new List<int> {_accountId});
		}
	}
}
