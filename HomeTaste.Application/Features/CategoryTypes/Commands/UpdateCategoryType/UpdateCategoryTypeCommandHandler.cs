using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.UpdateCategoryType
{
    public class UpdateCategoryTypeCommandHandler : IRequestHandler<UpdateCategoryTypeCommand, Result<CategoryTypeResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCategoryTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CategoryTypeResponse>> Handle(UpdateCategoryTypeCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var categoryTypeRequest = request.CategoryTypeRequest;

            var categoryType = await _context.CategoryTypes.FindAsync(new object?[] { id }, cancellationToken);

            if (categoryType == null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type not found", "Category type not found", ResultType.NotFound);
            }

            var existingCategoryType = await _context.CategoryTypes
                .Where(ct => ct.Name == categoryTypeRequest.Name && ct.Id != id)
                .Select(ct => new CategoryTypeResponse { Id = ct.Id, Name = ct.Name, Description = ct.Description })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingCategoryType != null)
            {
                return Result<CategoryTypeResponse>.Fail("Category type with the same name already exists.", "Duplicate category type", ResultType.Conflict);
            }

            categoryType.UpdateDetails(categoryTypeRequest.Name, categoryTypeRequest.Description);

            await _context.SaveChangesAsync(cancellationToken);

            var updatedCategoryTypeResponse = new CategoryTypeResponse
            {
                Id = categoryType.Id,
                Name = categoryType.Name,
                Description = categoryType.Description
            };

            return Result<CategoryTypeResponse>.Ok(updatedCategoryTypeResponse, "Category type updated successfully", ResultType.Success);
        }
    }
}
