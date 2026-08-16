using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Queries.GetIngredientById
{
    public class GetIngredientByIdQueryHandler : IRequestHandler<GetIngredientByIdQuery, Result<IngredientResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetIngredientByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IngredientResponse>> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
        {
            var ingredient = await _context.Ingredients.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (ingredient == null)
                throw new NotFoundException("Ingredient not found");

            var response = new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            };

            return Result<IngredientResponse>.Ok(response, "Ingredient retrieved successfully");
        }
    }
}
