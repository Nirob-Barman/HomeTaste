using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Ingredients.Queries.GetAllIngredients
{
    public class GetAllIngredientsQueryHandler : IRequestHandler<GetAllIngredientsQuery, Result<PaginatedResponse<IEnumerable<IngredientResponse>>>>
    {
        private static readonly List<string> ValidSortColumns = new() { "Id", "Name", "CreatedAt" };

        private readonly IApplicationDbContext _context;

        public GetAllIngredientsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<IngredientResponse>>>> Handle(GetAllIngredientsQuery request, CancellationToken cancellationToken)
        {
            if (!ValidSortColumns.Contains(request.SortBy))
            {
                throw new BadRequestException("Invalid sort column.");
            }

            var query = _context.Ingredients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(ingredient =>
                    ingredient.Name!.Contains(request.SearchTerm) ||
                    ingredient.Description!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var descending = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            query = request.SortBy switch
            {
                "Name" => descending ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name),
                "CreatedAt" => descending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt),
                _ => descending ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id),
            };

            var ingredients = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ingredient => new IngredientResponse
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Description = ingredient.Description,
                    IsAllergen = ingredient.IsAllergen,
                    ImageUrl = ingredient.ImageUrl
                })
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = ingredients.Count;

            var response = new PaginatedResponse<IEnumerable<IngredientResponse>>
            {
                MetaData = paginationMeta,
                Data = ingredients
            };

            return Result<PaginatedResponse<IEnumerable<IngredientResponse>>>.Ok(response, "Ingredients retrieved successfully");
        }
    }
}
