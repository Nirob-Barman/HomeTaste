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

        public async Task<Result<IngredientResponse>> Handle(UpdateIngredientCommand command, CancellationToken cancellationToken)
        {
            var ingredient = await _context.Ingredients.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (ingredient == null)
                throw new NotFoundException("Ingredient not found");

            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Name == command.Name && i.Id != command.Id, cancellationToken);

            if (existingIngredient != null)
            {
                throw new ConflictException("Ingredient with the same name already exists.");
            }

            ingredient.UpdateDetails(command.Name, command.Description, command.IsAllergen, command.ImageUrl);

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
