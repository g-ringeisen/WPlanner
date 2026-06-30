using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WurthPlanner.Models;
using WurthPlanner.Services;
using WurthPlannerWebview.Utils;

namespace WurthPlannerWebview.Components.Shared;

public partial class PlanningGantt : ComponentBase, IAsyncDisposable
{
    [Parameter, EditorRequired] public List<WorkItem> Assignments { get; set; } = new();
    [Parameter] public DateOnly RangeStart { get; set; }
    [Parameter] public int WeekCount { get; set; } = 4;
    [Parameter] public EventCallback<WorkItem> AssignmentChanged { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = null!;

    private const int DayWidthPx = 36;
    private const int RowHeightPx = 32;
    private const int LaneHeaderWidthPx = 140;

    private DotNetObjectReference<PlanningGantt>? _selfRef;
    private IJSObjectReference? _module;

    private DateOnly RangeEnd => RangeStart.AddDays(WeekCount * 7);
    private int TotalDays => WeekCount * 7;

    private List<string> _laneNames = new();
    private Dictionary<string, List<PlanningLayout.PlacedBar>> _lanesBars = new();
    private Dictionary<string, int> _laneRowCounts = new();

    protected override void OnParametersSet()
    {
        BuildLanes();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _selfRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("plannerGantt.init", _selfRef, DayWidthPx, RowHeightPx);
        }
        else
        {
            await JS.InvokeVoidAsync("plannerGantt.updateMetrics", DayWidthPx, RowHeightPx);
        }
    }

    private void BuildLanes()
    {
        _laneNames = Assignments
            .Select(a => string.IsNullOrWhiteSpace(a.Assignee) ? "Non assigné" : a.Assignee!)
            .Distinct()
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _lanesBars = new();
        _laneRowCounts = new();

        foreach (var lane in _laneNames)
        {
            var items = Assignments.Where(a =>
                (string.IsNullOrWhiteSpace(a.Assignee) ? "Non assigné" : a.Assignee!) == lane);

            var placed = PlanningLayout.PackBars(items, RangeStart);
            _lanesBars[lane] = placed;
            _laneRowCounts[lane] = placed.Count == 0 ? 1 : placed.Max(p => p.Row) + 1;
        }
    }

    private IEnumerable<DateOnly> AllDays()
    {
        for (int i = 0; i < TotalDays; i++)
            yield return RangeStart.AddDays(i);
    }

    private int LaneIndex(string lane) => _laneNames.IndexOf(lane);

    private async Task OnBarPointerDown(Microsoft.AspNetCore.Components.Web.PointerEventArgs e, Guid workItemId, string lane)
    {
        await JS.InvokeVoidAsync("plannerGantt.startMove", e.ClientX, e.ClientY, workItemId.ToString(), LaneIndex(lane));
    }

    private async Task OnResizePointerDown(Microsoft.AspNetCore.Components.Web.PointerEventArgs e, Guid workItemId, string lane)
    {
        await JS.InvokeVoidAsync("plannerGantt.startResize", e.ClientX, e.ClientY, workItemId.ToString(), LaneIndex(lane));
    }

    private IEnumerable<(string Label, int DayCount)> MonthHeaders()
    {
        var days = AllDays().ToList();
        var groups = days
            .Select((d, idx) => (d, idx))
            .GroupBy(x => new { x.d.Year, x.d.Month });

        foreach (var g in groups)
        {
            var label = System.Globalization.CultureInfo.GetCultureInfo("fr-FR")
                .DateTimeFormat.GetMonthName(g.Key.Month);
            label = char.ToUpper(label[0]) + label[1..];
            yield return (label, g.Count());
        }
    }

    [JSInvokable]
    public async Task OnBarMoved(string workItemIdStr, int dayDelta, int newLaneIndex)
    {
        if (!Guid.TryParse(workItemIdStr, out var workItemId)) return;

        var item = Assignments.FirstOrDefault(a => a.Id == workItemId);
        if (item is null || item.StartDate is null || item.EndDate is null) return;

        item.StartDate = item.StartDate.Value.AddDays(dayDelta);
        item.EndDate = item.EndDate.Value.AddDays(dayDelta);

        if (newLaneIndex >= 0 && newLaneIndex < _laneNames.Count)
        {
            var newAssignee = _laneNames[newLaneIndex];
            item.Assignee = newAssignee == "Non assigné" ? null : newAssignee;
        }

        await AssignmentChanged.InvokeAsync(item);
    }

    [JSInvokable]
    public async Task OnBarResized(string workItemIdStr, int durationDayDelta)
    {
        if (!Guid.TryParse(workItemIdStr, out var workItemId)) return;

        var item = Assignments.FirstOrDefault(a => a.Id == workItemId);
        if (item is null || item.EndDate is null) return;

        var newEnd = item.EndDate.Value.AddDays(durationDayDelta);
        // On garde au moins 1 jour de durée.
        if (item.StartDate.HasValue && newEnd <= item.StartDate.Value)
            newEnd = item.StartDate.Value.AddDays(1);

        item.EndDate = newEnd;

        await AssignmentChanged.InvokeAsync(item);
    }

    public async ValueTask DisposeAsync()
    {
        _selfRef?.Dispose();
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
