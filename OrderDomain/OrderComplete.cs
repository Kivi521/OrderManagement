using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    class OrderComplete: OlderState
    {
        public string State { get; set; }
        public override void Complete()
        {

        }
        public OrderComplete()
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
