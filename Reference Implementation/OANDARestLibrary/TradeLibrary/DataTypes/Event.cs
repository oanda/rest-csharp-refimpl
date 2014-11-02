using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes
{
	public class Event : IHeartbeat
	{
		public Heartbeat heartbeat { get; set; }
		public Transaction transaction { get; set; }
		public bool IsHeartbeat()
		{
			return (heartbeat != null);
		}
	}
}
