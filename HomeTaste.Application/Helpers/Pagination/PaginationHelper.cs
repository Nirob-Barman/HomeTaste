using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Helpers.Pagination
{
    public class PaginationHelper
    {
        public static PaginationMeta GetPaginationMetadata(int pageNumber, int pageSize, int totalCount)
        {
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PaginationMeta
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages,
                IsFirstPage = pageNumber == 1,
                IsLastPage = pageNumber == totalPages
            };
        }
    }

}
