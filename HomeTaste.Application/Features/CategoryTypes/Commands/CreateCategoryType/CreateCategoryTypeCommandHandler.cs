using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Support;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.CreateCategoryType
{
    public class CreateCategoryTypeCommandHandler : IRequestHandler<CreateCategoryTypeCommand, Result<CategoryTypeResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateCategoryTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CategoryTypeResponse>> Handle(CreateCategoryTypeCommand request, CancellationToken cancellationToken)
        {
            var categoryTypeRequest = request.CategoryTypeRequest;

            var existingCategoryType = await _context.CategoryTypes
                .Where(ct => ct.Name == categoryTypeRequest.Name)
                .Select(ct => new CategoryTypeResponse { Id = ct.Id, Name = ct.Name, Description = ct.Description })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingCategoryType != null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type with the same name already exists.", "Duplicate category type", ResultType.Conflict);
            }

            var categoryType = CategoryType.Create(categoryTypeRequest.Name, categoryTypeRequest.Description);

            _context.CategoryTypes.Add(categoryType);
            await _context.SaveChangesAsync(cancellationToken);

            var categoryTypeResponse = new CategoryTypeResponse
            {
                Id = categoryType.Id,
                Name = categoryType.Name,
                Description = categoryType.Description
            };

            return Result<CategoryTypeResponse>.Ok(categoryTypeResponse, "Category type created successfully", ResultType.Success);
        }
    }
}
