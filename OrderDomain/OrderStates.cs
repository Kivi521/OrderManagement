using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDomain
{
    public enum OrderStates
    {
        New=1,
        Pending=2,
        Rejected=3,
        Complete=4
    }
}
