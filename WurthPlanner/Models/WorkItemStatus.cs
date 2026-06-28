using System;
using System.Collections.Generic;
using System.Text;

namespace WurthPlanner.Models
{
    public enum WorkItemStatus
    {
        Unknown = 0,
        Draft = 1,
        Open = 2,
        InProgress = 3,
        Closed = 4,
        Cancelled = 5,
    }
}
