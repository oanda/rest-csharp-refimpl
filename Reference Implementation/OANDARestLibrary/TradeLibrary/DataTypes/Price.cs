using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Price
    {
        public enum State
        {
            Default,
            Increasing,
            Decreasing
        };

        public string instrument { get; set; }
        public string time;
        public double bid { get; set; }
        public double ask { get; set; }
	    public string status;
        public State state = State.Default;

	    public void update( Price update )
        {
            if ( this.bid > update.bid )
            {
                state = State.Decreasing;
            }
            else if ( this.bid < update.bid )
            {
                state = State.Increasing;
            }
            else
            {
                state = State.Default;
            }

            this.bid = update.bid;
            this.ask = update.ask;
            this.time = update.time;
        }
    }
}
