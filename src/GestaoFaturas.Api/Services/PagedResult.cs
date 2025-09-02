namespace GestaoFaturas.Api.Services;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public static PagedResult<T> Create(IEnumerable<T> allItems, int pageNumber, int pageSize)
    {
        var totalItems = allItems.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = allItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return new PagedResult<T>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}