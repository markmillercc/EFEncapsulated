using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFEncapsulated.Models
{
    public class Order
    {
        public int Id { get; private set; }
        private ICollection<OrderLineItem> _LineItems;
        public IEnumerable<OrderLineItem> LineItems { get { return _LineItems; } }

        public void AddLineItem(string description, double price)
        {
            var lineItem = new OrderLineItem(description, price);
            _LineItems.Add(lineItem);
        }
    }
}