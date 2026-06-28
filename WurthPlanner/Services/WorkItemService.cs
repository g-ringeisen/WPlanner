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

        private WorkItem Validate(WorkItem workItem)
        {
            return workItem.Type.ValidateWorkItem(workItem);
        }
    }
}
