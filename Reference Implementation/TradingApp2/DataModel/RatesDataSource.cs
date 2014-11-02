using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary;
using TradingApp2.Data;
using TradingApp2.TradeLibrary;
using TradingApp2.TradeLibrary.DataModels;

namespace TradingApp2.DataModel
{
    public class CandleData : DataGroup
    {
        public CandleData(string instrument, string granularity)
            : base(instrument + granularity, instrument, granularity, "", "")
        {
            Instrument = instrument;
            Granularity = granularity;
            RequestDataUpdate();
        }

        public async void RequestDataUpdate()
        {
            var candles = await Rest.GetCandlesAsync(Instrument, Granularity);
            Items.Clear();
            foreach (var candle in candles)
            {
                Items.Add(new CandleViewModel(candle, this));
            }
        }

        public string Instrument { get; set; }
        public string Granularity { get; set; }
    }

    public class RatesDataSource
    {
        private static RatesDataSource _ratesDataSource = new RatesDataSource();

        private ObservableCollection<CandleData> _allCandles = new ObservableCollection<CandleData>();
        public ObservableCollection<CandleData> AllCandles
        {
            get { return _allCandles; }
        }

        public static CandleData GetCandles(string instrumentName, string granularity)
        {
            var matches = _ratesDataSource._allCandles.Where((group) => group.UniqueId.Equals(instrumentName + granularity));
            if (matches.Count() == 1) return matches.First();
            // request the missing data
            var newGroup = new CandleData(instrumentName, granularity);
            _ratesDataSource._allCandles.Add(newGroup);
            return newGroup;
        }
    }
}
