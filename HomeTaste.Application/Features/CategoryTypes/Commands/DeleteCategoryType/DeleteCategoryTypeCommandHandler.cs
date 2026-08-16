using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Commands.DeleteCategoryType
{
    public class DeleteCategoryTypeCommandHandler : IRequestHandler<DeleteCategoryTypeCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCategoryTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryTypeCommand request, CancellationToken cancellationToken)
        {
            var categoryType = await _context.CategoryTypes.FindAsync(new object?[] { request.Id }, cancellationToken);

            if (categoryType == null)
            {
                return Result<bool>.Fail("Category type not found", "Category type not found", ResultType.NotFound);
            }

            _context.CategoryTypes.Remove(categoryType);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Category type deleted successfully", ResultType.Success);
        }
    }
}
