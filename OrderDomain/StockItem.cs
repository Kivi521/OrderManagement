using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    public class StockItem
    {
        public int Id { get; set; }
        public int InStock { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
