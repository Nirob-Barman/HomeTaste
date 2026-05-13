//using HomeTaste.Application.DTOs.MealManagement;
//using HomeTaste.Application.DTOs.Test.Ingredients;
//using HomeTaste.Application.DTOs.Test.MealAndMealCategory;
//using HomeTaste.Application.DTOs.Test.UnitAndMealCategory;
//using HomeTaste.Application.Interfaces.MealManagement;
//using HomeTaste.Application.Interfaces.Persistence;
//using HomeTaste.Application.Interfaces.Test;
//using HomeTaste.Application.Interfaces.Measurements;
//using HomeTaste.Application.Wrappers;
//using HomeTaste.Application.DTOs.File;

//namespace HomeTaste.Application.Services.Test
//{
//    public class TransactionTestService : ITransactionTestService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IUnitService _unitService;
//        private readonly IMealCategoryService _mealCategoryService;
//        private readonly IMealService _mealService;
//        private readonly IIngredientService _ingredientService;

//        public TransactionTestService(IUnitOfWork unitOfWork, IUnitService unitService, IMealCategoryService mealCategoryService, IMealService mealService, IIngredientService ingredientService)
//        {
//            _unitOfWork = unitOfWork;
//            _unitService = unitService;
//            _mealCategoryService = mealCategoryService;
//            _mealService = mealService;
//            _ingredientService = ingredientService;
//        }

//        public async Task<Result<UnitAndMealCategoryResponse>> CreateUnitAndMealCategoryAsync(UnitAndMealCategoryRequest request)
//        {
//            // Begin a transaction at the UnitOfWork level
//            await _unitOfWork.BeginTransaction();

//            try
//            {
//                // First, create the unit and save changes
//                var unitResponse = await _unitService.CreateUnitAsync(request.Unit!);
//                if (!unitResponse.Success)
//                {
//                    return Result<UnitAndMealCategoryResponse>.Fail(unitResponse.Errors!, unitResponse.Message, ResultType.Failure);
//                }

//                // Next, create the meal and save changes
//                var mealCategoryResponse = await _mealCategoryService.CreateMealCategoryAsync(request.MealCategory!);
//                if (!mealCategoryResponse.Success)
//                {
//                    return Result<UnitAndMealCategoryResponse>.Fail(mealCategoryResponse.Errors!, mealCategoryResponse.Message, ResultType.Failure);
//                }

//                // If both operations succeed, commit the transaction
//                await _unitOfWork.CommitAsync();

//                return Result<UnitAndMealCategoryResponse>.Ok(new UnitAndMealCategoryResponse
//                {
//                    Unit = unitResponse.Data,
//                    MealCategory = mealCategoryResponse.Data
//                }, "Unit and Meal Category created successfully", ResultType.Success);
//            }
//            catch (Exception ex)
//            {
//                await _unitOfWork.RollbackAsync();
//                return Result<UnitAndMealCategoryResponse>.Fail($"An error occurred: {ex.Message}", "", ResultType.Failure);
//            }
//        }

//        public async Task<Result<UnitAndIngredientsRespone>> CreateUnitAndIngredientAsync(UnitAndIngredientsRequest request)
//        {
//            // Begin a transaction at the UnitOfWork level
//            await _unitOfWork.BeginTransaction();

//            try
//            {
//                // First, create the unit and save changes
//                var unitResponse = await _unitService.CreateUnitAsync(request.Unit!);
//                if (!unitResponse.Success)
//                {
//                    return Result<UnitAndIngredientsRespone>.Fail(unitResponse.Errors!, unitResponse.Message, ResultType.Failure);
//                }

//                FileUploadDto file = null!;

//                // Next, create the ingredient and save changes
//                var ingredientResponse = await _ingredientService.CreateIngredientAsync(request.Ingredient!, file);
//                if (!ingredientResponse.Success)
//                {
//                    return Result<UnitAndIngredientsRespone>.Fail(ingredientResponse.Errors!, ingredientResponse.Message, ResultType.Failure);
//                }

//                // If both operations succeed, commit the transaction
//                await _unitOfWork.CommitAsync();

//                return Result<UnitAndIngredientsRespone>.Ok(new UnitAndIngredientsRespone
//                {
//                    Unit = unitResponse.Data,
//                    Ingredient = ingredientResponse.Data
//                }, "Unit and Ingredient created successfully", ResultType.Success);
//            }
//            catch (Exception ex)
//            {
//                await _unitOfWork.RollbackAsync();
//                return Result<UnitAndIngredientsRespone>.Fail($"An error occurred: {ex.Message}", "", ResultType.Failure);
//            }
//        }



//        //public async Task<Result<MealAndMealCategoryResponse>> CreateMealAndMealCategoryAsync(MealAndMealCategoryRequest request)
//        //{
//        //    // Begin a transaction at the UnitOfWork level
//        //    await _unitOfWork.BeginTransaction();

//        //    try
//        //    {
//        //        // First, create the meal category
//        //        var mealCategoryResponse = await _mealCategoryService.CreateMealCategoryAsync(request.MealCategory!);
//        //        if (!mealCategoryResponse.Success)
//        //        {
//        //            return Result<MealAndMealCategoryResponse>.Fail(mealCategoryResponse.Errors!, mealCategoryResponse.Message, ResultType.Failure);
//        //        }

//        //        // Next, create the meal and link it to the newly created category
//        //        var mealResponse = await _mealService.CreateMealAsync(new MealRequest
//        //        {
//        //            Name = request.Meal!.Name,
//        //            Description = request.Meal.Description,
//        //            Price = request.Meal.Price,
//        //            CategoryId = mealCategoryResponse.Data!.Id, // Link the newly created Meal Category ID
//        //            ImageUrl = request.Meal.ImageUrl
//        //        });

//        //        if (!mealResponse.Success)
//        //        {
//        //            return Result<MealAndMealCategoryResponse>.Fail(mealResponse.Errors!, mealResponse.Message, ResultType.Failure);
//        //        }

//        //        // If both operations succeed, commit the transaction
//        //        await _unitOfWork.CommitAsync();

//        //        // Return the response with both Meal and MealCategory data
//        //        return Result<MealAndMealCategoryResponse>.Ok(new MealAndMealCategoryResponse
//        //        {
//        //            MealCategory = mealCategoryResponse.Data,
//        //            Meal = mealResponse.Data
//        //        }, "Meal and Meal Category created successfully", ResultType.Success);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        await _unitOfWork.RollbackAsync();
//        //        return Result<MealAndMealCategoryResponse>.Fail($"An error occurred: {ex.Message}", "", ResultType.Failure);
//        //    }
//        //}


//        public async Task<Result<bool>> CreateUnitAndMealAsync(UnitAndIngredientsRequest unitAndIngredientsRequest, MealAndMealCategoryRequest mealAndMealCategoryRequest)
//        {
//            // Begin a transaction at the UnitOfWork level
//            await _unitOfWork.BeginTransaction();

//            try
//            {
//                // First, create the unit and ingredient (using the same UnitOfWork transaction)
//                var unitAndIngredientResponse = await CreateUnitAndIngredientAsync(unitAndIngredientsRequest);
//                if (!unitAndIngredientResponse.Success)
//                {
//                    await _unitOfWork.RollbackAsync();
//                    return Result<bool>.Fail(unitAndIngredientResponse.Errors[0], unitAndIngredientResponse.Message, ResultType.Failure);
//                }

//                // Next, create the meal and meal category (using the same UnitOfWork transaction)
//                var mealAndMealCategoryResponse = await CreateMealAndMealCategoryAsync(mealAndMealCategoryRequest);
//                if (!mealAndMealCategoryResponse.Success)
//                {
//                    await _unitOfWork.RollbackAsync();
//                    return Result<bool>.Fail(mealAndMealCategoryResponse.Errors[0], mealAndMealCategoryResponse.Message, ResultType.Failure);
//                }

//                // If both operations succeed, commit the transaction
//                await _unitOfWork.CommitAsync();

//                return Result<bool>.Ok(true, "Unit, Ingredient, Meal, and Meal Category created successfully.");
//            }
//            catch (Exception ex)
//            {
//                // If any error occurs, rollback the transaction
//                await _unitOfWork.RollbackAsync();
//                return Result<bool>.Fail("Error", $"An error occurred: {ex.Message}", ResultType.Failure);
//            }
//        }


//    }
//}
