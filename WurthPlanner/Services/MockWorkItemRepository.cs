using WurthPlanner.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WurthPlanner.Services;

// Implémentation en mémoire utilisée pour le développement de l'UI.
// À remplacer par une implémentation EF Core (ex: EfIWorkItemRepository)
// une fois la base de données branchée — le contrat IIWorkItemRepository
// ne change pas, donc aucun composant n'aura besoin d'être modifié.
public class MockWorkItemRepository : IWorkItemRepository
{
    private readonly List<WorkItem> _workitems;

    public MockWorkItemRepository()
    {
        _workitems = SeedIWorkItems();
    }

    public Task<List<WorkItem>> GetAllAsync()
    {
        var ordered = _workitems.OrderByDescending(n => n.UpdatedAt).ToList();
        return Task.FromResult(ordered);
    }

    public Task<WorkItem?> GetByIdAsync(Guid id)
    {
        var iworkitem = _workitems.FirstOrDefault(n => n.Id == id);
        return Task.FromResult(iworkitem);
    }

    public Task<WorkItem> SaveAsync(WorkItem iworkitem)
    {
        //iworkitem.UpdatedAt = DateTime.Now;
        var existingIndex = _workitems.FindIndex(n => n.Id == iworkitem.Id);

        if (existingIndex >= 0)
        {
            _workitems[existingIndex] = iworkitem;
        }
        else
        {
            _workitems.Add(iworkitem);
        }

        return Task.FromResult(iworkitem);
    }

    public Task DeleteAsync(Guid id)
    {
        _workitems.RemoveAll(n => n.Id == id);
        return Task.CompletedTask;
    }

    public IQueryable<WorkItem> AsQueryable()
    {
        return _workitems.AsQueryable();
    }

    private static List<WorkItem> SeedIWorkItems()
    {
        var now = DateTime.Now;

        return new List<WorkItem>
        {
            new WorkItem
            {
                Title = "Refonte espace client",
                Type = WorkItemType.Phase,
                Description = new WorkItemDescription
                {
                    MimeType = "text/html",
                    Text = "<p>Objectifs principaux du projet de refonte :</p><ul><li>Audit complet du parcours actuel</li><li>Maquettes des nouveaux écrans</li><li>Validation avec l'équipe support</li></ul>",
                },
                DueDate = DateOnly.FromDateTime(now.AddDays(23)),
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddMinutes(-10)
            },
            new WorkItem
            {
                Title = "Point hebdo équipe",
                Type = WorkItemType.Event,
                Description = new WorkItemDescription
                {
                    MimeType = "text/html",
                    Text = "<p>Présents : Marc, Lina, Yanis.</p><p>Sujets abordés : avancement sprint, blocages techniques.</p>",
                },
                StartDate = now.AddDays(-1),
                EndDate  = now.AddDays(-1).AddHours(+1),
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1).AddHours(2)
            },
            new WorkItem
            {
                Title = "Sprint 12 - rétro",
                Type = WorkItemType.Phase,
                Description = new WorkItemDescription
                {
                    MimeType = "text/html",
                    Text = "<p>Vélocité en hausse de 15% par rapport au sprint précédent.</p>",
                },
                StartDate = now.AddDays(-14),
                EndDate = now,
                Workload = 34,
                CreatedAt = now.AddDays(-14),
                UpdatedAt = now.AddHours(-3)
            },
            new WorkItem
            {
                Title = "Préparer la démo client",
                Type = WorkItemType.Task,
                Description = new WorkItemDescription
                {
                    MimeType = "text/html",
                    Text = "<p>À faire avant vendredi : vérifier l'environnement de démo et préparer le script.</p>",
                },
                CreatedAt = now.AddDays(-2),
                UpdatedAt = now.AddHours(-6)
            },
            new WorkItem
            {
                Title = "Idée week-end",
                Type = WorkItemType.Note,
                Description = new WorkItemDescription
                {
                    MimeType = "text/html",
                    Text = "<p>Visiter l'expo au musée, réserver les billets en ligne.</p>",
                },
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-3)
            }
        };

    }
}
