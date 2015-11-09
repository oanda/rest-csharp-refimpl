using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp2.Data;
using OANDARestLibrary.TradeLibrary.DataTypes;

namespace TradeLibrary.DataModels
{
    public class TradeViewModel : DataItem
    {
        public TradeViewModel(TradeData data, DataGroup group)
            : base(group)
        {
            _model = data;
        }
        protected TradeData _model;

        public override string UniqueId
        {
            get
            {
                return Id.ToString();
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
                return Side + " " + Instrument + " " + Units;
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
                return Price + " : " + TakeProfit + "/" + StopLoss;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public long Id { get { return _model.id; } }
        public string Side { get { return _model.side; } }
        public string Instrument { get { return _model.instrument; } }
        public int Units { get { return _model.units; } }
        public string Time { get { return _model.time; } }
        public double Price { get { return _model.price; } }
        public double TakeProfit { get { return _model.takeProfit; } }
        public double StopLoss { get { return _model.stopLoss; } }
        public int TrailingStop { get { return _model.trailingStop; } }
		public double TrailingAmount { get { return _model.trailingAmount; } }
    }
}
