using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Ingredients.Commands.UpdateIngredient
{
    public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, Result<IngredientResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateIngredientCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IngredientResponse>> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var ingredientRequest = request.IngredientRequest;

            var ingredient = await _context.Ingredients.FindAsync(new object?[] { id }, cancellationToken);
            if (ingredient == null)
                throw new NotFoundException("Ingredient not found");

            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Name == ingredientRequest.Name && i.Id != id, cancellationToken);

            if (existingIngredient != null)
            {
                throw new ConflictException("Ingredient with the same name already exists.");
            }

            ingredient.UpdateDetails(ingredientRequest.Name, ingredientRequest.Description, ingredientRequest.IsAllergen, ingredientRequest.ImageUrl);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            };

            return Result<IngredientResponse>.Ok(response, "Ingredient updated successfully");
        }
    }
}
