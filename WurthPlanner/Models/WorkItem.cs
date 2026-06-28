namespace WurthPlanner.Models;

public class WorkItem
{
    private DateTime? _startdate;
    private DateTime? _enddate;
    private TimeSpan? _duration;

    public Guid Id { get; private set; } = System.Guid.NewGuid();
    public required string Title { get; set; }

    public WorkItemStatus Status { get; set; } = WorkItemStatus.Unknown;
    public WorkItemType Type { get; set; } = WorkItemType.Unknown;

    public WorkItemReference? Parent { get; set; }
    public ExternalReference? Source { get; set; }

    public WorkItemDescription? Description { get; set; }
    public string? Excerpt { get => Description?.Excerpt; }

    public IEnumerable<string> Tags { get => []; }

    public string? Assignee { get; set; }

    public DateOnly? DueDate { get; set; }
    public DateTime? StartDate { 
        get => _startdate;
        set {
            if(value is null && _startdate.HasValue)
                _duration = null;
            else if (value is not null && _duration.HasValue)
                _enddate = value.Value + _duration.Value;
            else if (value is not null && _enddate.HasValue)
                _duration = _enddate.Value - value.Value;
            _startdate = value;
        }
    }
    public DateTime? EndDate { 
        get => _enddate; 
        set {
            if(value is null && _enddate.HasValue)
                _duration = null;
            else if (value is not null && _startdate.HasValue)
                _duration = value.Value - _startdate.Value;
            else if (value is not null && _duration.HasValue)
                _startdate = value.Value - _duration.Value;
            _enddate = value;
        }
    }
    public TimeSpan? Duration {
        get => _duration;
        set {
            if (value is null && _duration.HasValue)
                _enddate = null;
            else if (value is not null && _startdate.HasValue)
                _enddate = _startdate.Value + value.Value;
            else if (value is not null && _enddate.HasValue)
                _startdate = _enddate.Value - value.Value;
            _duration = value;
        }
    }
    public float? Workload { get; set; }

    public string CreatedBy { get; set; } = Environment.UserName;
    public string UpdatedBy { get; set; } = Environment.UserName;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
