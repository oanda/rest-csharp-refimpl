using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
	public class DeletePositionResponse : Response
	{
		public List<long> ids { get; set; }
		public string instrument { get; set; }
		public int totalUnits { get; set; }
		public double price { get; set; }
	}
}
