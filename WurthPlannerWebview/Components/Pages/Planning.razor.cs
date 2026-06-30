using Microsoft.AspNetCore.Components;
using WurthPlanner.Models;
using WurthPlanner.Services;
using WurthPlannerWebview.Utils;

namespace WurthPlannerWebview.Components.Pages;

public partial class Planning : ComponentBase
{
    [Inject] private WorkItemService WorkItemService { get; set; } = null!;

    private static readonly int[] ZoomOptions = { 2, 4, 6, 8, 12 };

    private DateOnly _rangeStart;
    private int _weekCount = 4;
    private List<WorkItem> _assignments = new();
    private List<WorkItem> _sidebarItems = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        // Par défaut : 4 semaines, la première étant la semaine passée.
        var thisWeekStart = PlanningLayout.GetWeekStart(DateOnly.FromDateTime(DateTime.Today));
        _rangeStart = thisWeekStart.AddDays(-7);

        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _isLoading = true;

        var rangeEnd = _rangeStart.AddDays(_weekCount * 7);
        var fromDateTime = _rangeStart.ToDateTime(TimeOnly.MinValue);
        var toDateTime = rangeEnd.ToDateTime(TimeOnly.MinValue);

        _assignments = await WorkItemService.GetAssignmentsAsync(fromDateTime, toDateTime);
        _sidebarItems = await WorkItemService.GetActiveTasksAndMeetingsAsync();

        _isLoading = false;
    }

    private async Task ShiftRange(int weeks)
    {
        _rangeStart = _rangeStart.AddDays(weeks * 7);
        await LoadAsync();
    }

    private async Task GoToToday()
    {
        var thisWeekStart = PlanningLayout.GetWeekStart(DateOnly.FromDateTime(DateTime.Today));
        _rangeStart = thisWeekStart.AddDays(-7);
        await LoadAsync();
    }

    private async Task OnWeekCountChanged(ChangeEventArgs args)
    {
        if (int.TryParse(args.Value?.ToString(), out var weeks))
        {
            _weekCount = weeks;
            await LoadAsync();
        }
    }

    private async Task OnAssignmentChanged(WorkItem item)
    {
        await WorkItemService.SaveAsync(item);
        await LoadAsync();
    }

    private string FormatRangeLabel()
    {
        var rangeEnd = _rangeStart.AddDays(_weekCount * 7 - 1);
        return $"{_rangeStart:dd MMM yyyy} — {rangeEnd:dd MMM yyyy}";
    }
}
