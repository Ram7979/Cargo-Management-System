namespace CMS.Shared.Pagination;

public static class PaginationHelper
{
    /// <summary>
    /// Applies pagination to an IQueryable source.
    /// </summary>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return source.Skip(skip).Take(pageSize);
    }

    /// <summary>
    /// Applies pagination using a PagedRequest.
    /// </summary>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> source, PagedRequest request) =>
        source.Paginate(request.Page, request.PageSize);
}
