using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class MealIngredientService : IMealIngredientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MealIngredientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<MealIngredientResponse>>> GetAllMealIngredientsAsync()
        {
            var mealIngredients = await _unitOfWork.Repository<MealIngredient>().GetAllAsync();
            var response = mealIngredients.Select(mealIngredient => new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                IngredientId = mealIngredient.IngredientId,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId
            });
            return Result<IEnumerable<MealIngredientResponse>>.Ok(response, "Meal ingredients retrieved successfully", ResultType.Success);
        }

        public async Task<Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>> GetAllMealIngredientsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!)
        {
            int offset = (pageNumber - 1) * pageSize;
            //string sqlQuery = @"
            //    SELECT COUNT(*) OVER () AS TotalCount, 
            //           mi.Id, mi.MealId, mi.IngredientId, mi.Quantity, mi.UnitId,
            //           i.Name AS IngredientName,
            //           u.Name AS UnitName,
            //           m.Name AS MealName
            //    FROM MealIngredients mi
            //    INNER JOIN Ingredients i ON mi.IngredientId = i.Id
            //    INNER JOIN Units u ON mi.UnitId = u.Id
            //    INNER JOIN Meals m ON mi.MealId = m.Id
            //    ORDER BY mi.Id
            //    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            string selectQuery = @"
                SELECT COUNT(*) OVER () AS TotalCount, 
                       mi.Id, mi.MealId, mi.IngredientId, mi.Quantity, mi.UnitId,
                       i.Name AS IngredientName,
                       u.Name AS UnitName,
                       m.Name AS MealName
                FROM MealIngredients mi
                INNER JOIN Ingredients i ON mi.IngredientId = i.Id
                INNER JOIN Units u ON mi.UnitId = u.Id
                INNER JOIN Meals m ON mi.MealId = m.Id";
            string whereClause = string.Empty;
            string orderBy = @"
                ORDER BY mi.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            // Add WHERE clause if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                whereClause = " WHERE (m.Name LIKE @SearchTerm OR i.Name LIKE @SearchTerm)";
            }

            string sqlQuery = selectQuery + whereClause + orderBy;

            var parameters = new { 
                Offset = offset, 
                PageSize = pageSize,
                SearchTerm = "%" + searchTerm + "%"
            };
            var result = await _unitOfWork.QueryAsync<MealIngredientPaginationResult>(sqlQuery, parameters);
            int totalCount = result.FirstOrDefault()?.TotalCount ?? 0;

            var mealIngredients = result.Select(mealIngredient => new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                MealName = mealIngredient.MealName,
                IngredientId = mealIngredient.IngredientId,
                IngredientName = mealIngredient.IngredientName,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId,
                UnitName = mealIngredient.UnitName
            }).ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            var currentPageCount = mealIngredients.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<MealIngredientResponse>>
            {
                MetaData = paginationMeta,
                Data = mealIngredients
            };

            return Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>.Ok(response, "Meal ingredients retrieved successfully", ResultType.Success);
        }



        public async Task<Result<MealIngredientResponse>> GetMealIngredientByIdAsync(Guid id)
        {
            var mealIngredient = await _unitOfWork.Repository<MealIngredient>().GetByIdAsync(id);
            if (mealIngredient == null)
                return Result<MealIngredientResponse>.Fail("Meal Ingredient not found", "Meal Ingredient not found", ResultType.NotFound);

            var response = new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                IngredientId = mealIngredient.IngredientId,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId
            };

            return Result<MealIngredientResponse>.Ok(response, "Meal Ingredient retrieved successfully", ResultType.Success);
        }

        public async Task<Result<MealIngredientResponse>> CreateMealIngredientAsync(MealIngredientRequest mealIngredientRequest)
        {
            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(mealIngredientRequest.MealId);
            if (meal == null)
            {
                return Result<MealIngredientResponse>.Fail("","Meal not found", ResultType.NotFound);
            }

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(mealIngredientRequest.IngredientId);
            if (ingredient == null)
            {
                return Result<MealIngredientResponse>.Fail("","Ingredient not found", ResultType.NotFound);
            }

            var unit = await _unitOfWork.Repository<Units>().GetByIdAsync(mealIngredientRequest.UnitId);
            if (unit == null)
            {
                return Result<MealIngredientResponse>.Fail("","Unit not found", ResultType.NotFound);
            }

            var mealIngredient = new MealIngredient
            {
                MealId = mealIngredientRequest.MealId,
                IngredientId = mealIngredientRequest.IngredientId,
                Quantity = mealIngredientRequest.Quantity,
                UnitId = mealIngredientRequest.UnitId
            };

            await _unitOfWork.Repository<MealIngredient>().AddAsync(mealIngredient);
            await _unitOfWork.SaveChangesAsync();

            var response = new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                IngredientId = mealIngredient.IngredientId,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId
            };

            return Result<MealIngredientResponse>.Ok(response, "Meal Ingredient created successfully", ResultType.Success);
        }


        public async Task<Result<int>> BulkInsertPredefinedMealIngredientsAsync()
        {
            try
            {
                // Predefined MealIngredient data (with MealName, IngredientName, UnitName, Quantity)
                var mealIngredients = new List<MealIngredientBulkRequest>
                {
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Chicken", Quantity = 350, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Rice", Quantity = 200, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Onion", Quantity = 100, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Yogurt", Quantity = 50, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Ginger-Garlic Paste", Quantity = 30, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Biryani Masala", Quantity = 15, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Ghee", Quantity = 30, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Cinnamon", Quantity = 2, UnitName = "Piece" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Cloves", Quantity = 4, UnitName = "Piece" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Bay Leaf", Quantity = 1, UnitName = "Piece" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Cardamom", Quantity = 2, UnitName = "Piece" },
                    //new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Saffron", Quantity = 1, UnitName = "Pinch" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Coriander Leaves", Quantity = 30, UnitName = "Gram" },
                    new MealIngredientBulkRequest { MealName = "Chicken Biryani", IngredientName = "Mint Leaves", Quantity = 30, UnitName = "Gram" }
                };

                var newMealIngredients = new List<MealIngredient>();

                foreach (var request in mealIngredients)
                {
                    var meal = await _unitOfWork.Repository<Meal>().FirstOrDefaultAsync(m => m.Name == request.MealName);
                    var ingredient = await _unitOfWork.Repository<Ingredient>().FirstOrDefaultAsync(i => i.Name == request.IngredientName);
                    var unit = await _unitOfWork.Repository<Units>().FirstOrDefaultAsync(u => u.Name == request.UnitName);

                    // Check if all entities exist
                    if (meal == null || ingredient == null || unit == null)
                    {
                        return Result<int>.Fail("Meal, Ingredient, or Unit not found.", "One or more entities are invalid", ResultType.NotFound);
                    }

                    // Check if the MealIngredient already exists in the DB
                    var existingMealIngredient = await _unitOfWork.Repository<MealIngredient>().FirstOrDefaultAsync(
                        mi => mi.MealId == meal.Id && mi.IngredientId == ingredient.Id && mi.UnitId == unit.Id);

                    if (existingMealIngredient == null)
                    {
                        // Create new MealIngredient
                        var mealIngredient = new MealIngredient
                        {
                            MealId = meal.Id,
                            IngredientId = ingredient.Id,
                            Quantity = request.Quantity,
                            UnitId = unit.Id
                        };

                        newMealIngredients.Add(mealIngredient);
                    }
                }

                // Check if there are new meal ingredients to insert
                if (!newMealIngredients.Any())
                {
                    return Result<int>.Fail("No new meal ingredients to insert.", "All provided meal ingredients already exist", ResultType.Conflict);
                }

                // Insert the new MealIngredients
                await _unitOfWork.Repository<MealIngredient>().AddRangeAsync(newMealIngredients);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newMealIngredients.Count, "Meal ingredients successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting meal ingredients: {ex.Message}", "", ResultType.Failure);
            }
        }


        public async Task<Result<MealIngredientResponse>> UpdateMealIngredientAsync(Guid id, MealIngredientRequest mealIngredientRequest)
        {
            var mealIngredient = await _unitOfWork.Repository<MealIngredient>().GetByIdAsync(id);
            if (mealIngredient == null)
                return Result<MealIngredientResponse>.Fail("Meal Ingredient not found", "Meal Ingredient not found", ResultType.NotFound);

            var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(mealIngredientRequest.MealId);
            if (meal == null)
            {
                return Result<MealIngredientResponse>.Fail("Meal not found", "Meal not found", ResultType.NotFound);
            }

            var ingredient = await _unitOfWork.Repository<Ingredient>().GetByIdAsync(mealIngredientRequest.IngredientId);
            if (ingredient == null)
            {
                return Result<MealIngredientResponse>.Fail("Ingredient not found", "Ingredient not found", ResultType.NotFound);
            }

            var unit = await _unitOfWork.Repository<Units>().GetByIdAsync(mealIngredientRequest.UnitId);
            if (unit == null)
            {
                return Result<MealIngredientResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            mealIngredient.MealId = mealIngredientRequest.MealId;
            mealIngredient.IngredientId = mealIngredientRequest.IngredientId;
            mealIngredient.Quantity = mealIngredientRequest.Quantity;
            mealIngredient.UnitId = mealIngredientRequest.UnitId;

            _unitOfWork.Repository<MealIngredient>().Update(mealIngredient);
            await _unitOfWork.SaveChangesAsync();

            var response = new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                IngredientId = mealIngredient.IngredientId,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId
            };


            return Result<MealIngredientResponse>.Ok(response, "Meal Ingredient updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteMealIngredientAsync(Guid id)
        {
            var mealIngredient = await _unitOfWork.Repository<MealIngredient>().GetByIdAsync(id);
            if (mealIngredient == null)
                return Result<bool>.Fail("Meal Ingredient not found", "Meal Ingredient not found", ResultType.NotFound);

            _unitOfWork.Repository<MealIngredient>().Remove(mealIngredient);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Meal Ingredient deleted successfully", ResultType.Success);
        }
    }
}
