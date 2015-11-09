using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using TradingApp2.Common;
using TradingApp2.DataModel;
using TradingApp2.DataModel.DataModels;
using TradingApp2.TradeLibrary;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using TradeLibrary;
using TradingApp2.TradeLibrary.DataModels;
using OANDARestLibrary.TradeLibrary.DataTypes;
using OANDARestLibrary;
using OANDARestLibrary.Framework;
using OANDARestLibrary.TradeLibrary;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace TradingApp2.Data
{
    /// <summary>
    /// Base class for <see cref="DataItem"/> and <see cref="DataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class DataCommon : TradingApp2.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public DataCommon()
        { }

        public DataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public virtual string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public virtual string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public virtual string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public virtual string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(DataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class DataItem : DataCommon
    {
        public DataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, DataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        public DataItem(DataGroup group)
        {
            this._group = group;
        }

        private string _content = string.Empty;
        public virtual string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private DataGroup _group;
        public DataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        public virtual ChartViewModel Chart
        {
            get { return null; }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class DataGroup : DataCommon
    {
        public DataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private readonly ObservableCollection<DataItem> _items = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> Items
        {
            get { return this._items; }
        }

        private readonly ObservableCollection<DataItem> _topItem = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> TopItems
        {
            get {return this._topItem; }
        }

        internal virtual void UpdateItems<T>(List<T> list)
        {
            var dataList = Factory.GetDataItems(list, this);
            // Note: it would be nice if this was smarter (updating existing entries rather than always wiping it out)
	        CentralDispatcher.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
		        {
			        _items.Clear();

			        foreach (var item in dataList)
			        {
				        var data = item;
				        _items.Add(data);
			        }
		        }
		    );
        }
    }

	public class HistoryDataGroup : DataGroup
	{
		private readonly int _maxEntries;
		private bool _firstUpdate = true;

		public HistoryDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description, int maxEntries)
			: base(uniqueId, title, subtitle, imagePath, description)
		{
			_maxEntries = maxEntries;
		}

		internal override void UpdateItems<T>(List<T> list)
		{
			if (_firstUpdate)
			{
				Items.Clear();
				_firstUpdate = false;
			}
			var dataList = Factory.GetDataItems(list, this);

			// prevent marshalling errors by using the cebtral dispatcher
			CentralDispatcher.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
				{
					for (int t = 0; t < dataList.Count && t < _maxEntries; t++)
					{
						Items.Insert(t, dataList[t]);
						if (Items.Count > _maxEntries)
						{
							Items.RemoveAt(_maxEntries);
						}
					}
				}
			);
		}
	}


    public class RatesGroup : DataGroup
    {
        DispatcherTimer _transTimer = null;
        RatesSession _currentSession = null;

		AccountDataSource _source;

	    public RatesGroup(String uniqueId, String title, String subtitle, String imagePath, String description, AccountDataSource source)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
			_source = source;
			if (Credentials.GetDefaultCredentials().HasServer(EServer.StreamingRates))
			{
				EnableUpdates();
			}
        }

        private void EnableUpdates()
        {
            if (_transTimer == null)
            {
                _transTimer = new DispatcherTimer();
                _transTimer.Tick += Refresh;
                _transTimer.Interval = new TimeSpan(0, 0, 0, 1);
                _transTimer.Start();
            }
        }

        private void Refresh(object sender, object e)
        {
            _transTimer.Stop();
            Refresh();
        }

        private async void Refresh()
        {
            if (_currentSession == null)
            {
                Items.Clear();
                var instruments = await Rest.GetInstrumentsAsync(_source.Id);
				_currentSession = new RatesSession(_source.Id, instruments);
				_currentSession.DataReceived += CurrentSessionOnTickReceived;
				_currentSession.StartSession();
            }
        }

	    private void CurrentSessionOnTickReceived(RateStreamResponse update)
	    {
			var price = new Price()
				{
					ask = update.tick.ask,
					bid = update.tick.bid,
					instrument = update.tick.instrument,
					time = update.tick.time
				};
			var priceItem = Items.Where((item) => item.UniqueId.Equals(price.instrument));
			if (!priceItem.Any())
			{
				Items.Add(Factory.GetViewModel(price, this));
			}
			else
			{
				((PriceViewModel)priceItem.First()).Update(price);
			}
	    }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// DataSource initializes with placeholder data rather than live production
    /// data so that  data is provided at both design-time and run-time.
    /// </summary>
    public sealed class AccountDataSource
    {
	    public static AccountDataSource DefaultDataSource = new AccountDataSource();

        private ObservableCollection<DataGroup> _allGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<DataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

			return DefaultDataSource.AllGroups;
        }

        public static DataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
			var matches = DefaultDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static DataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
			var matches = DefaultDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        DispatcherTimer _transTimer = null;
        long _currentTrans = 0;
	    private EventsSession _currentSession;

	    public void EnableUpdates()
        {
			if (Credentials.GetDefaultCredentials().HasServer(EServer.StreamingEvents))
			{
				_currentSession = new EventsSession(Id);
				_currentSession.DataReceived += CurrentSessionOnDataReceived;
				InternalRefresh();
				_currentSession.StartSession();
			}
			else
			{	// If we're not getting streamed the notifications, we'll have to poll
				if (_transTimer == null)
				{
					_transTimer = new DispatcherTimer();
					_transTimer.Tick += Refresh;
					_transTimer.Interval = new TimeSpan(0, 0, 0, 1);
					_transTimer.Start();
				}
			}
        }

	    private void CurrentSessionOnDataReceived(Event data)
	    {
			InternalRefresh();
	    }

	    private async Task InternalRefresh()
	    {
			if (Credentials.GetDefaultCredentials().HasServer(EServer.Account))
			{
				var transParams = new Dictionary<string, string> { { "minId", "" + (_currentTrans + 1) } };
				var newTransactions = await Rest.GetTransactionListAsync(Id, transParams);
				if (newTransactions.Count > 0)
				{
					_currentTrans = newTransactions[0].id;
					foreach (var newTransaction in newTransactions)
					{
						OnNewTransaction(newTransaction);
					}

					// these can't change unless there's been a transaction...
					GetGroup("Trades").UpdateItems(await Rest.GetTradeListAsync(Id));
					GetGroup("Orders").UpdateItems(await Rest.GetOrderListAsync(Id));
					GetGroup("Activity").UpdateItems(newTransactions);
					GetGroup("Positions").UpdateItems(await Rest.GetPositionsAsync(Id));
				}
			}
	    }

	    private void Refresh(object sender, object e)
        {
            _transTimer.Stop();
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
			await InternalRefresh();

	        _transTimer.Start();
        }

        public int Id { get; private set; }

		public AccountDataSource()
		{
			//InitSampleContent();
		}

	    public AccountDataSource(int accountId)
        {
            Id = accountId;

			InitSampleContent();

			EnableUpdates();
        }

		private void InitSampleContent()
		{
			String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
						"Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

			var group1 = new RatesGroup("Rates",
					"Rates",
					"Live rates and charts",
					"Assets/icon_rates.png",
					"Group Description: Live rates for tradable pairs.", this);
			group1.Items.Add(new DataItem("Group-1-Item-1",
					"Item Title: 1",
					"Item Subtitle: 1",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group1));
			group1.Items.Add(new DataItem("Group-1-Item-2",
					"Item Title: 2",
					"Item Subtitle: 2",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group1));
			group1.Items.Add(new DataItem("Group-1-Item-3",
					"Item Title: 3",
					"Item Subtitle: 3",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group1));
			group1.Items.Add(new DataItem("Group-1-Item-4",
					"Item Title: 4",
					"Item Subtitle: 4",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group1));
			group1.Items.Add(new DataItem("Group-1-Item-5",
					"Item Title: 5",
					"Item Subtitle: 5",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group1));
			this.AllGroups.Add(group1);

			var group2 = new DataGroup("Sample Requests",
					"Sample Requests",
					"Sample post/patch/delete requests",
					"Assets/icon_buysell.png",
					"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
			group2.Items.Add(new OrderPostViewModel("Post Market Order", group2, new Dictionary<string, string>
			{
				{"instrument", "EUR_USD"},
				{"units", "1"},
				{"side", "buy"},
				{"type", "market"},
				{"price", "1.0"}
			}));
			group2.Items.Add(new TradePatchViewModel("Patch Trade", group2, new Dictionary<string, string>
			{
				{"stopLoss", "0.5"},
				{"takeProfit", "6.0"}
			}));
			group2.Items.Add(new TradeDeleteViewModel("Delete Trade", group2));
			group2.Items.Add(new PositionDeleteViewModel("Delete Position", group2, "EUR_USD"));
			// 2013-12-06T20:36:06Z
			var expiry = DateTime.Now.AddMonths(1);
			// XmlConvert.ToDateTime and ToString can be used for going to/from RCF3339
			string expiryString = XmlConvert.ToString(expiry);
			group2.Items.Add(new OrderPostViewModel("Post Pending Order", group2, new Dictionary<string, string>
			{
				{"instrument", "EUR_USD"},
				{"units", "1"},
				{"side", "buy"},
				{"type", "marketIfTouched"},
				{"price", "1.0"},
				{"expiry", expiryString}
			}));
			group2.Items.Add(new OrderPatchViewModel("Patch Pending Order", group2, new Dictionary<string, string>
			{
				{"units", "5"},
				{"side", "sell"},
			}));
			group2.Items.Add(new OrderDeleteViewModel("Delete Pending Order", group2));
			this.AllGroups.Add(group2);
			
			var group4 = new DataGroup("Trades",
					"Trades",
					"Current Open Trades",
					"Assets/icon_trades.png",
					"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
			group4.Items.Add(new DataItem("Group-4-Item-1",
					"Item Title: 1",
					"Item Subtitle: 1",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group4));
			group4.Items.Add(new DataItem("Group-4-Item-2",
					"Item Title: 2",
					"Item Subtitle: 2",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group4));
			group4.Items.Add(new DataItem("Group-4-Item-3",
					"Item Title: 3",
					"Item Subtitle: 3",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group4));
			group4.Items.Add(new DataItem("Group-4-Item-4",
					"Item Title: 4",
					"Item Subtitle: 4",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group4));
			group4.Items.Add(new DataItem("Group-4-Item-5",
					"Item Title: 5",
					"Item Subtitle: 5",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group4));
			group4.Items.Add(new DataItem("Group-4-Item-6",
					"Item Title: 6",
					"Item Subtitle: 6",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group4));
			this.AllGroups.Add(group4);

			var group5 = new DataGroup("Orders",
					"Orders",
					"Current Open Orders",
					"Assets/icon_orders.png",
					"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
			group5.Items.Add(new DataItem("Group-5-Item-1",
					"Item Title: 1",
					"Item Subtitle: 1",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group5));
			group5.Items.Add(new DataItem("Group-5-Item-2",
					"Item Title: 2",
					"Item Subtitle: 2",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group5));
			group5.Items.Add(new DataItem("Group-5-Item-3",
					"Item Title: 3",
					"Item Subtitle: 3",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group5));
			group5.Items.Add(new DataItem("Group-5-Item-4",
					"Item Title: 4",
					"Item Subtitle: 4",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group5));
			this.AllGroups.Add(group5);

			var group6 = new DataGroup("Positions",
					"Positions",
					"Current Open Positions",
					"Assets/icon_positions.png",
					"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
			group6.Items.Add(new DataItem("Group-6-Item-1",
					"Item Title: 1",
					"Item Subtitle: 1",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-2",
					"Item Title: 2",
					"Item Subtitle: 2",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-3",
					"Item Title: 3",
					"Item Subtitle: 3",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-4",
					"Item Title: 4",
					"Item Subtitle: 4",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-5",
					"Item Title: 5",
					"Item Subtitle: 5",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-6",
					"Item Title: 6",
					"Item Subtitle: 6",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-7",
					"Item Title: 7",
					"Item Subtitle: 7",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			group6.Items.Add(new DataItem("Group-6-Item-8",
					"Item Title: 8",
					"Item Subtitle: 8",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group6));
			this.AllGroups.Add(group6);

			var group7 = new HistoryDataGroup("Activity",
					"Activity",
					"Recent Account Activity",
					"Assets/icon_activity.png",
					"Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante",
					100);
			group7.Items.Add(new DataItem("Group-7-Item-1",
					"Item Title: 1",
					"Item Subtitle: 1",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-2",
					"Item Title: 2",
					"Item Subtitle: 2",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-3",
					"Item Title: 3",
					"Item Subtitle: 3",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-4",
					"Item Title: 4",
					"Item Subtitle: 4",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-5",
					"Item Title: 5",
					"Item Subtitle: 5",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-6",
					"Item Title: 6",
					"Item Subtitle: 6",
					"Assets/MediumGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-7",
					"Item Title: 7",
					"Item Subtitle: 7",
					"Assets/DarkGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			group7.Items.Add(new DataItem("Group-7-Item-8",
					"Item Title: 8",
					"Item Subtitle: 8",
					"Assets/LightGray.png",
					"Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
					ITEM_CONTENT,
					group7));
			this.AllGroups.Add(group7);
		}
    }
}
