using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.CategoryTypes.Queries.GetCategoryTypeById
{
    public class GetCategoryTypeByIdQueryHandler : IRequestHandler<GetCategoryTypeByIdQuery, Result<CategoryTypeResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetCategoryTypeByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CategoryTypeResponse>> Handle(GetCategoryTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var categoryType = await _context.CategoryTypes
                .Where(ct => ct.Id == request.Id)
                .Select(ct => new CategoryTypeResponse
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (categoryType == null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type not found", "Category type not found", ResultType.NotFound);
            }

            return Result<CategoryTypeResponse>.Ok(categoryType, "Category type retrieved successfully", ResultType.Success);
        }
    }
}
