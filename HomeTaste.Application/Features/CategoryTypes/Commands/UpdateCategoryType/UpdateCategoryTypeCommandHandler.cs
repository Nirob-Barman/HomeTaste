using HomeTaste.Application.Common.Exceptions;
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
                throw new NotFoundException("Category type not found");
            }

            var existingCategoryType = await _context.CategoryTypes
                .Where(ct => ct.Name == categoryTypeRequest.Name && ct.Id != id)
                .Select(ct => new CategoryTypeResponse { Id = ct.Id, Name = ct.Name, Description = ct.Description })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingCategoryType != null)
            {
                throw new ConflictException("Category type with the same name already exists.");
            }

            categoryType.UpdateDetails(categoryTypeRequest.Name, categoryTypeRequest.Description);

            await _context.SaveChangesAsync(cancellationToken);

            var updatedCategoryTypeResponse = new CategoryTypeResponse
            {
                Id = categoryType.Id,
                Name = categoryType.Name,
                Description = categoryType.Description
            };

            return Result<CategoryTypeResponse>.Ok(updatedCategoryTypeResponse, "Category type updated successfully");
        }
    }
}
