using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary.TradeLibrary.DataTypes;

namespace OANDARestLibrary.TradeLibrary
{
	public class RatesSession : StreamSession<RateStreamResponse>
	{
		private readonly List<Instrument> _instruments;

		public RatesSession(int accountId, List<Instrument> instruments) : base(accountId)
		{
			_instruments = instruments;
		}

		protected override async Task<WebResponse> GetSession()
		{
			return await Rest.StartRatesSession(_instruments, _accountId);
		}
	}
}
