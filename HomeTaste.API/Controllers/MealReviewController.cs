using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealReviewController : ControllerBase
    {
        private readonly IMealReviewService _mealReviewService;
        public MealReviewController(IMealReviewService mealReviewService)
        {
            _mealReviewService = mealReviewService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(Guid id)
        {
            var result = await _mealReviewService.GetReviewByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [Authorize(Policy = Policies.CustomerOnly)]
        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewRequest request)
        {
            var result = await _mealReviewService.SubmitReviewAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpGet("meal/{id}")]
        public async Task<IActionResult> GetMealReviews(Guid id)
        {
            var result = await _mealReviewService.GetMealReviewsAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }


        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequest request)
        {
            var result = await _mealReviewService.UpdateReviewAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var result = await _mealReviewService.DeleteReviewAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [Authorize]
        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            var result = await _mealReviewService.GetMyReviewsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpGet("{id}/average-rating")]
        public async Task<IActionResult> GetAverageMealRating(Guid id)
        {
            var result = await _mealReviewService.GetAverageMealRatingAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
