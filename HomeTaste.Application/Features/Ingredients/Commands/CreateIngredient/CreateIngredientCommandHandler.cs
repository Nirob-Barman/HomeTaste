using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Ingredients.Commands.CreateIngredient
{
    public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, Result<IngredientResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorage _fileStorage;

        public CreateIngredientCommandHandler(IApplicationDbContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<Result<IngredientResponse>> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
        {
            var ingredientRequest = request.IngredientRequest;

            var existingIngredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Name == ingredientRequest.Name, cancellationToken);

            if (existingIngredient != null)
            {
                throw new ConflictException("Ingredient with the same name already exists.");
            }

            var imageUrl = ingredientRequest.ImageUrl;
            var publicId = ingredientRequest.PublicId;

            if (request.File != null)
            {
                var imageResult = await _fileStorage.UploadFileAsync(request.File.Content!, request.File.FileName!, "uploads/ingredients");
                if (imageResult != null)
                {
                    imageUrl = imageResult.Url;
                    publicId = imageResult.PublicId;
                }
                else
                {
                    throw new BadRequestException("Failed to upload image.");
                }
            }

            var ingredient = Ingredient.Create(ingredientRequest.Name, ingredientRequest.Description, ingredientRequest.IsAllergen, imageUrl, publicId);

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            };

            return Result<IngredientResponse>.Ok(response, "Ingredient created successfully");
        }
    }
}
