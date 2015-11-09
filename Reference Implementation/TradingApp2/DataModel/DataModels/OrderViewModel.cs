using System;
using System.Text;
using TradingApp2.Data;
using OANDARestLibrary.TradeLibrary.DataTypes;
using System.Reflection;

namespace TradingApp2.TradeLibrary.DataModels
{
    public class OrderViewModel : DataItem
    {
        public OrderViewModel(Order data, DataGroup group)
            : base(group)
        {
            _model = data;
        }
        protected Order _model;

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

        public override string Content
        {
            get
            {
                StringBuilder result = new StringBuilder();
                result.AppendLine("Id : " + Id);
                result.AppendLine("Type : " + Type);
                result.AppendLine("Side : " + Side);
                result.AppendLine("Instrument : " + Instrument);
                result.AppendLine("Units : " + Units);
                result.AppendLine("Time : " + Time);
                result.AppendLine("Price : " + Price);
                result.AppendLine("TakeProfit : " + TakeProfit);
                result.AppendLine("StopLoss : " + StopLoss);
                result.AppendLine("Expiry : " + Expiry);
				result.AppendLine("UpperBound : " + UpperBound);
				result.AppendLine("LowerBound : " + LowerBound);
                result.AppendLine("TrailingStop : " + TrailingStop);

                return result.ToString();
            }
            set
            {
                base.Content = value;
            }
        }

        public long Id { get { return _model.id; } }
        public string Type { get { return _model.type; } }
        public string Side { get { return _model.side; } }
        public string Instrument { get { return _model.instrument; } }
        public int Units { get { return _model.units; } }
        public string Time { get { return _model.time; } }
        public double Price { get { return _model.price; } }
        public double TakeProfit { get { return _model.takeProfit; } }
        public double StopLoss { get { return _model.stopLoss; } }
        public string Expiry { get { return _model.expiry; } }
        public double UpperBound { get { return _model.upperBound; } }
        public double LowerBound { get { return _model.lowerBound; } }
        public int TrailingStop { get { return _model.trailingStop; } }
    }
}
