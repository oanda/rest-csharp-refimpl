using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary.TradeLibrary.DataTypes.Communications;
using OANDARestLibrary.TradeLibrary.DataTypes.Communications.Requests;
using TradingApp2.Data;
using Microsoft.Practices.Prism.Commands;

namespace TradingApp2.DataModel.DataModels
{
	/// <summary>
	/// Storace for helper data to help make playing with requests more effective
	/// </summary>
	class RequestHelpers
	{
		public static long lastTradeId = 0;
		public static long lastOrderId = 0;
	}

	class OrderPostViewModel : RequestViewModel
	{
		public OrderPostViewModel(string name, DataGroup group, Dictionary<string, string> parameters ) 
			: base(name, group)
		{
			requestParams = parameters;
		}

		public override async Task MakeRequest()
		{
			await InternalMakeRequest(async () =>
				{
					var result = await OANDARestLibrary.Rest.PostOrderAsync(AccountDataSource.DefaultDataSource.Id, requestParams);
					if (result.tradeOpened != null)
					{
						RequestHelpers.lastTradeId = result.tradeOpened.id;
					}
					if (result.orderOpened != null)
					{
						RequestHelpers.lastOrderId = result.orderOpened.id;
					}
					return result;
				} );
		}
	}

	class OrderPatchViewModel : RequestViewModel
	{
		public OrderPatchViewModel(string name, DataGroup group, Dictionary<string, string> parameters)
			: base(name, group)
		{
			requestParams = parameters;
		}

		public override async Task MakeRequest()
		{
			await InternalMakeRequest(() => OANDARestLibrary.Rest.PatchOrderAsync(AccountDataSource.DefaultDataSource.Id, RequestHelpers.lastOrderId, requestParams));
		}
	}

	class OrderDeleteViewModel : RequestViewModel
	{
		public OrderDeleteViewModel(string name, DataGroup group)
			: base(name, group)
		{
			requestParams = new Dictionary<string, string>();
		}

		public override async Task MakeRequest()
		{
			await InternalMakeRequest(() => OANDARestLibrary.Rest.DeleteOrderAsync(AccountDataSource.DefaultDataSource.Id, RequestHelpers.lastOrderId));
		}
	}

	class TradePatchViewModel : RequestViewModel
	{
		public TradePatchViewModel(string name, DataGroup group, Dictionary<string, string> parameters)
			: base(name, group)
		{
			requestParams = parameters;
		}

		public override async Task MakeRequest()
		{
			await InternalMakeRequest(() => OANDARestLibrary.Rest.PatchTradeAsync(AccountDataSource.DefaultDataSource.Id, RequestHelpers.lastTradeId, requestParams));
		}
	}

	class TradeDeleteViewModel : RequestViewModel
	{
		public TradeDeleteViewModel(string name, DataGroup group)
			: base(name, group)
		{
			requestParams = new Dictionary<string, string>();
		}

		public override async Task MakeRequest()
		{
			await InternalMakeRequest(() => OANDARestLibrary.Rest.DeleteTradeAsync(AccountDataSource.DefaultDataSource.Id, RequestHelpers.lastTradeId));
		}
	}

	class PositionDeleteViewModel : RequestViewModel
	{
		public PositionDeleteViewModel(string name, DataGroup group, string instrument)
			: base(name, group)
		{
			requestParams = new Dictionary<string, string>() { { "instrument", instrument } };
		}

		public override async Task MakeRequest()
		{
			await InternalMakeRequest(() => OANDARestLibrary.Rest.DeletePositionAsync(AccountDataSource.DefaultDataSource.Id, requestParams["instrument"]));
		}
	}
}
