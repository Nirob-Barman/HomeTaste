using System.ComponentModel.DataAnnotations;

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealPreference
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [MaxLength(100)]
        public string? DietaryRestrictions { get; set; } // Example: "Gluten-Free", "Vegan", "Vegetarian"

        public bool? IsVegetarian { get; set; } // Boolean flag for vegetarian preference

        public bool? IsVegan { get; set; } // Boolean flag for vegan preference

        public bool? HasAllergies { get; set; } // Boolean flag indicating if the user has allergies

        public string? AllergyDetails { get; set; } // Example: "Peanuts, Shellfish"

        public bool? IsLactoseIntolerant { get; set; } // Boolean flag for lactose intolerance

        [MaxLength(500)]
        public string? OtherPreferences { get; set; } // Any other specific preferences (e.g., "No spicy food")
    }
}
