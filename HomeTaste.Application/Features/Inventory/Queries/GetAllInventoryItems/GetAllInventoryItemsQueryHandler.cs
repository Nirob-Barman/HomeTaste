using HomeTaste.Application.Common.Exceptions;
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
            var items = await _context.InventoryItems
                .Select(item => new InventoryItemResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    StockCount = item.StockCount,
                    Price = item.Price
                })
                .ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                items = items.Where(item =>
                    item.Name!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = items.Count();

            var pagedItems = items.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            paginationMeta.CurrentPageCount = pagedItems.Count();

            var response = new PaginatedResponse<IEnumerable<InventoryItemResponse>>
            {
                Data = pagedItems,
                MetaData = paginationMeta
            };

            if (!pagedItems.Any())
            {
                throw new NotFoundException("No inventory items found");
            }

            return Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>.Ok(response, "Inventory retrieved successfully");
        }
    }
}
