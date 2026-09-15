namespace TaskManagementAPI.DTOs.Common;

public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _PageSize = 10;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _PageSize;
        set => _PageSize = value>MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
    }
}

public class PagedResult<T>(IEnumerable<T> items, int totalcount, int pageNumber, int pageSize)
{
    public IEnumerable<T> Items { get; } = items;
    public int TotalCount { get; } = totalcount;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalPages=> (int)Math.Ceiling(TotalCount/(double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

}
