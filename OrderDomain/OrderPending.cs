using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    class OrderPending: OlderState
    {
        public override void Complete()
        {

        }
        public OrderPending()
        {

        }
        public override void Reject()
        {

        }
        public override void Submit()
        {

        }
    }
}
