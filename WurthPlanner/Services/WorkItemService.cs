using System.Linq.Expressions;
using WurthPlanner.Models;

namespace WurthPlanner.Services
{
    public class WorkItemService(IWorkItemRepository _workItemRepository)
    {

        public async Task<WorkItem?> GetAsync(Guid guid)
        {
            return await _workItemRepository.GetByIdAsync(guid);
        }

        public async Task<WorkItem> SaveAsync(WorkItem workItem)
        {
            workItem = Validate(workItem);
            return await _workItemRepository.SaveAsync(workItem);
        }

        public async Task DeleteAsync(Guid guid)
        {
            await _workItemRepository.DeleteAsync(guid);
        }

        public async Task<WorkItemSearchResult> SearchAsync(WorkItemSearchFilter filter)
        {
            IQueryable<WorkItem> result = _workItemRepository.AsQueryable();

            if(filter.Type.HasValue)
                result = result.Where(wi => wi.Type == filter.Type);

            // TODO:

            int totalCount = result.Count();
            result = result.Skip(filter.Offset).Take(filter.Limit);

            return new WorkItemSearchResult
            {
                TotalCount = totalCount,
                Offset = filter.Offset,
                WorkItems = [.. result]
            };
        }

        public async Task<List<WorkItem>> GetAllAsync()
        {
            return await Task.FromResult(_workItemRepository.AsQueryable().ToList());
        }

        /// <summary>
        /// Récupère les Assignments dont la période (StartDate/EndDate) intersecte
        /// la plage [from, to]. Un Assignment sans dates est ignoré du planning.
        /// </summary>
        public async Task<List<WorkItem>> GetAssignmentsAsync(DateTime from, DateTime to)
        {
            var result = _workItemRepository.AsQueryable()
                .Where(wi => wi.Type == WorkItemType.Assignment)
                .Where(wi => wi.StartDate.HasValue && wi.EndDate.HasValue)
                .Where(wi => wi.StartDate.Value < to && wi.EndDate.Value > from)
                .ToList();

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Liste des Task / Event (réunions) actifs, triés par date d'échéance
        /// (DueDate pour les Task) puis par date de début (StartDate pour les Event)
        /// croissante. Utilisé pour le panneau latéral de la page Planning.
        /// </summary>
        public async Task<List<WorkItem>> GetActiveTasksAndMeetingsAsync()
        {
            var activeStatuses = new[] { WorkItemStatus.Open, WorkItemStatus.InProgress };

            var result = _workItemRepository.AsQueryable()
                .Where(wi => wi.Type == WorkItemType.Task || wi.Type == WorkItemType.Event)
                .Where(wi => activeStatuses.Contains(wi.Status))
                .ToList()
                .OrderBy(wi => GetSortDate(wi) ?? DateTime.MaxValue)
                .ToList();

            return await Task.FromResult(result);
        }

        private static DateTime? GetSortDate(WorkItem workItem)
        {
            if (workItem.Type == WorkItemType.Task)
                return workItem.DueDate?.ToDateTime(TimeOnly.MinValue);

            return workItem.StartDate;
        }

        private WorkItem Validate(WorkItem workItem)
        {
            return workItem.Type.ValidateWorkItem(workItem);
        }
    }
}
