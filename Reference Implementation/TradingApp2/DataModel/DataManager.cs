using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLibrary.DataModels;
using TradingApp2.DataModel.DataModels;
using TradingApp2.TradeLibrary.DataModels;

namespace TradingApp2.TradeLibrary
{
    public class DataManager
    {
        private static Dictionary<int, AccountData> accounts = new Dictionary<int, AccountData>();

        public static AccountData GetAccountData(int id)
        {
            if (!accounts.ContainsKey(id))
            {
                accounts.Add(id, new AccountData(id));
            }
            return accounts[id];
        }
    }
}
