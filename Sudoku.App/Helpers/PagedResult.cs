namespace Sudoku.App.Helpers;

public class PagedResult<T>(List<T> items, int pageNumber, int pageSize, int totalItems)
{
    public List<T> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalPages { get; } = (int)Math.Ceiling(totalItems / (double)pageSize);
    public int TotalItems { get; } = totalItems;
}