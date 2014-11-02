using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using TradingApp2.Data;
using OANDARestLibrary.TradeLibrary.DataTypes.Communications;

namespace TradingApp2.DataModel.DataModels
{
	public abstract class RequestViewModel : DataItem
	{
		protected RequestViewModel(string name, DataGroup group) 
			: base(group)
		{
			MakeRequestCommand = DelegateCommand.FromAsyncHandler(MakeRequest, () => true);
			_name = name;
		}

		public override string UniqueId
		{
			get
			{
				return _name;
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
				return _name;
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
				return "";
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected async Task InternalMakeRequest<T>(Func<Task<T>> func)
		{
			try
			{
				var response = await func();
				ResponseDetails = response.ToString();
			}
			catch (Exception ex)
			{
				ResponseDetails = ex.Message;
			}

			this.OnPropertyChanged("ResponseDetails");
		}

		public string RequestDetails 
		{ 
			get
			{
				var details = new StringBuilder();
				foreach (var pair in requestParams)
				{
					details.AppendLine(pair.Key + ": " + pair.Value);
				}
				return details.ToString();
			}
			set { throw new NotImplementedException(); } 
		}

		public string ResponseDetails { get; set; }

		public DelegateCommand MakeRequestCommand { get; protected set; }

		public abstract Task MakeRequest();

		protected Dictionary<string, string> requestParams;

		private string _name;
	}
}
