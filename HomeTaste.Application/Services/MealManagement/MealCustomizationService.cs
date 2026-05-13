using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class MealCustomizationService : IMealCustomizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MealCustomizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<MealCustomizationOptionResponse>>> GetOptionsByMealIdAsync(Guid mealId)
        {
            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(mealId);
            if (meal == null)
                return Result<IEnumerable<MealCustomizationOptionResponse>>.Fail("Meal not found.", "Not found", ResultType.NotFound);

            var options = await _unitOfWork.Repository<MealCustomizationOption>()
                .Where(o => o.MealId == mealId);

            var response = options.Select(o => MapToResponse(o, meal.Name));
            return Result<IEnumerable<MealCustomizationOptionResponse>>.Ok(response, "Options retrieved successfully", ResultType.Success);
        }

        public async Task<Result<MealCustomizationOptionResponse>> GetOptionByIdAsync(Guid id)
        {
            var option = await _unitOfWork.Repository<MealCustomizationOption>().GetByIdAsync(id);
            if (option == null)
                return Result<MealCustomizationOptionResponse>.Fail("Option not found.", "Not found", ResultType.NotFound);

            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(option.MealId);
            return Result<MealCustomizationOptionResponse>.Ok(MapToResponse(option, meal?.Name), "Option retrieved successfully", ResultType.Success);
        }

        public async Task<Result<MealCustomizationOptionResponse>> CreateOptionAsync(MealCustomizationOptionRequest request)
        {
            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(request.MealId);
            if (meal == null)
                return Result<MealCustomizationOptionResponse>.Fail("Meal not found.", "Not found", ResultType.NotFound);

            var option = new MealCustomizationOption
            {
                Id = Guid.NewGuid(),
                MealId = request.MealId,
                Name = request.Name,
                AdditionalPrice = request.AdditionalPrice,
                IsAvailable = request.IsAvailable,
                OptionType = request.OptionType,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<MealCustomizationOption>().AddAsync(option);
            await _unitOfWork.SaveChangesAsync();

            return Result<MealCustomizationOptionResponse>.Ok(MapToResponse(option, meal.Name), "Option created successfully", ResultType.Created);
        }

        public async Task<Result<MealCustomizationOptionResponse>> UpdateOptionAsync(Guid id, MealCustomizationOptionRequest request)
        {
            var option = await _unitOfWork.Repository<MealCustomizationOption>().GetByIdAsync(id);
            if (option == null)
                return Result<MealCustomizationOptionResponse>.Fail("Option not found.", "Not found", ResultType.NotFound);

            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(request.MealId);
            if (meal == null)
                return Result<MealCustomizationOptionResponse>.Fail("Meal not found.", "Not found", ResultType.NotFound);

            option.MealId = request.MealId;
            option.Name = request.Name;
            option.AdditionalPrice = request.AdditionalPrice;
            option.IsAvailable = request.IsAvailable;
            option.OptionType = request.OptionType;
            option.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<MealCustomizationOption>().Update(option);
            await _unitOfWork.SaveChangesAsync();

            return Result<MealCustomizationOptionResponse>.Ok(MapToResponse(option, meal.Name), "Option updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteOptionAsync(Guid id)
        {
            var option = await _unitOfWork.Repository<MealCustomizationOption>().GetByIdAsync(id);
            if (option == null)
                return Result<bool>.Fail("Option not found.", "Not found", ResultType.NotFound);

            _unitOfWork.Repository<MealCustomizationOption>().Remove(option);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Option deleted successfully", ResultType.Success);
        }

        public async Task<Result<bool>> ToggleAvailabilityAsync(Guid id)
        {
            var option = await _unitOfWork.Repository<MealCustomizationOption>().GetByIdAsync(id);
            if (option == null)
                return Result<bool>.Fail("Option not found.", "Not found", ResultType.NotFound);

            option.IsAvailable = !option.IsAvailable;
            option.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<MealCustomizationOption>().Update(option);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(option.IsAvailable, $"Option marked as {(option.IsAvailable ? "available" : "unavailable")}", ResultType.Success);
        }

        private static MealCustomizationOptionResponse MapToResponse(MealCustomizationOption option, string? mealName) => new()
        {
            Id = option.Id,
            MealId = option.MealId,
            MealName = mealName,
            Name = option.Name,
            AdditionalPrice = option.AdditionalPrice,
            IsAvailable = option.IsAvailable,
            OptionType = option.OptionType,
            CreatedAt = option.CreatedAt
        };
    }
}
