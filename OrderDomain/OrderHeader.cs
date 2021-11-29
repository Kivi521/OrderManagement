using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    public class OrderHeader
    {
        public List<OrderItem> _orderItems;
        public DateTime DateTime { get; set;}
        public int Id { get; set; }
        public OrderStates State { get; set; }
        public decimal Total {
            get
            {
                decimal total = 0;
                foreach(OrderItem item in _orderItems)
                {
                    total += item.Total;
                }
                return total;
            }
        }

        public bool IsAllInStock
        {
            get
            {
                foreach (OrderItem item in _orderItems)
                {
                    if (item.StockGap < 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int Count
        {
            get
            {
                return _orderItems.Count;
            }
        }

        public void AddOrderItem(OrderItem item)
        {
            _orderItems.Add(item);
        }

        public OrderHeader()
        {
            _orderItems = new List<OrderItem>();
        }

        public void Complete()
        {
            State = OrderStates.Complete;
        }
  
        public void Reject()
        {
            State = OrderStates.Rejected;
        }
        public void setState(OrderStates state)
        {
            State = state;
        }
        public void Submit(Boolean result)
        {
            if (result)
            {
                State = OrderStates.Complete;
            }
            else
            {
                State = OrderStates.Pending;
            }
            
        }
    }
}
