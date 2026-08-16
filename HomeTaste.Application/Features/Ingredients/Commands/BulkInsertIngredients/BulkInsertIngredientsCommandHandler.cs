using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Ingredients.Commands.BulkInsertIngredients
{
    public class BulkInsertIngredientsCommandHandler : IRequestHandler<BulkInsertIngredientsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertIngredientsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertIngredientsCommand command, CancellationToken cancellationToken)
        {
            // Predefined ingredients
            var predefined = new List<(string Name, string Description, string ImageUrl)>
            {
                ("Bengali Mustard Oil", "A traditional cooking oil made from mustard seeds, commonly used in Bengali cuisine.", "https://example.com/images/mustard_oil.jpg"),
                ("Rohu Fish", "A popular freshwater fish in Bengali cuisine, known for its delicate flavor.", "https://example.com/images/rohu_fish.jpg"),
                ("Green Chilies", "Spicy chilies commonly used in Bengali cooking to add heat and flavor.", "https://example.com/images/green_chilies.jpg"),
                ("Basmati Rice", "A long-grain aromatic rice, commonly used in Bengali and Indian dishes.", "https://example.com/images/basmati_rice.jpg"),
                ("Mung Dal", "A type of lentil commonly used in Bengali dals and soups.", "https://example.com/images/mung_dal.jpg"),
                ("Ginger (Ada)", "A staple spice in Bengali cooking, used in almost every dish for its aromatic and spicy flavor.", "https://example.com/images/ginger.jpg"),
                ("Onion", "A staple vegetable used in almost every savory Bengali dish for flavor.", "https://example.com/images/onion.jpg"),
                ("Tomato", "A common vegetable used in Bengali curries, dals, and salads.", "https://example.com/images/tomato.jpg"),
                ("Garlic", "A pungent spice commonly used in Bengali cooking for its strong flavor.", "https://example.com/images/garlic.jpg"),
                ("Cumin Seeds", "A spice used in Bengali cooking, often used for tempering and flavoring dishes.", "https://example.com/images/cumin_seeds.jpg"),
                ("Turmeric", "A bright yellow spice used in Bengali cooking for its color and earthy flavor.", "https://example.com/images/turmeric.jpg"),
                ("Mustard Seeds", "Small seeds often used for tempering and flavoring Bengali dishes.", "https://example.com/images/mustard_seeds.jpg"),
                ("Potatoes", "A versatile vegetable used in Bengali curries, fries, and other dishes.", "https://example.com/images/potatoes.jpg"),
                ("Rice Flour", "Flour made from rice used in Bengali cooking for thickening gravies and making sweets.", "https://example.com/images/rice_flour.jpg"),
                ("Salt", "A basic seasoning used in almost every Bengali dish for flavoring.", "https://example.com/images/salt.jpg"),
                ("Sugar", "Used in both savory and sweet Bengali dishes, especially in sweets like rasgulla.", "https://example.com/images/sugar.jpg"),
                ("Hilsa Fish", "A delicate and oily fish, often considered the king of Bengali fish, used in various Bengali fish dishes.", "https://example.com/images/hilsa_fish.jpg"),
                ("Chili Powder", "Ground dried chilies used for adding heat and color to Bengali dishes.", "https://example.com/images/chili_powder.jpg"),
                ("Ghee", "Clarified butter used in Bengali cuisine for flavoring rice, dals, and sweets.", "https://example.com/images/ghee.jpg"),
                ("Jaggery", "Unrefined sugar made from sugarcane or palm, often used in Bengali sweets.", "https://example.com/images/jaggery.jpg"),
                ("Mustard Paste", "A paste made from mustard seeds, used in Bengali cooking to add flavor and heat.", "https://example.com/images/mustard_paste.jpg"),
                ("Coconut", "Used in both grated and milk form, coconut adds a unique flavor to Bengali curries and sweets.", "https://example.com/images/coconut.jpg"),
                ("Saffron", "A highly prized spice used for coloring and flavoring in Bengali biryanis and sweets.", "https://example.com/images/saffron.jpg"),
                ("Garam Masala", "A blend of ground spices, including cinnamon, cardamom, and cloves, used for seasoning in Bengali dishes.", "https://example.com/images/garam_masala.jpg"),
                ("Coriander Powder", "A ground spice made from dried coriander seeds, often used to flavor Bengali curries and dals.", "https://example.com/images/coriander_powder.jpg"),
                ("Fennel Seeds", "Used for tempering, fennel seeds have a sweet licorice-like flavor and are used in Bengali dishes.", "https://example.com/images/fennel_seeds.jpg"),
                ("Tamarind", "A sour fruit used in Bengali cooking for adding tangy flavors to chutneys and curries.", "https://example.com/images/tamarind.jpg"),
                ("Fenugreek Seeds", "Used in small quantities in Bengali cooking for their slightly bitter taste and health benefits.", "https://example.com/images/fenugreek_seeds.jpg"),
                ("Lemon", "Used to add a tangy flavor in Bengali cooking, especially in fish dishes and dals.", "https://example.com/images/lemon.jpg"),
                ("Bay Leaves", "Used for tempering, bay leaves add a subtle aromatic flavor to Bengali dishes.", "https://example.com/images/bay_leaves.jpg"),
                ("Chicken", "A common poultry meat used in various savory dishes, especially biryanis and curries.", "https://example.com/images/chicken.jpg"),
                ("Rice", "A staple grain used as the base for many Bengali dishes, especially biryanis.", "https://example.com/images/rice.jpg"),
                ("Biryani Masala", "A blend of aromatic spices used to flavor biryani rice, giving it a distinct taste and fragrance.", "https://example.com/images/biryani_masala.jpg"),
                ("Yogurt", "A creamy dairy product used in marinades, gravies, and sauces.", "https://example.com/images/yogurt.jpg"),
                ("Ginger-Garlic Paste", "A mixture of ginger and garlic, often used as a base in Indian curries and gravies.", "https://example.com/images/ginger_garlic_paste.jpg"),
                ("Cinnamon", "A warm and aromatic spice used in Bengali cooking, particularly in biryanis and curries.", "https://example.com/images/cinnamon.jpg"),
                ("Cloves", "A pungent spice with a warm, sweet flavor, commonly used in Bengali dishes.", "https://example.com/images/cloves.jpg"),
                ("Bay Leaf", "A fragrant leaf used in Bengali cooking to add a subtle aromatic flavor to dishes.", "https://example.com/images/bay_leaf.jpg"),
                ("Cardamom", "A sweet and fragrant spice, often used in Bengali sweets and biryanis.", "https://example.com/images/cardamom.jpg"),
                ("Coriander Leaves", "Fresh green leaves used as garnish in Bengali dishes for flavor and decoration.", "https://example.com/images/coriander_leaves.jpg"),
                ("Mint Leaves", "A refreshing herb used in Bengali dishes, especially in biryanis and salads.", "https://example.com/images/mint_leaves.jpg"),
            };

            var newIngredients = new List<Ingredient>();

            foreach (var (name, description, imageUrl) in predefined)
            {
                var ingredientExists = await _context.Ingredients.AnyAsync(i => i.Name == name, cancellationToken);

                if (!ingredientExists)
                {
                    newIngredients.Add(Ingredient.Create(name, description, false, imageUrl, null));
                }
            }

            if (!newIngredients.Any())
            {
                throw new ConflictException("All ingredients already exist.");
            }

            _context.Ingredients.AddRange(newIngredients);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(newIngredients.Count, "New ingredients successfully inserted");
        }
    }
}
