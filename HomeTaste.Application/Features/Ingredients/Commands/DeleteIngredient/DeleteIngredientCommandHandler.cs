using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.DeleteIngredient
{
    public class DeleteIngredientCommandHandler : IRequestHandler<DeleteIngredientCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorage _fileStorage;

        public DeleteIngredientCommandHandler(IApplicationDbContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<Result<bool>> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
        {
            var ingredient = await _context.Ingredients.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (ingredient == null)
                throw new NotFoundException("Ingredient not found");

            if (!string.IsNullOrEmpty(ingredient.PublicId))
            {
                //Mark ingredient as Deleted (soft delete)
                //Commit DB transaction
                //Publish a domain event
                //Background worker deletes file
                //If file deletion fails  retry

                var fileDeleted = await _fileStorage.DeleteFileAsync(ingredient.PublicId);
                if (!fileDeleted)
                {
                    throw new ServerErrorException("Failed to delete associated file");
                }
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Ingredient deleted successfully");
        }
    }
}
