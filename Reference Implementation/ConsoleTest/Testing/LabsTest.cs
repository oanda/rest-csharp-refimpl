using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary;

namespace ConsoleTest.Testing
{
	class LabsTest
	{
		private readonly TestResults _results;

		public LabsTest(TestResults results)
		{
			_results = results;
		}

		public async Task Run()
		{
			// Test Calendar
			await RunCalendarTest();
			// Test HPR
			await RunHistoricalPositionRatioTest();
			// Test spreads
			await RunSpreadsTest();
			// Test COT
			await RunCommitmentsOfTradersTest();
			// Test Orderbook
			await RunOrderbookTest();
			// Test Autochartist
			await RunAutochartistTest();
		}

		private async Task RunAutochartistTest()
		{
			var autochartistData = await Rest.GetAutochartistData(new Dictionary<string, string> { {"type", "keylevel"}});
			if (_results.Verify(autochartistData != null, "autochartistData retrieved"))
			{
				_results.Verify(autochartistData.Count > 0, "autochartist signals retrieved");
			}
		}

		private async Task RunOrderbookTest()
		{
			var orderbookData = await Rest.GetOrderbookData("EUR_USD", 604800);
			_results.Verify(orderbookData, "Retrieved orderbook data");
		}

		private async Task RunCommitmentsOfTradersTest()
		{
			var cotData = await Rest.GetCommitmentOfTradersData("EUR_USD");
			if (_results.Verify(cotData != null, "Retrieved cotData"))
			{
				_results.Verify(cotData.Count > 0, "Retrieved cotData snapshots");
				foreach (var period in cotData)
				{
					_results.Verify(period.oi > 0, "cotData oi retrieved");
					_results.Verify(period.ncl > 0, "cotData ncl retrieved");
					_results.Verify(period.price > 0, "cotData price retrieved");
					_results.Verify(period.date > 0, "cotData date retrieved");
					_results.Verify(period.ncs > 0, "cotData ncs retrieved");
					_results.Verify(period.unit, "cotData unit retrieved");
				}
			}
		}

		private async Task RunSpreadsTest()
		{
			var spreadsData = await Rest.GetSpreadData("EUR_USD", 2592000);
			if (_results.Verify(spreadsData != null, "Retrieved spreadsData"))
			{
				_results.Verify(spreadsData.Count > 0, "Retrieved spreadsData snapshots");
				foreach (var period in spreadsData)
				{
					_results.Verify(period.timestamp > 0, "spreads timestamp retrieved");
					_results.Verify(period.max > 0, "spreads max retrieved");
					_results.Verify(period.min > 0, "spreads min retrieved");
					_results.Verify(period.avg > 0, "spreads avg retrieved");
				}
			}
		}

		private async Task RunHistoricalPositionRatioTest()
		{
			var hprData = await Rest.GetHistoricalPostionRatioData("EUR_USD", 2592000);
			if (_results.Verify(hprData != null, "Retrieved hprData"))
			{
				_results.Verify(hprData.Count > 0, "Retrieved hprData snapshots");
				foreach (var hpr in hprData)
				{
					_results.Verify(hpr.timestamp > 0, "hpr timestamp retrieved");
					_results.Verify(hpr.longPositionRatio > 0, "hpr longPositionRatio retrieved");
					_results.Verify(hpr.exchangeRate > 0, "hpr exchangeRate retrieved");
				}
			}
		}

		private async Task RunCalendarTest()
		{
			var calendarEvents = await Rest.GetCalendarData("EUR_USD", 2592000);
			bool detailsVerified = false;
			if (_results.Verify(calendarEvents != null, "Retrieved calendar list"))
			{
				_results.Verify(calendarEvents.Count > 0, "Retrieved calendar events");
				foreach (var calEvent in calendarEvents)
				{
					_results.Verify(calEvent.title, "Event Title retrieved");
					_results.Verify(calEvent.timestamp, "Event timestamp retrieved");
					_results.Verify(calEvent.currency, "Event currency retrieved");
					if (!string.IsNullOrEmpty(calEvent.unit))
					{
						// Forecast isn't always present
						//_results.Verify(calEvent.forecast, "Event forecast retrieved");
						_results.Verify(calEvent.previous, "Event previous retrieved");
						_results.Verify(calEvent.actual, "Event actual retrieved");
						// Market is only present sometimes
						detailsVerified = detailsVerified || !string.IsNullOrEmpty(calEvent.market);
					}
				}
			}
			_results.Verify(detailsVerified, "Confirmed details checked");
		}
	}
}
