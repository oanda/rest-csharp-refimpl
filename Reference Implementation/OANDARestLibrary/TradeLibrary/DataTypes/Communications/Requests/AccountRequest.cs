using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests
{
	abstract class AccountRequest : Request
	{
		private readonly int _accountId;

		AccountRequest(int accountId)
		{
			_accountId = accountId;
		}

		public override string EndPoint
		{
			get { return "/accounts/" + _accountId + GetAccountEndPoint(); }
		}

		protected abstract string GetAccountEndPoint();
		
		public override EServer GetServer()
		{
			return EServer.Account;
		}


	}
}
