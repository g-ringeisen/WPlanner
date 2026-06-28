using WurthPlanner.Models;

namespace WurthPlanner.Services;

public interface IWorkItemRepository
{
    public Task<WorkItem?> GetByIdAsync(Guid id);
    public Task<WorkItem> SaveAsync(WorkItem workItem);
    public Task DeleteAsync(Guid id);
    public IQueryable<WorkItem> AsQueryable();
}
