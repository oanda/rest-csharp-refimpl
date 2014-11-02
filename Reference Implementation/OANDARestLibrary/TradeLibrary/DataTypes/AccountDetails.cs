using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class AccountDetails
    {
        public int accountId;
        public string name;
        public double balance { get; set; }
        public double unrealizedPl { get; set; }
        public double nav;
        public double realizedPl { get; set; }
        public double marginUsed { get; set; }
        public double marginAvail { get; set; }
        public int openTrades;
        public int openOrders;
        public double marginRate;
        public string homecurr;
    }
}
