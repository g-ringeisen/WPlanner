using System;

namespace WurthPlanner.Models
{
    /// <summary>
    /// Instantiable planned allocation model for a work item.
    /// </summary>
    public class Assignment : Auditable, IAssignment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkItemId { get; set; }
        public string? AssigneeId { get; set; }
        public ITextContent Instruction { get; set; } = new TextContent();
        public string? Excerpt => Instruction.Excerpt;
        public decimal? PlannedDays { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
