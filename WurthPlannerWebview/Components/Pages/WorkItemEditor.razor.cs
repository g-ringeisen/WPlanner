using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WurthPlanner.Contracts.Content;
using WurthPlanner.Contracts.WorkItems;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlannerWebview.Components.Pages;

public partial class WorkItemEditor : ComponentBase
{
    [Inject] private IWorkItemService WorkItemService { get; set; } = null!;

    private readonly List<IWorkItemBrief> _workItems = [];
    private readonly WorkItemType[] _availableTypes = Enum.GetValues<WorkItemType>()
        .Where(type => type != WorkItemType.Unknown).ToArray();
    private readonly WorkItemStatus[] _availableStatuses = Enum.GetValues<WorkItemStatus>()
        .Where(status => status != WorkItemStatus.Unknown).ToArray();

    private IWorkItem? _currentWorkItem;
    private WorkItemEditorModel _editor = new();
    private string _searchText = string.Empty;
    private string? _error;
    private bool _isLoading;
    private bool _isSaving;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        _isLoading = true;
        _error = null;
        try
        {
            var result = await WorkItemService.SearchAsync(new WorkItemSearchRequest
            {
                SearchText = string.IsNullOrWhiteSpace(_searchText) ? null : _searchText.Trim(),
                PageNumber = 1,
                PageSize = 100
            });
            _workItems.Clear();
            _workItems.AddRange(result.Items);
        }
        catch (Exception exception) { _error = exception.Message; }
        finally { _isLoading = false; }
    }

    private async Task SearchOnEnterAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Enter") await LoadAsync();
    }

    private async Task SelectAsync(Guid id)
    {
        _error = null;
        _currentWorkItem = await WorkItemService.GetByIdAsync(id);
        if (_currentWorkItem is null)
        {
            _error = "Cet élément de travail n'existe plus.";
            return;
        }
        _editor = WorkItemEditorModel.From(_currentWorkItem);
    }

    private async Task CreateAsync()
    {
        _isSaving = true;
        _error = null;
        try
        {
            _currentWorkItem = await WorkItemService.CreateAsync(new CreateWorkItemRequest(
                "Nouvel élément de travail", WorkItemType.Task, new TextContentInput(string.Empty)));
            _editor = WorkItemEditorModel.From(_currentWorkItem);
            await LoadAsync();
        }
        catch (Exception exception) { _error = exception.Message; }
        finally { _isSaving = false; }
    }

    private async Task SaveAsync()
    {
        if (_currentWorkItem is null || string.IsNullOrWhiteSpace(_editor.Title)) return;

        _isSaving = true;
        _error = null;
        try
        {
            var source = _currentWorkItem.Source is null
                ? null
                : new ExternalReferenceInput(_currentWorkItem.Source.System, _currentWorkItem.Source.Id,
                    _currentWorkItem.Source.Title, _currentWorkItem.Source.Url);
            _currentWorkItem = await WorkItemService.UpdateAsync(_currentWorkItem.Id, new UpdateWorkItemRequest(
                _editor.Title.Trim(), _editor.Status, _editor.Type,
                new TextContentInput(_editor.Description ?? string.Empty), _currentWorkItem.Tags,
                _currentWorkItem.DueDate, _currentWorkItem.EffortDays, _currentWorkItem.ParentId, source));
            _editor = WorkItemEditorModel.From(_currentWorkItem);
            await LoadAsync();
        }
        catch (Exception exception) { _error = exception.Message; }
        finally { _isSaving = false; }
    }

    private sealed class WorkItemEditorModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkItemType Type { get; set; } = WorkItemType.Task;
        public WorkItemStatus Status { get; set; } = WorkItemStatus.Draft;
        public static WorkItemEditorModel From(IWorkItem item) => new()
        {
            Title = item.Title,
            Description = item.Description.Content,
            Type = item.Type,
            Status = item.Status
        };
    }
}
