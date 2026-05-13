using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class IngredientService : IIngredientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Ingredient> _ingredientRepository;
        private readonly IFileStorage _fileStorage;

        public IngredientService(IUnitOfWork unitOfWork, 
            IRepository<Ingredient> ingredientRepository,
            IFileStorage fileStorage)
        {
            _unitOfWork = unitOfWork;
            _ingredientRepository = ingredientRepository;
            _fileStorage = fileStorage;
        }


        public async Task<Result<PaginatedResponse<IEnumerable<IngredientResponse>>>> GetAllIngredientsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!, string sortBy = "Id", string sortOrder = "ASC")
        {
            var validSortColumns = new List<string> { "Id", "Name", "CreatedAt" };
            if (!validSortColumns.Contains(sortBy))
            {
                return Result<PaginatedResponse<IEnumerable<IngredientResponse>>>.Fail("Invalid sort column", "Invalid sort column.", ResultType.Failure);
            }

            int offset = (pageNumber - 1) * pageSize;
            //string sqlQuery = @"
            //                SELECT COUNT(*) OVER () AS TotalCount, Id, Name, Description, IsAllergen, ImageUrl
            //                FROM Ingredients
            //                ORDER BY Id
            //                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            string selectQuery = @"
                            SELECT COUNT(*) OVER () AS TotalCount, Id, Name, Description, IsAllergen, ImageUrl
                            FROM Ingredients";

            string whereClause = string.Empty;
            sortOrder = sortOrder?.ToUpper() == "DESC" ? "DESC" : "ASC";
            string orderBy = $@"
                ORDER BY {sortBy} {sortOrder}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                whereClause = " WHERE (Name LIKE @SearchTerm OR Description LIKE @SearchTerm)";
            }

            string sqlQuery = selectQuery + whereClause + orderBy;

            var parameters = new
            {
                Offset = offset,
                PageSize = pageSize,
                SearchTerm = "%" + searchTerm + "%"
            };

            //var result = await _unitOfWork.QueryAsync<dynamic>(sqlQuery, parameters);
            var result = await _unitOfWork.QueryAsync<IngredientPaginationResult>(sqlQuery, parameters);
            int totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

            var ingredients = result.Select(ingredient => new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            }).ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            var currentPageCount = ingredients.Count();
            paginationMeta.CurrentPageCount = currentPageCount;

            // Construct the response with paginated data and metadata
            var response = new PaginatedResponse<IEnumerable<IngredientResponse>>
            {
                MetaData = paginationMeta,
                Data = ingredients
            };

            return Result<PaginatedResponse<IEnumerable<IngredientResponse>>>.Ok(response, "Ingredients retrieved successfully", ResultType.Success);
        }



        public async Task<Result<IngredientResponse>> GetIngredientByIdAsync(Guid id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return Result<IngredientResponse>.Fail("Ingredient not found", "Ingredient not found", ResultType.NotFound);

            var response = new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            };

            return Result<IngredientResponse>.Ok(response, "Ingredient retrieved successfully", ResultType.Success);
        }

        public async Task<Result<IngredientResponse>> CreateIngredientAsync(IngredientRequest ingredientRequest, FileUploadDto? file)
        {
            var existingIngredient = await _ingredientRepository.FirstOrDefaultAsync(i => i.Name == ingredientRequest.Name);

            if (existingIngredient != null)
            {
                return Result<IngredientResponse>.Fail("Ingredient with the same name already exists.", "Duplicate ingredient", ResultType.Conflict);
            }

            if (file != null)
            {
                var imageResult = await _fileStorage.UploadFileAsync(file.Content!, file.FileName!, "uploads/ingredients");
                if (imageResult != null)
                {
                    ingredientRequest.ImageUrl = imageResult.Url;
                    ingredientRequest.PublicId = imageResult.PublicId;
                }
                else
                {
                    return Result<IngredientResponse>.Fail("Failed to upload image.", "Image upload failed", ResultType.BadRequest);
                }
            }

            var ingredient = new Ingredient
            {
                Name = ingredientRequest.Name,
                Description = ingredientRequest.Description,
                IsAllergen = ingredientRequest.IsAllergen,
                ImageUrl = ingredientRequest.ImageUrl,
                PublicId = ingredientRequest.PublicId
            };

            await _ingredientRepository.AddAsync(ingredient);
            await _unitOfWork.SaveChangesAsync();

            var response = new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            };

            return Result<IngredientResponse>.Ok(response, "Ingredient created successfully", ResultType.Success);
        }

        public async Task<Result<int>> BulkInsertPredefinedIngredientsAsync()
        {
            try
            {
                // Predefined ingredients                
                var ingredients = new List<Ingredient>
                {
                    new Ingredient { Name = "Bengali Mustard Oil", Description = "A traditional cooking oil made from mustard seeds, commonly used in Bengali cuisine.", IsAllergen = false, ImageUrl = "https://example.com/images/mustard_oil.jpg" },
                    new Ingredient { Name = "Rohu Fish", Description = "A popular freshwater fish in Bengali cuisine, known for its delicate flavor.", IsAllergen = false, ImageUrl = "https://example.com/images/rohu_fish.jpg" },
                    new Ingredient { Name = "Green Chilies", Description = "Spicy chilies commonly used in Bengali cooking to add heat and flavor.", IsAllergen = false, ImageUrl = "https://example.com/images/green_chilies.jpg" },
                    new Ingredient { Name = "Basmati Rice", Description = "A long-grain aromatic rice, commonly used in Bengali and Indian dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/basmati_rice.jpg" },
                    new Ingredient { Name = "Mung Dal", Description = "A type of lentil commonly used in Bengali dals and soups.", IsAllergen = false, ImageUrl = "https://example.com/images/mung_dal.jpg" },
                    new Ingredient { Name = "Ginger (Ada)", Description = "A staple spice in Bengali cooking, used in almost every dish for its aromatic and spicy flavor.", IsAllergen = false, ImageUrl = "https://example.com/images/ginger.jpg" },
                    new Ingredient { Name = "Onion", Description = "A staple vegetable used in almost every savory Bengali dish for flavor.", IsAllergen = false, ImageUrl = "https://example.com/images/onion.jpg" },
                    new Ingredient { Name = "Tomato", Description = "A common vegetable used in Bengali curries, dals, and salads.", IsAllergen = false, ImageUrl = "https://example.com/images/tomato.jpg" },
                    new Ingredient { Name = "Garlic", Description = "A pungent spice commonly used in Bengali cooking for its strong flavor.", IsAllergen = false, ImageUrl = "https://example.com/images/garlic.jpg" },
                    new Ingredient { Name = "Cumin Seeds", Description = "A spice used in Bengali cooking, often used for tempering and flavoring dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/cumin_seeds.jpg" },
                    new Ingredient { Name = "Turmeric", Description = "A bright yellow spice used in Bengali cooking for its color and earthy flavor.", IsAllergen = false, ImageUrl = "https://example.com/images/turmeric.jpg" },
                    new Ingredient { Name = "Mustard Seeds", Description = "Small seeds often used for tempering and flavoring Bengali dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/mustard_seeds.jpg" },
                    new Ingredient { Name = "Potatoes", Description = "A versatile vegetable used in Bengali curries, fries, and other dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/potatoes.jpg" },
                    new Ingredient { Name = "Rice Flour", Description = "Flour made from rice used in Bengali cooking for thickening gravies and making sweets.", IsAllergen = false, ImageUrl = "https://example.com/images/rice_flour.jpg" },
                    new Ingredient { Name = "Salt", Description = "A basic seasoning used in almost every Bengali dish for flavoring.", IsAllergen = false, ImageUrl = "https://example.com/images/salt.jpg" },
                    new Ingredient { Name = "Sugar", Description = "Used in both savory and sweet Bengali dishes, especially in sweets like rasgulla.", IsAllergen = false, ImageUrl = "https://example.com/images/sugar.jpg" },


                    new Ingredient { Name = "Hilsa Fish", Description = "A delicate and oily fish, often considered the king of Bengali fish, used in various Bengali fish dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/hilsa_fish.jpg" },
                    new Ingredient { Name = "Chili Powder", Description = "Ground dried chilies used for adding heat and color to Bengali dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/chili_powder.jpg" },
                    new Ingredient { Name = "Ghee", Description = "Clarified butter used in Bengali cuisine for flavoring rice, dals, and sweets.", IsAllergen = false, ImageUrl = "https://example.com/images/ghee.jpg" },
                    new Ingredient { Name = "Jaggery", Description = "Unrefined sugar made from sugarcane or palm, often used in Bengali sweets.", IsAllergen = false, ImageUrl = "https://example.com/images/jaggery.jpg" },
                    new Ingredient { Name = "Mustard Paste", Description = "A paste made from mustard seeds, used in Bengali cooking to add flavor and heat.", IsAllergen = false, ImageUrl = "https://example.com/images/mustard_paste.jpg" },
                    new Ingredient { Name = "Coconut", Description = "Used in both grated and milk form, coconut adds a unique flavor to Bengali curries and sweets.", IsAllergen = false, ImageUrl = "https://example.com/images/coconut.jpg" },
                    new Ingredient { Name = "Saffron", Description = "A highly prized spice used for coloring and flavoring in Bengali biryanis and sweets.", IsAllergen = false, ImageUrl = "https://example.com/images/saffron.jpg" },
                    new Ingredient { Name = "Garam Masala", Description = "A blend of ground spices, including cinnamon, cardamom, and cloves, used for seasoning in Bengali dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/garam_masala.jpg" },
                    new Ingredient { Name = "Coriander Powder", Description = "A ground spice made from dried coriander seeds, often used to flavor Bengali curries and dals.", IsAllergen = false, ImageUrl = "https://example.com/images/coriander_powder.jpg" },
                    new Ingredient { Name = "Fennel Seeds", Description = "Used for tempering, fennel seeds have a sweet licorice-like flavor and are used in Bengali dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/fennel_seeds.jpg" },
                    new Ingredient { Name = "Tamarind", Description = "A sour fruit used in Bengali cooking for adding tangy flavors to chutneys and curries.", IsAllergen = false, ImageUrl = "https://example.com/images/tamarind.jpg" },
                    new Ingredient { Name = "Fenugreek Seeds", Description = "Used in small quantities in Bengali cooking for their slightly bitter taste and health benefits.", IsAllergen = false, ImageUrl = "https://example.com/images/fenugreek_seeds.jpg" },
                    new Ingredient { Name = "Lemon", Description = "Used to add a tangy flavor in Bengali cooking, especially in fish dishes and dals.", IsAllergen = false, ImageUrl = "https://example.com/images/lemon.jpg" },
                    new Ingredient { Name = "Bay Leaves", Description = "Used for tempering, bay leaves add a subtle aromatic flavor to Bengali dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/bay_leaves.jpg" },


                    new Ingredient { Name = "Chicken", Description = "A common poultry meat used in various savory dishes, especially biryanis and curries.", IsAllergen = false, ImageUrl = "https://example.com/images/chicken.jpg" },
                    new Ingredient { Name = "Rice", Description = "A staple grain used as the base for many Bengali dishes, especially biryanis.", IsAllergen = false, ImageUrl = "https://example.com/images/rice.jpg" },
                    new Ingredient { Name = "Biryani Masala", Description = "A blend of aromatic spices used to flavor biryani rice, giving it a distinct taste and fragrance.", IsAllergen = false, ImageUrl = "https://example.com/images/biryani_masala.jpg" },
                    new Ingredient { Name = "Yogurt", Description = "A creamy dairy product used in marinades, gravies, and sauces.", IsAllergen = false, ImageUrl = "https://example.com/images/yogurt.jpg" },
                    new Ingredient { Name = "Ginger-Garlic Paste", Description = "A mixture of ginger and garlic, often used as a base in Indian curries and gravies.", IsAllergen = false, ImageUrl = "https://example.com/images/ginger_garlic_paste.jpg" },
                    new Ingredient { Name = "Cinnamon", Description = "A warm and aromatic spice used in Bengali cooking, particularly in biryanis and curries.", IsAllergen = false, ImageUrl = "https://example.com/images/cinnamon.jpg" },
                    new Ingredient { Name = "Cloves", Description = "A pungent spice with a warm, sweet flavor, commonly used in Bengali dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/cloves.jpg" },
                    new Ingredient { Name = "Bay Leaf", Description = "A fragrant leaf used in Bengali cooking to add a subtle aromatic flavor to dishes.", IsAllergen = false, ImageUrl = "https://example.com/images/bay_leaf.jpg" },
                    new Ingredient { Name = "Cardamom", Description = "A sweet and fragrant spice, often used in Bengali sweets and biryanis.", IsAllergen = false, ImageUrl = "https://example.com/images/cardamom.jpg" },
                    new Ingredient { Name = "Coriander Leaves", Description = "Fresh green leaves used as garnish in Bengali dishes for flavor and decoration.", IsAllergen = false, ImageUrl = "https://example.com/images/coriander_leaves.jpg" },
                    new Ingredient { Name = "Mint Leaves", Description = "A refreshing herb used in Bengali dishes, especially in biryanis and salads.", IsAllergen = false, ImageUrl = "https://example.com/images/mint_leaves.jpg" }
                };

                var newIngredients = new List<Ingredient>();

                // Check if each ingredient already exists, if not, add it to the newIngredients list
                foreach (var ingredient in ingredients)
                {
                    var ingredientExists = await _ingredientRepository.AnyAsync(i => i.Name == ingredient.Name);

                    if (!ingredientExists)
                    {
                        newIngredients.Add(ingredient);
                    }
                }

                if (!newIngredients.Any())
                {
                    return Result<int>.Fail("All ingredients already exist.", "No new ingredients to insert", ResultType.Conflict);
                }

                // Bulk insert the new ingredients
                await _ingredientRepository.AddRangeAsync(newIngredients);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newIngredients.Count, "New ingredients successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting ingredients: {ex.Message}", "", ResultType.Failure);
            }
        }

        public async Task<Result<IngredientResponse>> UpdateIngredientAsync(Guid id, IngredientRequest ingredientRequest)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return Result<IngredientResponse>.Fail("Ingredient not found", "Ingredient not found", ResultType.NotFound);

            // Check if another ingredient with the same name exists (excluding the current one)
            var existingIngredient = await _ingredientRepository.FirstOrDefaultAsync(i => i.Name == ingredientRequest.Name && i.Id != id);

            if (existingIngredient != null)
            {
                return Result<IngredientResponse>.Fail("Ingredient with the same name already exists.", "Duplicate ingredient", ResultType.Conflict);
            }

            ingredient.Name = ingredientRequest.Name ?? ingredient.Name;
            ingredient.Description = ingredientRequest.Description ?? ingredient.Description;
            ingredient.IsAllergen = ingredientRequest.IsAllergen;
            ingredient.ImageUrl = ingredientRequest.ImageUrl ?? ingredient.ImageUrl;

            _ingredientRepository.Update(ingredient);
            await _unitOfWork.SaveChangesAsync();

            var response = new IngredientResponse
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                IsAllergen = ingredient.IsAllergen,
                ImageUrl = ingredient.ImageUrl
            };

            return Result<IngredientResponse>.Ok(response, "Ingredient updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteIngredientAsync(Guid id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
                return Result<bool>.Fail("Ingredient not found", "Ingredient not found", ResultType.NotFound);

            if (!string.IsNullOrEmpty(ingredient.PublicId))
            {

                //Mark ingredient as Deleted (soft delete)
                //Commit DB transaction
                //Publish a domain event
                //Background worker deletes file
                //If file deletion fails  retry

                var fileDeleted = await _fileStorage.DeleteFileAsync(ingredient.PublicId);
                if (!fileDeleted)
                {
                    return Result<bool>.Fail("Failed to delete associated file", "File deletion failed", ResultType.Failure);
                }
            }

            _ingredientRepository.Remove(ingredient);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Ingredient deleted successfully", ResultType.Success);
        }



        //public async Task<Result<PaginatedResponse<IEnumerable<IngredientResponse>>>> GetAllIngredientsAsync(int pageNumber = 1,
        //    int pageSize = 10,
        //    string searchTerm = null!,
        //    string sortBy = "Id",
        //    string sortOrder = "ASC")
        //{
        //    // Valid sort columns
        //    var validSortColumns = new List<string> { "Id", "Name", "CreatedAt" };
        //    if (!validSortColumns.Contains(sortBy))
        //    {
        //        return Result<PaginatedResponse<IEnumerable<IngredientResponse>>>.Fail("Invalid sort column", "Invalid sort column.", ResultType.Failure);
        //    }

        //    int offset = (pageNumber - 1) * pageSize;

        //    string totalCountQuery = @"
        //                SELECT COUNT(*) AS TotalCount
        //                FROM Ingredients";

        //    int totalCount = await _unitOfWork.ExecuteScalarAsync<int>(totalCountQuery);

        //    string selectQuery = @"
        //                SELECT Id, Name, Description, IsAllergen, ImageUrl
        //                FROM Ingredients";

        //    // Handle WHERE clause if search term is provided
        //    string whereClause = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        whereClause = " WHERE (Name LIKE @SearchTerm OR Description LIKE @SearchTerm)";
        //    }

        //    string queryWithWhere = selectQuery + whereClause;

        //    sortOrder = sortOrder?.ToUpper() == "DESC" ? "DESC" : "ASC";
        //    string orderBy = $@"
        //                ORDER BY {sortBy} {sortOrder}
        //                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //    string finalQuery = queryWithWhere + orderBy;

        //    var parameters = new
        //    {
        //        Offset = offset,
        //        PageSize = pageSize,
        //        SearchTerm = "%" + searchTerm + "%"
        //    };

        //    var ingredients = await _unitOfWork.QueryAsync<IngredientResponse>(finalQuery, parameters);

        //    var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
        //    var currentPageCount = ingredients.Count();
        //    paginationMeta.CurrentPageCount = currentPageCount;

        //    var response = new PaginatedResponse<IEnumerable<IngredientResponse>>
        //    {
        //        MetaData = paginationMeta,
        //        Data = ingredients
        //    };

        //    return Result<PaginatedResponse<IEnumerable<IngredientResponse>>>.Ok(response, "Ingredients retrieved successfully", ResultType.Success);
        //}

    }
}
