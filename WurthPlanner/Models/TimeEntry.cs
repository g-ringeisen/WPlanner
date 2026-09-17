using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable model for time actually spent by an employee on a work item.
    /// </summary>
    public class TimeEntry : Auditable, ITimeEntry
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkItemId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public Guid? AssignmentId { get; set; }
        public DateOnly WorkDate { get; set; }
        public decimal ActualHours { get; set; }
        public ITextContent? Comment { get; set; }
        public string? Excerpt => Comment?.Excerpt;
    }
}
