using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Entity.Enums
{
    public enum OrderStatus
    {
        Initiated = 0,
        New=1,
        InProcess=2,
        Completed=3,
        Rejected=4
    }
}