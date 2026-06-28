using System;
using System.Collections.Generic;
using System.Text;
using WurthPlanner.Models;

namespace WurthPlanner.Services
{
    public class WorkItemSearchFilter
    {
        public string? SearchTerm { get; set; }
        public WorkItemType? Type { get; set; }
        public string? Assignee { get; set; }
        public List<string>? Tags { get; set; }
    
        
        public int Limit { get; set; } = 100;
        public int Offset { get; set; } = 0;
    }
}
