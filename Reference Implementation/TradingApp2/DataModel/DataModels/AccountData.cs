using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using OANDARestLibrary.TradeLibrary.DataTypes;
using Framework;
using OANDARestLibrary.Framework;
using OANDARestLibrary;

namespace TradingApp2.DataModel.DataModels
{
    public class AccountData : ObservableObject
    {
        DispatcherTimer _transTimer;

        long _currentTrans;

        public AccountData(int id)
        {
            Id = id;
        }

        public void EnableUpdates()
        {
            if ( _transTimer == null )
            {
                _transTimer = new DispatcherTimer();
                _transTimer.Tick += Refresh;
                _transTimer.Interval = new TimeSpan( 0, 0, 0, 1 );
                _transTimer.Start();
            }
        }
        
        public int Id { get; private set; }

        public AccountDetails Details { get; set; }

        private ObservableCollection<Position> _positions;
        private ObservableCollection<Order> _orders;
        private ObservableCollection<TradeData> _trades;
        private ObservableCollection<Transaction> _transactions;

        public ObservableCollection<Position> Positions { get { return _positions; } set { _positions = value; RaisePropertyChanged("Positions"); } }
        public ObservableCollection<Order> Orders { get { return _orders; } set { _orders = value; RaisePropertyChanged("Orders"); } }
        public ObservableCollection<TradeData> Trades { get { return _trades; } set { _trades = value; RaisePropertyChanged("Trades"); } }
        public ObservableCollection<Transaction> Transactions { get { return _transactions; } set { _transactions = value; RaisePropertyChanged("Transactions"); } }

        private void Refresh( object sender, object e )
        {
            Refresh();
        }

        public event EventHandler<CustomEventArgs<Transaction>> NewTransaction;

        public void OnNewTransaction(Transaction e)
        {
            EventHandler<CustomEventArgs<Transaction>> handler = NewTransaction;
            if (handler != null) handler(this, new CustomEventArgs<Transaction>(e));
        }

        public async void Refresh()
        {
	        var transParams = new Dictionary<string, string> {{"minId", "" + (_currentTrans + 1)}};
			var newTransactions = await Rest.GetTransactionListAsync(Id, transParams);
            if ( newTransactions.Count > 0 )
            {
                _currentTrans = newTransactions[0].id;
                foreach (var newTransaction in newTransactions)
                {
                    OnNewTransaction(newTransaction);
                }

                // these can't change unless there's been a transaction...
                Trades = GetObservable(await Rest.GetTradeListAsync(Id));
                Orders = GetObservable(await Rest.GetOrderListAsync(Id));
                Transactions = GetObservable(await Rest.GetTransactionListAsync(Id));
                Positions = GetObservable(await Rest.GetPositionsAsync(Id));
            }
        }


        public ObservableCollection<T> GetObservable<T>(List<T> list)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in list)
            {
                collection.Add(item);
            }
            return collection;
        }
    }
}
