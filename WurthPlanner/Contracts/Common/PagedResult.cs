using System.Collections.Generic;

namespace WurthPlanner.Contracts.Common;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)System.Math.Ceiling((decimal)TotalCount / PageSize);
}
