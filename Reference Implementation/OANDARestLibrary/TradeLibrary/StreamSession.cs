using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using OANDARestLibrary.TradeLibrary.DataTypes;

namespace OANDARestLibrary.TradeLibrary
{
	public abstract class StreamSession<T> where T: IHeartbeat
	{
		protected readonly int _accountId;
		private WebResponse _response;
		private bool _shutdown;

		public delegate void DataHandler(T data);

		public event DataHandler DataReceived;

		public void OnDataReceived(T data)
		{
			DataHandler handler = DataReceived;
			if (handler != null) handler(data);
		}

		protected StreamSession(int accountId)
		{
			_accountId = accountId;
		}

		protected abstract Task<WebResponse> GetSession();
		
		public async void StartSession()
		{
			_shutdown = false;
			_response = await GetSession();
			

			Task.Run(() =>
				{
					DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
					StreamReader reader = new StreamReader(_response.GetResponseStream());
					while (!_shutdown)
					{
						MemoryStream memStream = new MemoryStream();
						
						string line = reader.ReadLine();
						memStream.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetByteCount(line));
						memStream.Position = 0;

						var data = (T)serializer.ReadObject(memStream);
						
						// Don't send heartbeats
						if (!data.IsHeartbeat())
						{
							OnDataReceived(data);
						}
					}
				}
				);

		}

		public void StopSession()
		{
		        _response.Dispose();
			_shutdown = true;
		}
	}
}
