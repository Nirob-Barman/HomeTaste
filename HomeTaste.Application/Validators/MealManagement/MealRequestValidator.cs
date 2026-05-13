using HomeTaste.Application.DTOs.MealManagement;

namespace HomeTaste.Application.Validators.MealManagement
{
    public static class MealRequestValidator
    {
        public static List<string> Validate(MealRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Meal name is required.");
            else if (request.Name.Trim().Length > 200)
                errors.Add("Meal name cannot exceed 200 characters.");

            if (request.Description?.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters.");

            if (request.Price <= 0)
                errors.Add("Price must be greater than zero.");

            if (request.Price > 100_000)
                errors.Add("Price seems unrealistically high.");

            if (request.CategoryId == Guid.Empty)
                errors.Add("Category is required.");

            return errors;
        }
    }
}
