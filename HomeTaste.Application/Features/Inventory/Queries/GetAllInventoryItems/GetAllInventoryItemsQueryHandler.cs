using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Inventory.Queries.GetAllInventoryItems
{
    public class GetAllInventoryItemsQueryHandler : IRequestHandler<GetAllInventoryItemsQuery, Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllInventoryItemsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>> Handle(GetAllInventoryItemsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.InventoryItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(item => item.Name!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(item => new InventoryItemResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    StockCount = item.StockCount,
                    Price = item.Price
                })
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = items.Count;

            var response = new PaginatedResponse<IEnumerable<InventoryItemResponse>>
            {
                Data = items,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>.Ok(response, "Inventory retrieved successfully");
        }
    }
}
