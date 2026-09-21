using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WurthPlanner.Models;
using WurthPlanner.Services;

namespace WurthPlannerWebview.Components.Pages;

public partial class WorkItemEditor : ComponentBase
{
    [Inject] private IWorkItemService WorkItemService { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private List<WorkItem> _notes = new();
    private WorkItem? _currentWorkItem;
    private bool _isDarkMode;

    // Petit délai avant sauvegarde pour éviter d'écrire à chaque frappe.
    // Remplaçable plus tard par un vrai debounce si besoin de fluidité accrue.
    protected override async Task OnInitializedAsync()
    {
        _notes = await WorkItemService.GetAllAsync();
        _currentWorkItem = _notes.FirstOrDefault();
    }

    private async Task OnWorkItemSelected(Guid noteId)
    {
        _currentWorkItem = await WorkItemService.GetAsync(noteId);
    }

    private async Task CreateNewWorkItem()
    {
        var note = new WorkItem
        {
            Title = string.Empty,
            Type = WorkItemType.Task,
            //Metadata = WorkItemMetadataFactory.CreateDefault(WorkItemType.Personal)
        };

        await WorkItemService.SaveAsync(note);
        _notes = await WorkItemService.GetAllAsync();
        _currentWorkItem = note;
    }

    private async Task OnTitleChanged(ChangeEventArgs args)
    {
        if (_currentWorkItem is null) return;

        _currentWorkItem.Title = args.Value?.ToString() ?? string.Empty;
        await PersistCurrentWorkItem();
    }

    private async Task OnContentChanged(string html)
    {
        if (_currentWorkItem is null) return;

        _currentWorkItem.Description?.Text = html;
        await PersistCurrentWorkItem();
    }

    private async Task OnMetadataChanged()
    {
        await PersistCurrentWorkItem();
    }

    private async Task OnWorkItemTypeChanged(WorkItemType newType)
    {
        if (_currentWorkItem is null) return;

        _currentWorkItem.Type = newType;
        //_currentWorkItem.Metadata = WorkItemMetadataFactory.CreateDefault(newType);
        await PersistCurrentWorkItem();
    }

    private async Task PersistCurrentWorkItem()
    {
        if (_currentWorkItem is null) return;

        await WorkItemService.SaveAsync(_currentWorkItem);
        _notes = await WorkItemService.GetAllAsync();
    }

    private async Task ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
        await JS.InvokeVoidAsync("plannerTheme.setTheme", _isDarkMode ? "dark" : "light");
    }

    // Extrait brut et minimal : suffisant pour la liste latérale.
    // Une version future pourra utiliser une vraie librairie de
    // nettoyage HTML si le contenu devient plus complexe.
    private static string BuildExcerpt(string html)
    {
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", " ");
        text = System.Net.WebUtility.HtmlDecode(text).Trim();
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        return text.Length > 80 ? text[..80] + "..." : text;
    }
}
