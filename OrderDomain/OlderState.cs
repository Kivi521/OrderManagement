using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    class  OlderState
    {
        public string _olderHeader;
        public string state { get; set; }
        public virtual void Complete()
        {

        }
        public static void OrderState()
        {

        }
        public virtual void Reject()
        {

        }
        public virtual void Submit()
        {

        }
    }
}
