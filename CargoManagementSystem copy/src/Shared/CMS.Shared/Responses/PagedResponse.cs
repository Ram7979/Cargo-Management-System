namespace CMS.Shared.Responses;

public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedResponse<T> Ok(
        IEnumerable<T> data,
        int page,
        int pageSize,
        int totalCount,
        string message = "Success") =>
        new()
        {
            Success = true,
            Data = data,
            Message = message,
            Errors = null,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
