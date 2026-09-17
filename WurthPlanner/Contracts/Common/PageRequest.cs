namespace WurthPlanner.Contracts.Common;

public abstract record PageRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
