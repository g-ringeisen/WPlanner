using System;
using System.Collections.Generic;
using System.Text;
using WurthPlanner.Models;

namespace WurthPlanner.Services
{
    public class WorkItemSearchResult
    {
        public int TotalCount { get; set; }
        public int Offset { get; set; }
        public int Count { get => WorkItems.Count(); }
        public IEnumerable<WorkItem> WorkItems { get; set; } = [];
    }
}
