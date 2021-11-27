using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OrderDomain
{
    class OrderNew: OlderState
    {
       public string State { get; set; }
        public override void Complete()
        {

        }
        public  OrderNew()
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