using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    public class  OrderItem
    {
        public string Description { get; set; }
        public int OrderHeaderId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int StockItemId { get; set; }

        public int StockGap { get; set; }

        public string Name { get; set; }

        public decimal Total {
            get
            {
                return Price * Quantity;
            }
            set
            {

            }
        }

        public string OrderHeader { get; set; }

    }
}
