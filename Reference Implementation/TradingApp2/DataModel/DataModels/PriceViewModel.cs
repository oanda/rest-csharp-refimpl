using System;
using System.Text;
using TradingApp2.Data;
using TradingApp2.DataModel;
using OANDARestLibrary.TradeLibrary.DataTypes;
using System.Reflection;

namespace TradingApp2.TradeLibrary.DataModels
{
    public class PriceViewModel : DataItem
    {
        public PriceViewModel(Price data, DataGroup group)
            : base(group)
        {
            _model = data;
            _chart = new ChartViewModel(Instrument);
        }
        protected Price _model;

        public override string UniqueId
        {
            get
            {
                return Instrument;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override string Title
        {
            get
            {
                return Instrument;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override string Subtitle
        {
            get
            {
                return Bid + " / " + Ask;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override string Content
        {
            get { return "Last Tick: " + Time; }
            set
            {
                base.Content = value;
            }
        }

        private ChartViewModel _chart;
        public override ChartViewModel Chart
        {
            get { return _chart; }
        }

        //public int Id { get { return _model.id; } }
        //public string Type { get { return _model.type; } }
        //public string Direction { get { return _model.direction; } }
        public string Instrument { get { return _model.instrument; } }
        //public int Units { get { return _model.units; } }
        public string Time { get { return _model.time; } }
        public double Bid { get { return _model.bid; } }
        public double Ask { get { return _model.ask; } }
        //public double StopLoss { get { return _model.stopLoss; } }
        //public long Expiry { get { return _model.expiry; } }
        //public double HighLimit { get { return _model.highLimit; } }
        //public double LowLimit { get { return _model.lowLimit; } }
        //public int TrailingStop { get { return _model.trailingStop; } }
        //public int OcaGroupId { get { return _model.ocaGroupId; } }
        public void Update(Price price)
        {
            _model.update(price);
			this.OnPropertyChanged("Subtitle");
			this.OnPropertyChanged("Content");
        }
    }
}
