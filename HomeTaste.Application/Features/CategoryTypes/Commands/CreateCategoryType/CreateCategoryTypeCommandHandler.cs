using HomeTaste.Application.Common.Exceptions;
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
            var existingCategoryType = await _context.CategoryTypes
                .Where(ct => ct.Name == request.Name)
                .Select(ct => new CategoryTypeResponse { Id = ct.Id, Name = ct.Name, Description = ct.Description })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingCategoryType != null)
            {
                throw new ConflictException("Category type with the same name already exists.");
            }

            var categoryType = CategoryType.Create(request.Name, request.Description);

            _context.CategoryTypes.Add(categoryType);
            await _context.SaveChangesAsync(cancellationToken);

            var categoryTypeResponse = new CategoryTypeResponse
            {
                Id = categoryType.Id,
                Name = categoryType.Name,
                Description = categoryType.Description
            };

            return Result<CategoryTypeResponse>.Ok(categoryTypeResponse, "Category type created successfully");
        }
    }
}
