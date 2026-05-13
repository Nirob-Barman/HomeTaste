
namespace HomeTaste.Application.Wrappers
{
    public class PaginatedResponse<T>
    {
        public PaginationMeta? MetaData { get; set; }
        public T? Data { get; set; }
    }

    public class PaginationMeta
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPageCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
    }
}
