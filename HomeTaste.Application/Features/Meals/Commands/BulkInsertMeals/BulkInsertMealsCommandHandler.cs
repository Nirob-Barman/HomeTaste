using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MealEntity = HomeTaste.Domain.Entities.MealManagement.Meal;

namespace HomeTaste.Application.Features.Meals.Commands.BulkInsertMeals
{
    public class BulkInsertMealsCommandHandler : IRequestHandler<BulkInsertMealsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertMealsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertMealsCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Predefined Bengali Meals with CategoryName
                var meals = new List<MealRequestWithCategoryName>
                {
                    // **Fish Curry** Category
                    new() { Name = "Fish Curry", Description = "A popular Bengali dish, typically made with fresh fish, mustard oil, and spices.", Price = 200, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/fish_curry.jpg", IsAvailable = true, PreparationTime = 30, Calories = 250 },
                    new() { Name = "Macher Jhol", Description = "A classic Bengali fish curry with potatoes, tomatoes, and spices, often served with steamed rice.", Price = 180, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_jhol.jpg", IsAvailable = true, PreparationTime = 25, Calories = 220 },
                    new() { Name = "Macher Bhorta", Description = "Mashed fish mixed with mustard oil, green chilies, and spices, often served as a side with rice.", Price = 160, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_bhorta.jpg", IsAvailable = true, PreparationTime = 20, Calories = 180 },
                    new() { Name = "Sorse Bata Maach", Description = "Fish cooked with mustard paste and mustard oil, giving it a unique, pungent flavor.", Price = 220, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/sorse_bata_maach.jpg", IsAvailable = true, PreparationTime = 30, Calories = 240 },
                    new() { Name = "Shorshe Ilish", Description = "Hilsa fish cooked in a mustard sauce, a true delicacy in Bengali cuisine.", Price = 500, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/shorshe_ilish.jpg", IsAvailable = true, PreparationTime = 35, Calories = 300 },
                    new() { Name = "Fried Fish", Description = "Fish marinated with spices and shallow-fried, served as a side dish with rice.", Price = 250, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/fried_fish.jpg", IsAvailable = true, PreparationTime = 20, Calories = 280 },
                    new() { Name = "Chingri Bhorta", Description = "A mashed prawn dish, cooked with mustard oil, green chilies, and other spices, served with rice.", Price = 200, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/chingri_bhorta.jpg", IsAvailable = true, PreparationTime = 25, Calories = 200 },
                    new() { Name = "Prawn Malai Curry", Description = "A mild, creamy prawn curry made with coconut milk and fragrant spices.", Price = 400, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/prawn_malai_curry.jpg", IsAvailable = true, PreparationTime = 40, Calories = 350 },
                    new() { Name = "Chingri Macher Malai", Description = "Prawns cooked in a coconut milk-based gravy with mustard oil, spices, and green chilies.", Price = 450, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/chingri_macher_malai.jpg", IsAvailable = true, PreparationTime = 45, Calories = 380 },
                    new() { Name = "Macher Paturi", Description = "Fish marinated in mustard paste, wrapped in banana leaves, and steamed.", Price = 350, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_paturi.jpg", IsAvailable = true, PreparationTime = 30, Calories = 260 },
                    new() { Name = "Macher Kalia", Description = "A rich, spiced fish curry cooked in a tomato-based gravy, served with rice.", Price = 250, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_kalia.jpg", IsAvailable = true, PreparationTime = 35, Calories = 290 },
                    new() { Name = "Panta Bhat with Fish", Description = "Leftover rice soaked in water, served with fried fish or green chilies.", Price = 120, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/panta_bhat_with_fish.jpg", IsAvailable = true, PreparationTime = 15, Calories = 200 },
                    new() { Name = "Chingri Malai Curry", Description = "Prawns cooked in a rich and creamy coconut milk gravy, with mustard oil and spices.", Price = 450, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/chingri_malai_curry.jpg", IsAvailable = true, PreparationTime = 40, Calories = 380 },
                    new() { Name = "Shorshe Bata Ilish", Description = "Hilsa fish cooked with mustard paste and mustard oil, a Bengali specialty.", Price = 500, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/shorshe_bata_ilish.jpg", IsAvailable = true, PreparationTime = 35, Calories = 300 },
                    new() { Name = "Macher Mouri", Description = "Fish cooked with a combination of mustard oil and nigella seeds, a popular Bengali flavor.", Price = 230, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_mouri.jpg", IsAvailable = true, PreparationTime = 30, Calories = 240 },

                    // **Vegetarian** Category
                    new() { Name = "Vegetarian", Description = "Meals that do not include meat or fish, often rich in lentils, vegetables, and spices.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/vegetarian.jpg", IsAvailable = true, PreparationTime = 20, Calories = 150 },
                    new() { Name = "Alur Dom", Description = "A potato-based dish, spiced and cooked in a rich gravy, commonly served with puris or rice.", Price = 120, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/alur_dom.jpg", IsAvailable = true, PreparationTime = 25, Calories = 200 },
                    new() { Name = "Cholar Dal", Description = "A flavorful Bengal gram dal, often cooked with coconut and served with rice.", Price = 130, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/cholar_dal.jpg", IsAvailable = true, PreparationTime = 30, Calories = 180 },
                    new() { Name = "Egg Curry", Description = "A flavorful curry made with hard-boiled eggs, cooked in a spicy gravy.", Price = 180, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/egg_curry.jpg", IsAvailable = true, PreparationTime = 25, Calories = 220 },
                    new() { Name = "Aloo Bhorta", Description = "Mashed potatoes with mustard oil, green chilies, and spices, often eaten with rice.", Price = 120, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/aloo_bhorta.jpg", IsAvailable = true, PreparationTime = 15, Calories = 160 },
                    new() { Name = "Shorshe Pui Shaak", Description = "A dish made with Bengali spinach cooked in mustard sauce and mustard oil.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/shorshe_pui_shaak.jpg", IsAvailable = true, PreparationTime = 20, Calories = 150 },
                    new() { Name = "Dhokar Dalna", Description = "A delicious Bengali dish made with fried lentil cakes cooked in a spicy gravy.", Price = 160, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/dhokar_dalna.jpg", IsAvailable = true, PreparationTime = 35, Calories = 220 },
                    new() { Name = "Beguni", Description = "Sliced eggplant coated with a seasoned chickpea flour batter, fried until crispy.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/beguni.jpg", IsAvailable = true, PreparationTime = 15, Calories = 180 },
                    new() { Name = "Macher Bhorta (Veg version)", Description = "A vegetarian version of the traditional Macher Bhorta, using mashed vegetables, mustard oil, and spices.", Price = 130, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/veggie_bhorta.jpg", IsAvailable = true, PreparationTime = 20, Calories = 160 },
                    new() { Name = "Shukto", Description = "A bitter-sweet vegetable medley, including bitter gourd, cooked with mustard oil and spices.", Price = 170, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/shukto.jpg", IsAvailable = true, PreparationTime = 30, Calories = 170 },
                    new() { Name = "Lau Chingri", Description = "A vegetarian version made with bottle gourd, seasoned with mustard oil, cumin, and green chilies.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/lau_chingri.jpg", IsAvailable = true, PreparationTime = 25, Calories = 150 },
                    new() { Name = "Mishti Pulao", Description = "A sweet, aromatic rice dish made with ghee, raisins, and a touch of saffron.", Price = 180, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/mishti_pulao.jpg", IsAvailable = true, PreparationTime = 30, Calories = 280 },

                    // **Biryani** Category
                    new() { Name = "Biryani", Description = "A fragrant rice dish made with spices, meat (usually chicken or mutton), and often served with raita.", Price = 300, CategoryName = "Biryani", ImageUrl = "https://example.com/images/biryani.jpg", IsAvailable = true, PreparationTime = 45, Calories = 450 },
                    new() { Name = "Chicken Biryani", Description = "A flavorful biryani made with marinated chicken, basmati rice, and a mix of aromatic spices, often served with raita.", Price = 350, CategoryName = "Biryani", ImageUrl = "https://example.com/images/chicken_biryani.jpg", IsAvailable = true, PreparationTime = 50, Calories = 480 },
                    new() { Name = "Mutton Biryani", Description = "A rich biryani made with tender mutton, marinated with spices and cooked with basmati rice.", Price = 400, CategoryName = "Biryani", ImageUrl = "https://example.com/images/mutton_biryani.jpg", IsAvailable = true, PreparationTime = 60, Calories = 520 },
                    new() { Name = "Vegetable Biryani", Description = "A vegetarian version of biryani made with a variety of vegetables and fragrant basmati rice.", Price = 250, CategoryName = "Biryani", ImageUrl = "https://example.com/images/vegetable_biryani.jpg", IsAvailable = true, PreparationTime = 40, Calories = 380 },
                    new() { Name = "Egg Biryani", Description = "A spicy and aromatic biryani made with boiled eggs, rice, and a blend of spices.", Price = 280, CategoryName = "Biryani", ImageUrl = "https://example.com/images/egg_biryani.jpg", IsAvailable = true, PreparationTime = 40, Calories = 400 },
                    new() { Name = "Fish Biryani", Description = "A delicious biryani made with fish, cooked with spices and basmati rice, often served with raita.", Price = 350, CategoryName = "Biryani", ImageUrl = "https://example.com/images/fish_biryani.jpg", IsAvailable = true, PreparationTime = 45, Calories = 430 },
                    new() { Name = "Kacchi Biryani", Description = "A traditional Bengali-style biryani made with marinated meat and raw rice, cooked together in a sealed pot.", Price = 450, CategoryName = "Biryani", ImageUrl = "https://example.com/images/kacchi_biryani.jpg", IsAvailable = true, PreparationTime = 90, Calories = 550 },
                    new() { Name = "Hyderabadi Biryani", Description = "A spicy, aromatic biryani from Hyderabad made with marinated meat, rice, and a mix of spices.", Price = 400, CategoryName = "Biryani", ImageUrl = "https://example.com/images/hyderabadi_biryani.jpg", IsAvailable = true, PreparationTime = 60, Calories = 500 },
                    new() { Name = "Lucknowi Biryani", Description = "A lighter, fragrant biryani from Lucknow made with aromatic spices, tender meat, and basmati rice.", Price = 450, CategoryName = "Biryani", ImageUrl = "https://example.com/images/lucknowi_biryani.jpg", IsAvailable = true, PreparationTime = 60, Calories = 480 },

                    // **Rice and Dal** Category
                    new() { Name = "Pulao", Description = "A fragrant rice dish made with aromatic spices, vegetables, and sometimes meat, often served with curries.", Price = 150, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/pulao.jpg", IsAvailable = true, PreparationTime = 30, Calories = 300 },
                    new() { Name = "Panta Bhat", Description = "A traditional Bengali dish made with leftover rice soaked in water, usually served with fried fish or green chilies.", Price = 120, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/panta_bhat.jpg", IsAvailable = true, PreparationTime = 10, Calories = 150 },
                    new() { Name = "Khichuri", Description = "A comfort food made with rice and lentils, often served with fried eggplant or chutney.", Price = 180, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/khichuri.jpg", IsAvailable = true, PreparationTime = 30, Calories = 280 },
                    new() { Name = "Cholar Dal", Description = "A flavorful Bengal gram dal, often cooked with coconut and served with rice.", Price = 130, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/cholar_dal.jpg", IsAvailable = true, PreparationTime = 30, Calories = 180 },
                    new() { Name = "Lentil Soup (Dal)", Description = "A staple in Bengali cuisine, this dish is made with lentils and flavored with spices and ghee.", Price = 120, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/lentil_soup.jpg", IsAvailable = true, PreparationTime = 25, Calories = 160 },
                    new() { Name = "Bengali Macher Jhol with Rice", Description = "A traditional Bengali fish curry made with potatoes, tomatoes, and spices, served with rice.", Price = 220, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/macher_jhol_rice.jpg", IsAvailable = true, PreparationTime = 35, Calories = 350 },
                    new() { Name = "Dal Tadka", Description = "A classic lentil dish, tempered with garlic, cumin, mustard seeds, and ghee, typically served with rice.", Price = 140, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/dal_tadka.jpg", IsAvailable = true, PreparationTime = 25, Calories = 200 },
                    new() { Name = "Masoor Dal", Description = "A red lentil curry cooked with onions, tomatoes, and aromatic spices, served with rice.", Price = 130, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/masoor_dal.jpg", IsAvailable = true, PreparationTime = 20, Calories = 180 },
                    new() { Name = "Dal Fry", Description = "A spiced and sautéed version of dal, usually made with yellow lentils, served with steamed rice.", Price = 150, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/dal_fry.jpg", IsAvailable = true, PreparationTime = 20, Calories = 190 },

                    // **Non-Vegetarian (Mutton/Chicken)** Category
                    new() { Name = "Kosha Mangsho", Description = "A slow-cooked, spicy mutton curry, a traditional Bengali delicacy often served with rice or paratha.", Price = 350, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/kosha_mangsho.jpg", IsAvailable = true, PreparationTime = 60, Calories = 420 },
                    new() { Name = "Tandoori Chicken", Description = "Chicken marinated with yogurt and spices, then cooked in a tandoor oven for a smoky flavor.", Price = 350, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/tandoori_chicken.jpg", IsAvailable = true, PreparationTime = 45, Calories = 350 },
                    new() { Name = "Butter Chicken", Description = "Chicken cooked in a creamy tomato-based gravy, spiced with aromatic herbs and butter.", Price = 400, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/butter_chicken.jpg", IsAvailable = true, PreparationTime = 40, Calories = 450 },
                    new() { Name = "Chicken Korma", Description = "A rich, mildly spiced chicken curry made with yogurt, cream, and ground nuts.", Price = 380, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/chicken_korma.jpg", IsAvailable = true, PreparationTime = 45, Calories = 430 },
                    new() { Name = "Mutton Rogan Josh", Description = "A flavorful, aromatic mutton curry cooked with a variety of spices, yogurt, and tomatoes.", Price = 450, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/mutton_rogan_josh.jpg", IsAvailable = true, PreparationTime = 60, Calories = 470 },
                    new() { Name = "Fish Tikka", Description = "Fish marinated in a mix of yogurt and spices, then grilled to perfection, often served with a tangy mint chutney.", Price = 300, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/fish_tikka.jpg", IsAvailable = true, PreparationTime = 30, Calories = 280 },
                    new() { Name = "Prawn Masala", Description = "Prawns cooked in a spicy and tangy masala gravy with tomatoes and onions.", Price = 450, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/prawn_masala.jpg", IsAvailable = true, PreparationTime = 35, Calories = 320 },
                    new() { Name = "Chicken Biryani", Description = "A fragrant rice dish made with spices, marinated chicken, and cooked together with saffron rice.", Price = 400, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/chicken_biryani.jpg", IsAvailable = true, PreparationTime = 50, Calories = 480 },
                    new() { Name = "Mutton Keema", Description = "Ground mutton cooked with spices, peas, and tomatoes, often served with paratha or rice.", Price = 380, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/mutton_keema.jpg", IsAvailable = true, PreparationTime = 40, Calories = 400 },
                    new() { Name = "Prawn Malai Curry", Description = "A mild, creamy prawn curry made with coconut milk and fragrant spices.", Price = 400, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/prawn_malai_curry.jpg", IsAvailable = true, PreparationTime = 40, Calories = 350 },

                    // **Snacks** Category
                    new() { Name = "Beguni", Description = "Sliced eggplant coated with a seasoned chickpea flour batter, fried until crispy.", Price = 150, CategoryName = "Snacks", ImageUrl = "https://example.com/images/beguni.jpg", IsAvailable = true, PreparationTime = 15, Calories = 180 },
                    new() { Name = "Samosa", Description = "A deep-fried pastry filled with spiced potatoes, peas, and sometimes meat.", Price = 100, CategoryName = "Snacks", ImageUrl = "https://example.com/images/samosa.jpg", IsAvailable = true, PreparationTime = 20, Calories = 200 },
                    new() { Name = "Kachuri", Description = "Stuffed fried bread, often filled with spiced peas or potatoes, commonly eaten as a snack or breakfast.", Price = 120, CategoryName = "Snacks", ImageUrl = "https://example.com/images/kachuri.jpg", IsAvailable = true, PreparationTime = 25, Calories = 220 },
                    new() { Name = "Puffs", Description = "Flaky pastry filled with spiced vegetables or minced meat, typically served as a snack.", Price = 130, CategoryName = "Snacks", ImageUrl = "https://example.com/images/puffs.jpg", IsAvailable = true, PreparationTime = 20, Calories = 210 },
                    new() { Name = "Chotpoti", Description = "A tangy and spicy street food made from peas, potatoes, and boiled eggs, topped with tamarind and spices.", Price = 140, CategoryName = "Snacks", ImageUrl = "https://example.com/images/chotpoti.jpg", IsAvailable = true, PreparationTime = 15, Calories = 180 },
                    new() { Name = "Fuchka", Description = "Crispy hollow shells filled with spicy potato filling, tamarind water, and chutney, a popular street food.", Price = 80, CategoryName = "Snacks", ImageUrl = "https://example.com/images/fuchka.jpg", IsAvailable = true, PreparationTime = 15, Calories = 120 },
                    new() { Name = "Momos", Description = "Steamed dumplings stuffed with vegetables or meat, served with spicy chutney.", Price = 160, CategoryName = "Snacks", ImageUrl = "https://example.com/images/momos.jpg", IsAvailable = true, PreparationTime = 30, Calories = 200 },
                    new() { Name = "Chingri Bhorta", Description = "A mashed prawn dish, cooked with mustard oil, green chilies, and other spices, served with rice.", Price = 200, CategoryName = "Snacks", ImageUrl = "https://example.com/images/chingri_bhorta.jpg", IsAvailable = true, PreparationTime = 25, Calories = 200 },
                    new() { Name = "Shingara", Description = "A Bengali version of samosa, stuffed with spiced potatoes, peas, and sometimes meat, then deep-fried.", Price = 120, CategoryName = "Snacks", ImageUrl = "https://example.com/images/shingara.jpg", IsAvailable = true, PreparationTime = 20, Calories = 190 },
                    new() { Name = "Muri", Description = "Puffed rice mixed with mustard oil, peanuts, and a variety of spices, a crunchy, savory snack.", Price = 50, CategoryName = "Snacks", ImageUrl = "https://example.com/images/muri.jpg", IsAvailable = true, PreparationTime = 5, Calories = 100 },

                    // **Bengali Sweets** Category
                    new() { Name = "Misti Doi", Description = "A popular Bengali dessert made with sweetened yogurt, often served chilled.", Price = 100, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/misti_doi.jpg", IsAvailable = true, PreparationTime = 10, Calories = 150 },
                    new() { Name = "Rasgulla", Description = "Soft, spongy, and sweet cheese balls soaked in sugar syrup, a quintessential Bengali sweet.", Price = 120, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/rasgulla.jpg", IsAvailable = true, PreparationTime = 20, Calories = 120 },
                    new() { Name = "Sandesh", Description = "A delicate Bengali sweet made from fresh chhena (cottage cheese), often garnished with pistachio or saffron.", Price = 150, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/sandesh.jpg", IsAvailable = true, PreparationTime = 20, Calories = 130 },
                    new() { Name = "Rosogolla", Description = "A traditional Bengali sweet made from chhena, flavored with rosewater or cardamom syrup.", Price = 120, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/rosogolla.jpg", IsAvailable = true, PreparationTime = 20, Calories = 120 },
                    new() { Name = "Kheer", Description = "A creamy rice pudding made with milk, rice, sugar, and flavored with cardamom and saffron.", Price = 130, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/kheer.jpg", IsAvailable = true, PreparationTime = 40, Calories = 250 },
                    new() { Name = "Mawa Malai", Description = "A sweet made with reduced milk, flavored with cardamom, and garnished with dry fruits.", Price = 180, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/mawa_malai.jpg", IsAvailable = true, PreparationTime = 30, Calories = 280 },
                    new() { Name = "Chamcham", Description = "Small, elongated pieces of chhena soaked in sugar syrup, often flavored with rose water.", Price = 130, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/chamcham.jpg", IsAvailable = true, PreparationTime = 25, Calories = 160 },
                    new() { Name = "Patishapta", Description = "A Bengali sweet made with thin crepes stuffed with coconut, khoya, and jaggery, and served with sugar syrup.", Price = 200, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/patishapta.jpg", IsAvailable = true, PreparationTime = 30, Calories = 200 },
                    new() { Name = "Narikol Naru", Description = "A coconut-based sweet ball, flavored with jaggery and cardamom, often prepared during festivals.", Price = 140, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/narikol_naru.jpg", IsAvailable = true, PreparationTime = 20, Calories = 180 },
                    new() { Name = "Kalonji Mishti", Description = "A Bengali sweet made from sugar syrup, cardamom, and a special kind of poppy seed called Kalonji.", Price = 160, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/kalonji_mishti.jpg", IsAvailable = true, PreparationTime = 25, Calories = 160 }
                };

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var uniqueMeals = meals.Where(m => seen.Add(m.Name!)).ToList();

                var existingNames = (await _context.Meals.Select(m => m.Name!).ToListAsync(cancellationToken))
                    .Select(n => n.ToLower())
                    .ToHashSet();

                var mealsToInsert = uniqueMeals
                    .Where(m => !existingNames.Contains(m.Name!.ToLower()))
                    .ToList();

                if (mealsToInsert.Count == 0)
                    throw new ConflictException("No new meals to insert.");

                var newMeals = new List<MealEntity>();

                foreach (var mealRequest in mealsToInsert)
                {
                    var category = await _context.MealCategories
                        .FirstOrDefaultAsync(c => c.Name == mealRequest.CategoryName, cancellationToken);

                    if (category != null)
                    {
                        newMeals.Add(MealEntity.Create(
                            mealRequest.Name,
                            mealRequest.Description,
                            mealRequest.Price,
                            category.Id,
                            mealRequest.ImageUrl,
                            mealRequest.IsAvailable,
                            mealRequest.PreparationTime,
                            mealRequest.DiscountPrice,
                            mealRequest.Calories));
                    }
                }

                if (newMeals.Count == 0)
                    throw new ConflictException("No valid meals to insert.");

                _context.Meals.AddRange(newMeals);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Ok(newMeals.Count, "New meals successfully inserted");
            }
            catch (Exception ex) when (ex is not ConflictException)
            {
                throw new ServerErrorException($"Error occurred while bulk inserting meals: {ex.Message}");
            }
        }
    }
}
