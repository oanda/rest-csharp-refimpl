using System;
using System.Reflection;
using System.Text;
using TradingApp2.Data;
using OANDARestLibrary.TradeLibrary.DataTypes;

namespace TradingApp2.TradeLibrary.DataModels
{
    public class TransactionViewModel : DataItem
    {
        public TransactionViewModel(Transaction data, DataGroup group)
            : base(group)
        {
            _model = data;
        }
        protected Transaction _model;

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
                return Type + " " + Instrument + " " + Units;
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
                return "P/L: " + ProfitLoss + "\tBalance: " + Balance;
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
				// use reflection to display all the properties that have non default values
                StringBuilder result = new StringBuilder();
	            var props = typeof (TransactionViewModel).GetTypeInfo().DeclaredProperties;
				foreach (var prop in props)
				{
					if (prop.Name != "Content" && prop.Name != "Subtitle" && prop.Name != "Title" && prop.Name != "UniqueId")
					{
						object value = prop.GetValue(this);
						bool valueIsNull = value == null;
						object defaultValue = OANDARestLibrary.Framework.Common.GetDefault(prop.PropertyType);
						bool defaultValueIsNull = defaultValue == null;
						if ((valueIsNull != defaultValueIsNull) // one is null when the other isn't
							|| (!valueIsNull && (value.ToString() != defaultValue.ToString()))) // both aren't null, so compare as strings
						{
							result.AppendLine(prop.Name + " : " + prop.GetValue(this));
						}
					}
				}

                return result.ToString();
            }
            set
            {
                base.Content = value;
            }
        }

        public long Id { get { return _model.id; } }
        public int AccountId { get { return _model.accountId; } }
        public string Type { get { return _model.type; } }
        public string Side { get { return _model.side; } }
        public string Instrument { get { return _model.instrument; } }
        public int Units { get { return _model.units; } }
        public string Time { get { return _model.time; } }
        public double Price { get { return _model.price; } }
        public double Balance { get { return _model.accountBalance; } }
        public double Interest { get { return _model.interest; } }
        public double ProfitLoss { get { return _model.pl; } }
        public double TakeProfit { get { return _model.takeProfitPrice; } }
        public double StopLoss { get { return _model.stopLossPrice; } }
        public string Expiry { get { return _model.expiry; } }
		public double UpperBound { get { return _model.upperBound; } }
        public double LowerBound { get { return _model.lowerBound; } }
        public double TrailingStop { get { return _model.trailingStopLossDistance; } }
		public string Reason { get { return _model.reason; } }
    }
}
