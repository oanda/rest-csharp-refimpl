using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using OANDARestLibrary;
using OANDARestLibrary.TradeLibrary;
using OANDARestLibrary.TradeLibrary.DataTypes;
using System.Reflection;


namespace ConsoleTest.Testing
{
	/// <summary>
	/// Goal of this class is to test each piece of the REST library
	/// </summary>
	class RestTest
	{
		private TestResults _results = new TestResults();
		private List<Instrument> _instruments;
		private Semaphore _tickReceived;
		private Semaphore _eventReceived;
		private bool _marketHalted;
		private const string TestInstrument = "EUR_USD";

		private async Task<bool> IsMarketHalted()
		{
			var eurusd = _instruments.Where(x => x.instrument == TestInstrument).ToList();
			var rates = await Rest.GetRatesAsync(eurusd);
			return rates[0].status == "halted";
		}

		public async Task RunTest()
		{
			//Credentials.SetCredentials(EEnvironment.Practice, "SOME_TOKEN", 12345);

			if (Credentials.GetDefaultCredentials() == null)
			{
				throw new Exception("Credentials must be defined to run this test");
			}

			// if run against sandbox, with no account, then autogenerate an account
			if (Credentials.GetDefaultCredentials().IsSandbox && Credentials.GetDefaultCredentials().DefaultAccountId == 0)
			{
				// Create a test account
				var response = await Rest.CreateAccount();
				Credentials.GetDefaultCredentials().DefaultAccountId = response.accountId;
				Credentials.GetDefaultCredentials().Username = response.username;
			}

			if (Credentials.GetDefaultCredentials().HasServer(EServer.Rates))
			{
				await RunInstrumentListTest();

				_marketHalted = await IsMarketHalted();
				// Rates
				await RunRatesTest();
			}
			if (Credentials.GetDefaultCredentials().HasServer(EServer.Labs))
			{
				var labs = new LabsTest(_results);
				await labs.Run();
			}
			Task eventsCheck = null;
			if (Credentials.GetDefaultCredentials().HasServer(EServer.StreamingEvents))
			{
				// Streaming Notifications
				eventsCheck = RunStreamingNotificationsTest();
			}
			if (Credentials.GetDefaultCredentials().HasServer(EServer.Account))
			{
				// Accounts
				await RunAccountsTest();
				// Orders
				await RunOrdersTest();
				// Trades
				await RunTradesTest();
				// Positions
				await RunPositionsTest();
				// Transaction History
				await RunTransactionsTest();
			}
			if (Credentials.GetDefaultCredentials().HasServer(EServer.StreamingRates))
			{
				// Streaming Rates
				RunStreamingRatesTest();
			}
			
			if (eventsCheck != null)
			{
				await eventsCheck;
			}
		}


		private Task RunStreamingNotificationsTest()
		{
			EventsSession session = new EventsSession(_accountId);
			_eventReceived = new Semaphore(0, 100);
			session.DataReceived += OnEventReceived;
			session.StartSession();
			Console.WriteLine("Starting event stream test");
			return Task.Run(() =>
				{
					bool success = _eventReceived.WaitOne(10000);
					session.StopSession();
					_results.Verify(success, "Streaming events successfully received");
				}
			);
		}

		private void OnEventReceived(Event data)
		{
			// _results.Verify the event data
			_results.Verify(data.transaction != null, "Event transaction received");
			if (data.transaction != null)
			{
				_results.Verify(data.transaction.id != 0, "Event data received");
				_results.Verify(data.transaction.accountId != 0, "Account id received");
			}
			_eventReceived.Release();
		}

		private void RunStreamingRatesTest()
		{
			RatesSession session = new RatesSession(_accountId, _instruments);
			_tickReceived = new Semaphore(0, 100);
			session.DataReceived += SessionOnDataReceived;
			session.StartSession();
			Console.WriteLine("Starting rate stream test");
			bool success = _tickReceived.WaitOne(10000);
			session.StopSession();
			_results.Verify(success, "Streaming rates successfully received");
		}

		private void SessionOnDataReceived(RateStreamResponse data)
		{
			// _results.Verify the tick data
			_results.Verify(data.tick != null, "Streaming Tick received");
			if (data.tick != null)
			{
				_results.Verify(data.tick.ask > 0 && data.tick.bid > 0, "Streaming tick has bid/ask");
				_results.Verify(!string.IsNullOrEmpty(data.tick.instrument), "Streaming tick has instrument");
			}
			_tickReceived.Release();
		}

		private async Task RunTransactionsTest()
		{
			// transaction tests

			// Get transaction history basic
			var result = await Rest.GetTransactionListAsync(_accountId);
			_results.Verify(result.Count > 0, "Recent transactions retrieved");
			foreach (var transaction in result)
			{
				_results.Verify(transaction.id > 0, "Transaction has id");
				_results.Verify(!string.IsNullOrEmpty(transaction.type), "Transation has type");
			}
			var parameters = new Dictionary<string, string> {{"count", "500"}};
			result = await Rest.GetTransactionListAsync(_accountId, parameters);
			_results.Verify(result.Count == 500, "Recent transactions retrieved");
			foreach (var transaction in result)
			{
				_results.Verify(transaction.id > 0, "Transaction has id");
				_results.Verify(!string.IsNullOrEmpty(transaction.type), "Transation has type");
			}

			// Get details for a transaction
			var trans = await Rest.GetTransactionDetailsAsync(_accountId, result[0].id);
			_results.Verify(trans.id == result[0].id, "Transaction details retrieved");

			if (!Credentials.GetDefaultCredentials().IsSandbox)
			{	// Not available on sandbox
				// Get Full account history
				var fullHistory = await Rest.GetFullTransactionHistoryAsync(_accountId);
				_results.Verify(fullHistory.Count > 0, "Full transaction history retrieved");
			}
		}

		private async Task RunPositionsTest()
		{
			if (!_marketHalted)
			{
				// Make sure there's a position to test
				await PlaceMarketOrder();

				// get list of open positions
				var positions = await Rest.GetPositionsAsync(_accountId);
				_results.Verify(positions.Count > 0, "Positions retrieved");
				foreach (var position in positions)
				{
					VerifyPosition(position);
				}
			
				// get position for a given instrument
				var onePosition = await Rest.GetPositionAsync(_accountId, TestInstrument);
				VerifyPosition(onePosition);

				// close a whole position
				var closePositionResponse = await Rest.DeletePositionAsync(_accountId, TestInstrument);
				_results.Verify(closePositionResponse.ids.Count > 0 && closePositionResponse.instrument == TestInstrument, "Position closed");
				_results.Verify(closePositionResponse.totalUnits > 0 && closePositionResponse.price > 0, "Position close response seems valid");
			}
			else
			{
				Console.WriteLine("Skipping: Position test because market is halted");
			}
		}

		private void VerifyPosition(Position position)
		{
			_results.Verify(position.units > 0, "Position has units");
			_results.Verify(position.avgPrice > 0, "Position has avgPrice");
			_results.Verify(!string.IsNullOrEmpty(position.side), "Position has direction");
			_results.Verify(!string.IsNullOrEmpty(position.instrument), "Position has instrument");
		}

		// Convenience wrapper
		private int _accountId { get { return Credentials.GetDefaultCredentials().DefaultAccountId;  } }

		private async Task RunTradesTest()
		{
			// trade tests
			await PlaceMarketOrder();

			// get list of open trades
			var openTrades = await Rest.GetTradeListAsync(_accountId);
			_results.Verify(openTrades.Count > 0 && openTrades[0].id > 0, "Trades list retrieved");
			if (openTrades.Count > 0)
			{
				// get details for a trade
				var tradeDetails = await Rest.GetTradeDetailsAsync(_accountId, openTrades[0].id);
				_results.Verify(tradeDetails.id > 0 && tradeDetails.price > 0 && tradeDetails.units != 0, "Trade details retrieved");

				// Modify an open trade
				var request = new Dictionary<string, string>
					{
						{"stopLoss", "0.4"}
					};
				var modifiedDetails = await Rest.PatchTradeAsync(_accountId, openTrades[0].id, request);
				_results.Verify(modifiedDetails.id > 0 && Math.Abs(modifiedDetails.stopLoss - 0.4) < float.Epsilon, "Trade modified");

				if (!_marketHalted)
				{
					// close an open trade
					var closedDetails = await Rest.DeleteTradeAsync(_accountId, openTrades[0].id);
					_results.Verify(closedDetails.id > 0, "Trade closed");
					_results.Verify(!string.IsNullOrEmpty(closedDetails.time), "Trade close details time");
					_results.Verify(!string.IsNullOrEmpty(closedDetails.side), "Trade close details side");
					_results.Verify(!string.IsNullOrEmpty(closedDetails.instrument), "Trade close details instrument");
					_results.Verify(closedDetails.price > 0, "Trade close details price");
					_results.Verify(closedDetails.profit != 0, "Trade close details profit");
				}
				else
				{
					Console.WriteLine("Skipping: Trade delete test because market is halted");
				}
			}
			else
			{
				Console.WriteLine("Skipping: Trade details test because no trades were found");
				Console.WriteLine("Skipping: Trade modify test because no trades were found");
				Console.WriteLine("Skipping: Trade delete test because no trades were found");
			}
		}

		private async Task PlaceMarketOrder()
		{
			if (!_marketHalted)
			{
				// create new market order
				var request = new Dictionary<string, string>
					{
						{"instrument", TestInstrument},
						{"units", "1"},
						{"side", "buy"},
						{"type", "market"},
						{"price", "1.0"}
					};
				var response = await Rest.PostOrderAsync(_accountId, request);
				// We're assuming we don't already have a position on the sell side
				_results.Verify(response.tradeOpened != null && response.tradeOpened.id > 0, "Trade successfully placed");
			}
			else
			{
				Console.WriteLine("Skipping: Market open test because market is halted");
			}
		}

		private async Task RunOrdersTest()
		{

			// 2013-12-06T20:36:06Z
			var expiry = DateTime.Now.AddMonths(1);
			// XmlConvert.ToDateTime and ToString can be used for going to/from RCF3339
			string expiryString = XmlConvert.ToString(expiry, XmlDateTimeSerializationMode.Utc);
			
			// create new pending order
			var request = new Dictionary<string, string>
				{
					{"instrument", TestInstrument},
					{"units", "1"},
					{"side", "buy"},
					{"type", "marketIfTouched"},
					{"expiry", expiryString},
					{"price", "1.0"}
				};
			var response = await Rest.PostOrderAsync(_accountId, request);
			_results.Verify(response.orderOpened != null && response.orderOpened.id > 0, "Order successfully opened");
			// Get open orders
			var orders = await Rest.GetOrderListAsync(_accountId);

			// Get order details
			if (orders.Count == 0)
			{
				Console.WriteLine("Error: No orders to request details for...");
			}
			else
			{
				var order = await Rest.GetOrderDetailsAsync(_accountId, orders[0].id);
				_results.Verify(order.id > 0, "Order details retrieved");
			}

			// Modify an Existing order
			request["units"] += 10;
			var patchResponse = await Rest.PatchOrderAsync(_accountId, orders[0].id, request);
			_results.Verify(patchResponse.id > 0 && patchResponse.id == orders[0].id && patchResponse.units.ToString() == request["units"], "Order patched");

			// close an order
			var deletedOrder = await Rest.DeleteOrderAsync(_accountId, orders[0].id);
			_results.Verify(deletedOrder.id > 0 && deletedOrder.units == patchResponse.units, "Order deleted");
		}

		private async Task RunAccountsTest()
		{
			// Get Account List
			List<Account> result;
			if (Credentials.GetDefaultCredentials().IsSandbox)
			{
				result = await Rest.GetAccountListAsync(Credentials.GetDefaultCredentials().Username);
			}
			else
			{
				result = await Rest.GetAccountListAsync();
			}
			_results.Verify(result.Count > 0, "Accounts are returned");
			foreach(var account in result)
			{
				_results.Verify(VerifyDefaultData(account), "Checking account data for " + account.accountId);
				// Get Account Information
				var accountDetails = await Rest.GetAccountDetailsAsync(account.accountId);
				_results.Verify(VerifyAllData(accountDetails), "Checking account details data for " + account.accountId);
			}
			
			
		}

		private async Task RunRatesTest()
		{
			await RunPricesTest();
			new CandlesTest(_results).Run();
		}

		

		private async Task RunPricesTest()
		{	
			// Get a price list (basic, all instruments)
			var result = await Rest.GetRatesAsync(_instruments);
			_results.Verify(result.Count == _instruments.Count, "Price returned for all " + _instruments.Count + " instruments");
			foreach (var price in result)
			{
				_results.Verify(!string.IsNullOrEmpty(price.instrument), "price has instrument");
				_results.Verify(price.ask > 0 && price.bid > 0, "Seemingly valid rates for instrument " + price.instrument);
			}
		}

		private async Task RunInstrumentListTest()
		{
			// Get an instrument list (basic)
			var result = await Rest.GetInstrumentsAsync(_accountId);
			_results.Verify(result.Count > 0, "Instrument list received");
			foreach(var entry in result)
			{
				_results.Verify(VerifyDefaultData(entry), "Checking instrument data for " + entry.instrument);
			}
			// Store the instruments for other tests
			_instruments = result;
		}

		private bool VerifyAllData<T>(T entry)
		{
			var fields = entry.GetType().GetTypeInfo().DeclaredFields.Where(x => x.Name.StartsWith("Has") && x.FieldType == typeof (bool));
			foreach(var field in fields)
			{
				if ((bool)field.GetValue(entry) == false)
				{
					Console.WriteLine("Fail: " + field.Name + " is missing.");
					return false;
				}
			}
			return true;
		}

		private bool VerifyDefaultData<T>(T entry)
		{
			var fields = entry.GetType().GetTypeInfo().DeclaredFields.Where(x => x.Name.StartsWith("Has") && x.FieldType == typeof(bool));
			foreach (var field in fields)
			{
				bool isOptional = (null != field.GetCustomAttribute(typeof (IsOptionalAttribute)));
				bool valueIsPresent = (bool) field.GetValue(entry);
				// Data should be present iff it is not optional
				if (isOptional == valueIsPresent)
				{
					return false;
				}
			}
			return true;
		}
	}

	internal class TestResult
	{
		public bool Success { get; set; }
		public string Details { get; set; }
	}
}
