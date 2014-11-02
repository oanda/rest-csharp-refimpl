using System;
using System.Collections.Generic;
using System.Text;
using TradingApp2.Data;
using OANDARestLibrary.TradeLibrary.DataTypes;
using System.Reflection;

namespace TradingApp2.TradeLibrary.DataModels
{
    public class CandleViewModel : DataItem
    {
        public CandleViewModel(Candle data, DataGroup group)
            : base(group)
        {
            _model = data;
        }
        private Candle _model;

        public override string UniqueId
        {
            get
            {
                return Time.ToString();
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
                return OpenMid + " / " + CloseMid;
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
                return HighMid + " / " + LowMid;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override string Content
        {
            get { return "Time: " + Time; }
            set
            {
                base.Content = value;
            }
        }

        public List<double> YValues
        {
            get { return new List<double>() {OpenMid, HighMid, LowMid, CloseMid};}
        }

        //public int Id { get { return _model.id; } }
        //public string Type { get { return _model.type; } }
        //public string Direction { get { return _model.direction; } }
        //public string Instrument { get { return _model.instrument; } }
        //public int Units { get { return _model.units; } }
        public string Time { get { return _model.time; } }
        public double OpenMid { get { return _model.openMid; } }
        public double HighMid { get { return _model.highMid; } }
        public double LowMid { get { return _model.lowMid; } }
        public double CloseMid { get { return _model.closeMid; } }
        public bool Complete { get { return _model.complete; } }
        //public double StopLoss { get { return _model.stopLoss; } }
        //public long Expiry { get { return _model.expiry; } }
        //public double HighLimit { get { return _model.highLimit; } }
        //public double LowLimit { get { return _model.lowLimit; } }
        //public int TrailingStop { get { return _model.trailingStop; } }
        //public int OcaGroupId { get { return _model.ocaGroupId; } }
    }
}
