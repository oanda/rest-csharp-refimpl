using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Position
    {
        public string side { get; set; }
        public string instrument { get; set; }
        public int units { get; set; }
        public double avgPrice { get; set; }
    }
}
