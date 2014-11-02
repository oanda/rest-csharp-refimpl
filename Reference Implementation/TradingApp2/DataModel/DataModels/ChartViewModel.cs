using System;
using System.Text;
using TradingApp2.Data;
using TradingApp2.DataModel;
using OANDARestLibrary.TradeLibrary.DataTypes;
using System.Reflection;

namespace TradingApp2.TradeLibrary.DataModels
{
    public class ChartViewModel : TradingApp2.Common.BindableBase
    {
        public ChartViewModel(String instrument)
        {
            Instrument = instrument;
            // Note: we currently don't support changing the period
            Period = "H1";
        }
        
        public string Instrument { get; private set; }
        public CandleData Candles
        {
            get { return RatesDataSource.GetCandles(Instrument, Period); }
        }
        public string Period { get; private set; }
    }
}
