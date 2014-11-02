using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary.TradeLibrary.DataTypes;

namespace TradeLibrary.DataModels
{
    public class UserData
    {
        public List<Account> Accounts { get; private set;  }

        //public AccountData SelectedAccount { get; }
    }
}
