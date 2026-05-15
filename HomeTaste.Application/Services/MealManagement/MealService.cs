using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Validators.MealManagement;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class MealService : IMealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Meal> _mealRepository;
        //private readonly IFileStorage _fileStorage;

        public MealService(IUnitOfWork unitOfWork, IRepository<Meal> mealRepository 
            //IFileStorage fileStorage
            )
        {
            _unitOfWork = unitOfWork;
            _mealRepository = mealRepository;
            //_fileStorage = fileStorage;
        }

        public async Task<Result<IEnumerable<MealResponse>>> GetAllMealsAsync()
        {
            var meals = await _mealRepository.GetAllAsync();
            var mealResponses = meals.Select(meal => new MealResponse
            {
                Id = meal.Id,
                Name = meal.Name,
                Description = meal.Description,
                Price = meal.Price,
                ImageUrl = meal.ImageUrl,
                CategoryId = meal.CategoryId
            });
            return Result<IEnumerable<MealResponse>>.Ok(mealResponses, "Meals retrieved successfully", ResultType.Success);
        }

        public async Task<Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>> GetAllMealsAsync(int pageNumber, int pageSize, string searchTerm = null!, Guid? categoryId = null)
        {
            var query = _mealRepository.WithIncludesAsQueryable(meal => meal.MealCategory!);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(meal =>
                    meal.Name!.Contains(searchTerm) ||
                    meal.Description!.Contains(searchTerm) ||
                    (meal.MealCategory != null && meal.MealCategory.Name!.Contains(searchTerm))
                );
            }

            if (categoryId.HasValue)
            {
                query = query.Where(meal => meal.CategoryId == categoryId.Value);
            }

            var totalCount = await _mealRepository.CountAsync(query);

            var meals = (await _mealRepository.PaginateAsQueryable(query, pageNumber, pageSize,
                meal => new MealResponseWithMealCategory
                {
                    Id = meal.Id,
                    Name = meal.Name,
                    Description = meal.Description,
                    Price = meal.Price,
                    ImageUrl = meal.ImageUrl,
                    CategoryId = meal.CategoryId,
                    CategoryName = meal.MealCategory != null ? meal.MealCategory.Name : null
                })).ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            paginationMeta.CurrentPageCount = meals.Count;

            var response = new PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>
            {
                Data = meals,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>.Ok(response, "Meals retrieved successfully", ResultType.Success);            
        }


        public async Task<Result<MealResponse>> GetMealByIdAsync(Guid id)
        {
            var meal = await _mealRepository.GetByIdAsync(id);
            if (meal == null)
                return Result<MealResponse>.Fail("Meal not found", "Meal not found", ResultType.NotFound);

            var response = new MealResponse
            {
                Id = meal.Id,
                Name = meal.Name,
                Description = meal.Description,
                Price = meal.Price,
                ImageUrl = meal.ImageUrl,
                CategoryId = meal.CategoryId
            };
            return Result<MealResponse>.Ok(response, "Meal retrieved successfully", ResultType.Success);
        }

        public async Task<Result<MealResponse>> CreateMealAsync(MealRequest mealRequest, FileUploadDto? file)
        {
            var errors = MealRequestValidator.Validate(mealRequest);
            if (errors.Count > 0)
                return Result<MealResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var mealCategory = await _unitOfWork.Repository<MealCategory>().GetByIdAsync(mealRequest.CategoryId);
            if (mealCategory == null)
            {
                return Result<MealResponse>.Fail("Meal category not found.", "Invalid category ID", ResultType.NotFound);
            }

            // Check if a meal with the same name already exists in the same category
            var existingMeal = await _unitOfWork.Repository<Meal>().FirstOrDefaultAsync(m => m.Name == mealRequest.Name
            && m.CategoryId == mealRequest.CategoryId);

            if (existingMeal != null)
            {
                return Result<MealResponse>.Fail("Meal with the same name already exists in this category.", "Duplicate meal", ResultType.Conflict);
            }

            //if (file != null)
            //{
            //    string imageUrl = await _fileStorage.UploadFileAsync(file.Content!, file.FileName!, "uploads/meals");
            //    if (imageUrl != null)
            //    {
            //        mealRequest.ImageUrl = imageUrl;
            //    }
            //    else
            //    {
            //        return Result<MealResponse>.Fail("Failed to upload image.", "Image upload failed", ResultType.BadRequest);
            //    }
            //}

            var meal = new Meal
            {
                Name = mealRequest.Name,
                Description = mealRequest.Description,
                Price = mealRequest.Price,
                CategoryId = mealRequest.CategoryId,
                ImageUrl = mealRequest.ImageUrl
            };

            await _mealRepository.AddAsync(meal);
            await _unitOfWork.SaveChangesAsync();

            var response = new MealResponse
            {
                Id = meal.Id,
                Name = meal.Name,
                Description = meal.Description,
                Price = meal.Price,
                ImageUrl = meal.ImageUrl,
                CategoryId = meal.CategoryId
            };

            return Result<MealResponse>.Ok(response, "Meal created successfully", ResultType.Success);
        }

        public async Task<Result<int>> BulkInsertPredefinedMealsAsync()
        {
            try
            {
                // Predefined Bengali Meals with CategoryName
                var meals = new List<MealRequestWithCategoryName>
                {
                    // **Fish Curry** Category
                    new MealRequestWithCategoryName { Name = "Fish Curry", Description = "A popular Bengali dish, typically made with fresh fish, mustard oil, and spices.", Price = 200, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/fish_curry.jpg" },
                    new MealRequestWithCategoryName { Name = "Macher Jhol", Description = "A classic Bengali fish curry with potatoes, tomatoes, and spices, often served with steamed rice.", Price = 180, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_jhol.jpg" },
                    new MealRequestWithCategoryName { Name = "Macher Bhorta", Description = "Mashed fish mixed with mustard oil, green chilies, and spices, often served as a side with rice.", Price = 160, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_bhorta.jpg" },
                    new MealRequestWithCategoryName { Name = "Sorse Bata Maach", Description = "Fish cooked with mustard paste and mustard oil, giving it a unique, pungent flavor.", Price = 220, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/sorse_bata_maach.jpg" },
                    new MealRequestWithCategoryName { Name = "Shorshe Ilish", Description = "Hilsa fish cooked in a mustard sauce, a true delicacy in Bengali cuisine.", Price = 500, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/shorshe_ilish.jpg" },
                    new MealRequestWithCategoryName { Name = "Fried Fish", Description = "Fish marinated with spices and shallow-fried, served as a side dish with rice.", Price = 250, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/fried_fish.jpg" },
                    new MealRequestWithCategoryName { Name = "Chingri Bhorta", Description = "A mashed prawn dish, cooked with mustard oil, green chilies, and other spices, served with rice.", Price = 200, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/chingri_bhorta.jpg" },
                    new MealRequestWithCategoryName { Name = "Prawn Malai Curry", Description = "A mild, creamy prawn curry made with coconut milk and fragrant spices.", Price = 400, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/prawn_malai_curry.jpg" },
                    new MealRequestWithCategoryName { Name = "Chingri Macher Malai", Description = "Prawns cooked in a coconut milk-based gravy with mustard oil, spices, and green chilies.", Price = 450, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/chingri_macher_malai.jpg" },
                    new MealRequestWithCategoryName { Name = "Macher Paturi", Description = "Fish marinated in mustard paste, wrapped in banana leaves, and steamed.", Price = 350, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_paturi.jpg" },
                    new MealRequestWithCategoryName { Name = "Macher Kalia", Description = "A rich, spiced fish curry cooked in a tomato-based gravy, served with rice.", Price = 250, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_kalia.jpg" },
                    new MealRequestWithCategoryName { Name = "Panta Bhat with Fish", Description = "Leftover rice soaked in water, served with fried fish or green chilies.", Price = 120, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/panta_bhat_with_fish.jpg" },
                    new MealRequestWithCategoryName { Name = "Chingri Malai Curry", Description = "Prawns cooked in a rich and creamy coconut milk gravy, with mustard oil and spices.", Price = 450, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/chingri_malai_curry.jpg" },
                    new MealRequestWithCategoryName { Name = "Shorshe Bata Ilish", Description = "Hilsa fish cooked with mustard paste and mustard oil, a Bengali specialty.", Price = 500, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/shorshe_bata_ilish.jpg" },
                    new MealRequestWithCategoryName { Name = "Macher Mouri", Description = "Fish cooked with a combination of mustard oil and nigella seeds, a popular Bengali flavor.", Price = 230, CategoryName = "Fish Curry", ImageUrl = "https://example.com/images/macher_mouri.jpg" },


                    // **Vegetarian** Category
                    new MealRequestWithCategoryName { Name = "Vegetarian", Description = "Meals that do not include meat or fish, often rich in lentils, vegetables, and spices.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/vegetarian.jpg" },
                    new MealRequestWithCategoryName { Name = "Alur Dom", Description = "A potato-based dish, spiced and cooked in a rich gravy, commonly served with puris or rice.", Price = 120, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/alur_dom.jpg" },
                    new MealRequestWithCategoryName { Name = "Cholar Dal", Description = "A flavorful Bengal gram dal, often cooked with coconut and served with rice.", Price = 130, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/cholar_dal.jpg" },
                    new MealRequestWithCategoryName { Name = "Egg Curry", Description = "A flavorful curry made with hard-boiled eggs, cooked in a spicy gravy.", Price = 180, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/egg_curry.jpg" },
                    new MealRequestWithCategoryName { Name = "Aloo Bhorta", Description = "Mashed potatoes with mustard oil, green chilies, and spices, often eaten with rice.", Price = 120, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/aloo_bhorta.jpg" },
                    new MealRequestWithCategoryName { Name = "Shorshe Pui Shaak", Description = "A dish made with Bengali spinach cooked in mustard sauce and mustard oil.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/shorshe_pui_shaak.jpg" },
                    new MealRequestWithCategoryName { Name = "Dhokar Dalna", Description = "A delicious Bengali dish made with fried lentil cakes cooked in a spicy gravy.", Price = 160, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/dhokar_dalna.jpg" },
                    new MealRequestWithCategoryName { Name = "Beguni", Description = "Sliced eggplant coated with a seasoned chickpea flour batter, fried until crispy.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/beguni.jpg" },
                    new MealRequestWithCategoryName { Name = "Macher Bhorta (Veg version)", Description = "A vegetarian version of the traditional Macher Bhorta, using mashed vegetables, mustard oil, and spices.", Price = 130, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/veggie_bhorta.jpg" },
                    new MealRequestWithCategoryName { Name = "Shukto", Description = "A bitter-sweet vegetable medley, including bitter gourd, cooked with mustard oil and spices.", Price = 170, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/shukto.jpg" },
                    new MealRequestWithCategoryName { Name = "Lau Chingri", Description = "A vegetarian version made with bottle gourd, seasoned with mustard oil, cumin, and green chilies.", Price = 150, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/lau_chingri.jpg" },
                    new MealRequestWithCategoryName { Name = "Mishti Pulao", Description = "A sweet, aromatic rice dish made with ghee, raisins, and a touch of saffron.", Price = 180, CategoryName = "Vegetarian", ImageUrl = "https://example.com/images/mishti_pulao.jpg" },


                    // **Biryani** Category
                    new MealRequestWithCategoryName { Name = "Biryani", Description = "A fragrant rice dish made with spices, meat (usually chicken or mutton), and often served with raita.", Price = 300, CategoryName = "Biryani", ImageUrl = "https://example.com/images/biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Chicken Biryani", Description = "A flavorful biryani made with marinated chicken, basmati rice, and a mix of aromatic spices, often served with raita.", Price = 350, CategoryName = "Biryani", ImageUrl = "https://example.com/images/chicken_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Mutton Biryani", Description = "A rich biryani made with tender mutton, marinated with spices and cooked with basmati rice.", Price = 400, CategoryName = "Biryani", ImageUrl = "https://example.com/images/mutton_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Vegetable Biryani", Description = "A vegetarian version of biryani made with a variety of vegetables and fragrant basmati rice.", Price = 250, CategoryName = "Biryani", ImageUrl = "https://example.com/images/vegetable_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Egg Biryani", Description = "A spicy and aromatic biryani made with boiled eggs, rice, and a blend of spices.", Price = 280, CategoryName = "Biryani", ImageUrl = "https://example.com/images/egg_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Fish Biryani", Description = "A delicious biryani made with fish, cooked with spices and basmati rice, often served with raita.", Price = 350, CategoryName = "Biryani", ImageUrl = "https://example.com/images/fish_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Kacchi Biryani", Description = "A traditional Bengali-style biryani made with marinated meat and raw rice, cooked together in a sealed pot.", Price = 450, CategoryName = "Biryani", ImageUrl = "https://example.com/images/kacchi_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Hyderabadi Biryani", Description = "A spicy, aromatic biryani from Hyderabad made with marinated meat, rice, and a mix of spices.", Price = 400, CategoryName = "Biryani", ImageUrl = "https://example.com/images/hyderabadi_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Lucknowi Biryani", Description = "A lighter, fragrant biryani from Lucknow made with aromatic spices, tender meat, and basmati rice.", Price = 450, CategoryName = "Biryani", ImageUrl = "https://example.com/images/lucknowi_biryani.jpg" },

                    // **Rice and Dal** Category
                    new MealRequestWithCategoryName { Name = "Pulao", Description = "A fragrant rice dish made with aromatic spices, vegetables, and sometimes meat, often served with curries.", Price = 150, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/pulao.jpg" },
                    new MealRequestWithCategoryName { Name = "Panta Bhat", Description = "A traditional Bengali dish made with leftover rice soaked in water, usually served with fried fish or green chilies.", Price = 120, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/panta_bhat.jpg" },
                    new MealRequestWithCategoryName { Name = "Khichuri", Description = "A comfort food made with rice and lentils, often served with fried eggplant or chutney.", Price = 180, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/khichuri.jpg" },
                    new MealRequestWithCategoryName { Name = "Cholar Dal", Description = "A flavorful Bengal gram dal, often cooked with coconut and served with rice.", Price = 130, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/cholar_dal.jpg" },
                    new MealRequestWithCategoryName { Name = "Lentil Soup (Dal)", Description = "A staple in Bengali cuisine, this dish is made with lentils and flavored with spices and ghee.", Price = 120, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/lentil_soup.jpg" },
                    new MealRequestWithCategoryName { Name = "Bengali Macher Jhol with Rice", Description = "A traditional Bengali fish curry made with potatoes, tomatoes, and spices, served with rice.", Price = 220, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/macher_jhol_rice.jpg" },
                    new MealRequestWithCategoryName { Name = "Dal Tadka", Description = "A classic lentil dish, tempered with garlic, cumin, mustard seeds, and ghee, typically served with rice.", Price = 140, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/dal_tadka.jpg" },
                    new MealRequestWithCategoryName { Name = "Masoor Dal", Description = "A red lentil curry cooked with onions, tomatoes, and aromatic spices, served with rice.", Price = 130, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/masoor_dal.jpg" },
                    new MealRequestWithCategoryName { Name = "Dal Fry", Description = "A spiced and sautéed version of dal, usually made with yellow lentils, served with steamed rice.", Price = 150, CategoryName = "Rice and Dal", ImageUrl = "https://example.com/images/dal_fry.jpg" },

                    // **Non-Vegetarian (Mutton/Chicken)** Category
                    new MealRequestWithCategoryName { Name = "Kosha Mangsho", Description = "A slow-cooked, spicy mutton curry, a traditional Bengali delicacy often served with rice or paratha.", Price = 350, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/kosha_mangsho.jpg" },
                    new MealRequestWithCategoryName { Name = "Tandoori Chicken", Description = "Chicken marinated with yogurt and spices, then cooked in a tandoor oven for a smoky flavor.", Price = 350, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/tandoori_chicken.jpg" },
                    new MealRequestWithCategoryName { Name = "Butter Chicken", Description = "Chicken cooked in a creamy tomato-based gravy, spiced with aromatic herbs and butter.", Price = 400, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/butter_chicken.jpg" },
                    new MealRequestWithCategoryName { Name = "Chicken Korma", Description = "A rich, mildly spiced chicken curry made with yogurt, cream, and ground nuts.", Price = 380, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/chicken_korma.jpg" },
                    new MealRequestWithCategoryName { Name = "Mutton Rogan Josh", Description = "A flavorful, aromatic mutton curry cooked with a variety of spices, yogurt, and tomatoes.", Price = 450, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/mutton_rogan_josh.jpg" },
                    new MealRequestWithCategoryName { Name = "Fish Tikka", Description = "Fish marinated in a mix of yogurt and spices, then grilled to perfection, often served with a tangy mint chutney.", Price = 300, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/fish_tikka.jpg" },
                    new MealRequestWithCategoryName { Name = "Prawn Masala", Description = "Prawns cooked in a spicy and tangy masala gravy with tomatoes and onions.", Price = 450, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/prawn_masala.jpg" },
                    new MealRequestWithCategoryName { Name = "Chicken Biryani", Description = "A fragrant rice dish made with spices, marinated chicken, and cooked together with saffron rice.", Price = 400, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/chicken_biryani.jpg" },
                    new MealRequestWithCategoryName { Name = "Mutton Keema", Description = "Ground mutton cooked with spices, peas, and tomatoes, often served with paratha or rice.", Price = 380, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/mutton_keema.jpg" },
                    new MealRequestWithCategoryName { Name = "Prawn Malai Curry", Description = "A mild, creamy prawn curry made with coconut milk and fragrant spices.", Price = 400, CategoryName = "Non-Vegetarian (Mutton/Chicken)", ImageUrl = "https://example.com/images/prawn_malai_curry.jpg" },

                    // **Snacks** Category
                    new MealRequestWithCategoryName { Name = "Beguni", Description = "Sliced eggplant coated with a seasoned chickpea flour batter, fried until crispy.", Price = 150, CategoryName = "Snacks", ImageUrl = "https://example.com/images/beguni.jpg" },
                    new MealRequestWithCategoryName { Name = "Samosa", Description = "A deep-fried pastry filled with spiced potatoes, peas, and sometimes meat.", Price = 100, CategoryName = "Snacks", ImageUrl = "https://example.com/images/samosa.jpg" },
                    new MealRequestWithCategoryName { Name = "Kachuri", Description = "Stuffed fried bread, often filled with spiced peas or potatoes, commonly eaten as a snack or breakfast.", Price = 120, CategoryName = "Snacks", ImageUrl = "https://example.com/images/kachuri.jpg" },
                    new MealRequestWithCategoryName { Name = "Puffs", Description = "Flaky pastry filled with spiced vegetables or minced meat, typically served as a snack.", Price = 130, CategoryName = "Snacks", ImageUrl = "https://example.com/images/puffs.jpg" },
                    new MealRequestWithCategoryName { Name = "Chotpoti", Description = "A tangy and spicy street food made from peas, potatoes, and boiled eggs, topped with tamarind and spices.", Price = 140, CategoryName = "Snacks", ImageUrl = "https://example.com/images/chotpoti.jpg" },
                    new MealRequestWithCategoryName { Name = "Fuchka", Description = "Crispy hollow shells filled with spicy potato filling, tamarind water, and chutney, a popular street food.", Price = 80, CategoryName = "Snacks", ImageUrl = "https://example.com/images/fuchka.jpg" },
                    new MealRequestWithCategoryName { Name = "Momos", Description = "Steamed dumplings stuffed with vegetables or meat, served with spicy chutney.", Price = 160, CategoryName = "Snacks", ImageUrl = "https://example.com/images/momos.jpg" },
                    new MealRequestWithCategoryName { Name = "Chingri Bhorta", Description = "A mashed prawn dish, cooked with mustard oil, green chilies, and other spices, served with rice.", Price = 200, CategoryName = "Snacks", ImageUrl = "https://example.com/images/chingri_bhorta.jpg" },
                    new MealRequestWithCategoryName { Name = "Shingara", Description = "A Bengali version of samosa, stuffed with spiced potatoes, peas, and sometimes meat, then deep-fried.", Price = 120, CategoryName = "Snacks", ImageUrl = "https://example.com/images/shingara.jpg" },
                    new MealRequestWithCategoryName { Name = "Muri", Description = "Puffed rice mixed with mustard oil, peanuts, and a variety of spices, a crunchy, savory snack.", Price = 50, CategoryName = "Snacks", ImageUrl = "https://example.com/images/muri.jpg" },

                    // **Bengali Sweets** Category
                    new MealRequestWithCategoryName { Name = "Misti Doi", Description = "A popular Bengali dessert made with sweetened yogurt, often served chilled.", Price = 100, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/misti_doi.jpg" },
                    new MealRequestWithCategoryName { Name = "Rasgulla", Description = "Soft, spongy, and sweet cheese balls soaked in sugar syrup, a quintessential Bengali sweet.", Price = 120, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/rasgulla.jpg" },
                    new MealRequestWithCategoryName { Name = "Sandesh", Description = "A delicate Bengali sweet made from fresh chhena (cottage cheese), often garnished with pistachio or saffron.", Price = 150, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/sandesh.jpg" },
                    new MealRequestWithCategoryName { Name = "Rosogolla", Description = "A traditional Bengali sweet made from chhena, flavored with rosewater or cardamom syrup.", Price = 120, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/rosogolla.jpg" },
                    new MealRequestWithCategoryName { Name = "Kheer", Description = "A creamy rice pudding made with milk, rice, sugar, and flavored with cardamom and saffron.", Price = 130, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/kheer.jpg" },
                    new MealRequestWithCategoryName { Name = "Mawa Malai", Description = "A sweet made with reduced milk, flavored with cardamom, and garnished with dry fruits.", Price = 180, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/mawa_malai.jpg" },
                    new MealRequestWithCategoryName { Name = "Chamcham", Description = "Small, elongated pieces of chhena soaked in sugar syrup, often flavored with rose water.", Price = 130, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/chamcham.jpg" },
                    new MealRequestWithCategoryName { Name = "Patishapta", Description = "A Bengali sweet made with thin crepes stuffed with coconut, khoya, and jaggery, and served with sugar syrup.", Price = 200, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/patishapta.jpg" },
                    new MealRequestWithCategoryName { Name = "Narikol Naru", Description = "A coconut-based sweet ball, flavored with jaggery and cardamom, often prepared during festivals.", Price = 140, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/narikol_naru.jpg" },
                    new MealRequestWithCategoryName { Name = "Kalonji Mishti", Description = "A Bengali sweet made from sugar syrup, cardamom, and a special kind of poppy seed called Kalonji.", Price = 160, CategoryName = "Bengali Sweets", ImageUrl = "https://example.com/images/kalonji_mishti.jpg" }
                };

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var uniqueMeals = meals.Where(m => seen.Add(m.Name!)).ToList();

                var existingNames = (await _mealRepository.GetAllAsync())
                    .Select(m => m.Name!.ToLower())
                    .ToHashSet();

                var mealsToInsert = uniqueMeals
                    .Where(m => !existingNames.Contains(m.Name!.ToLower()))
                    .ToList();

                if (mealsToInsert.Count == 0)
                    return Result<int>.Fail("No new meals to insert.", "All predefined meals already exist", ResultType.Conflict);

                var newMeals = new List<Meal>();

                foreach (var mealRequest in mealsToInsert)
                {
                    var category = await _unitOfWork.Repository<MealCategory>()
                        .FirstOrDefaultAsync(c => c.Name == mealRequest.CategoryName);

                    if (category != null)
                    {
                        newMeals.Add(new Meal
                        {
                            Name = mealRequest.Name,
                            Description = mealRequest.Description,
                            Price = mealRequest.Price,
                            CategoryId = category.Id,
                            ImageUrl = mealRequest.ImageUrl
                        });
                    }
                }

                if (newMeals.Count == 0)
                {
                    return Result<int>.Fail("No valid meals to insert.", "All meals have invalid categories", ResultType.Conflict);
                }

                // Bulk insert the valid meals
                await _mealRepository.AddRangeAsync(newMeals);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newMeals.Count, "New meals successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting meals: {ex.Message}", "", ResultType.Failure);
            }
        }


        public async Task<Result<MealResponse>> UpdateMealAsync(Guid id, MealRequest mealRequest)
        {
            var errors = MealRequestValidator.Validate(mealRequest);
            if (errors.Count > 0)
                return Result<MealResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var meal = await _mealRepository.GetByIdAsync(id);
            if (meal == null)
                return Result<MealResponse>.Fail("Meal not found", "Meal not found", ResultType.NotFound);


            var mealCategory = await _unitOfWork.Repository<MealCategory>().GetByIdAsync(mealRequest.CategoryId);
            if (mealCategory == null)
            {
                return Result<MealResponse>.Fail("MealCategory not found", "MealCategory not found", ResultType.NotFound);
            }

            // Check if a meal with the same name already exists in the same category (excluding the current meal being updated)
            var existingMeal = await _unitOfWork.Repository<Meal>().FirstOrDefaultAsync(m =>
            m.Name == mealRequest.Name && m.CategoryId == mealRequest.CategoryId && m.Id != id);

            if (existingMeal != null)
            {
                return Result<MealResponse>.Fail("Meal with the same name already exists in this category.", "Duplicate meal", ResultType.Conflict);
            }

            meal.Name = mealRequest.Name ?? meal.Name;
            meal.Description = mealRequest.Description ?? meal.Description;
            meal.Price = mealRequest.Price;
            meal.CategoryId = mealRequest.CategoryId;
            meal.ImageUrl = mealRequest.ImageUrl ?? meal.ImageUrl;

            _mealRepository.Update(meal);
            await _unitOfWork.SaveChangesAsync();

            var response = new MealResponse
            {
                Id = meal.Id,
                Name = meal.Name,
                Description = meal.Description,
                Price = meal.Price,
                ImageUrl = meal.ImageUrl,
                CategoryId = meal.CategoryId
            };

            return Result<MealResponse>.Ok(response, "Meal updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteMealAsync(Guid id)
        {
            var meal = await _mealRepository.GetByIdAsync(id);
            if (meal == null)
                return Result<bool>.Fail("Meal not found", "Meal not found", ResultType.NotFound);

            _mealRepository.Remove(meal);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Meal deleted successfully", ResultType.Success);
        }
    }
}
