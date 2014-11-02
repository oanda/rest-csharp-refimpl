using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes
{
	public class Signal
	{
		public Metadata meta;
		public int id;
		public string instrument;
		public string type;
		public Data data;
		public Prediction prediction;

		public class Metadata
		{
			public int completed;
			public Scores scores;
			public string patterntype;
			public double probability;
			public int interval;
			public int direction;
			public string pattern;
			public int length;
			public HistoricalStats historicalstats;
			public string trendtype;
		}

		public class Scores
		{
			public int uniformity;
			public int quality;
			public int breakout;
			public int initialtrend;
			public int clarity;
		}

		public class Data
		{
			public double price;
			public long patternendtime;
			public Points points;
		}

		public class Points
		{
			public Point resistance;
			public Point support;
			// Note: this doesn't appear to work
			public Dictionary<int, long> keytime;
		}

		public class Point
		{
			public long x0;
			public long x1;
			public double y0;
			public double y1;
		}

		public class Prediction
		{
			public long timeto;
			public long timefrom;
			public int timebars;
			public double pricehigh;
			public double pricelow;
		}
	}

	
	

	public class HistoricalStats
	{
		public HistoricalStat hourofday;
		public HistoricalStat pattern;
		public HistoricalStat symbol;
	}

	public class HistoricalStat
	{
		public int total;
		public double percent;
		public int correct;
	}
}
