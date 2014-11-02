using System.Collections.Generic;

namespace OANDARestLibrary.TradeLibrary.DataTypes.Communications
{
    public class OrdersResponse
    {
        public List<Order> orders;
        public string nextPage;
    }
}
