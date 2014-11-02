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
    public class PositionViewModel : DataItem
    {
        public PositionViewModel(Position data, DataGroup group)
            : base(group)
        {
            _model = data;
        }
        protected Position _model;

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
                return AveragePrice.ToString();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        //public int Id { get { return _model.id; } }
        //public string Type { get { return _model.type; } }
        public string Side { get { return _model.side; } }
        public string Instrument { get { return _model.instrument; } }
        public int Units { get { return _model.units; } }
        public double AveragePrice { get { return _model.avgPrice; } }
        //public string Time { get { return _model.time; } }
        //public double Price { get { return _model.price; } }
        //public double TakeProfit { get { return _model.takeProfit; } }
        //public double StopLoss { get { return _model.stopLoss; } }
        //public long Expiry { get { return _model.expiry; } }
        //public double HighLimit { get { return _model.highLimit; } }
        //public double LowLimit { get { return _model.lowLimit; } }
        //public int TrailingStop { get { return _model.trailingStop; } }
        //public int OcaGroupId { get { return _model.ocaGroupId; } }
    }
}
