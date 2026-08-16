using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.MealReviews;
using HomeTaste.Application.Features.MealReviews.Commands.DeleteReview;
using HomeTaste.Application.Features.MealReviews.Commands.SubmitReview;
using HomeTaste.Application.Features.MealReviews.Commands.UpdateReview;
using HomeTaste.Application.Features.MealReviews.Queries.GetAverageMealRating;
using HomeTaste.Application.Features.MealReviews.Queries.GetMealReviews;
using HomeTaste.Application.Features.MealReviews.Queries.GetMyReviews;
using HomeTaste.Application.Features.MealReviews.Queries.GetReviewById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealReviewController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MealReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(Guid id)
        {
            var result = await _mediator.Send(new GetReviewByIdQuery(id));
            return Ok(result);
        }

        [Authorize(Policy = Policies.CustomerOnly)]
        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewRequest request)
        {
            var result = await _mediator.Send(new SubmitReviewCommand(request.MealId, request.UserId, request.Rating, request.Feedback));
            return Ok(result);
        }

        [HttpGet("meal/{id}")]
        public async Task<IActionResult> GetMealReviews(Guid id)
        {
            var result = await _mediator.Send(new GetMealReviewsQuery(id));
            return Ok(result);
        }


        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequest request)
        {
            var result = await _mediator.Send(new UpdateReviewCommand(id, request.Rating, request.Feedback));
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var result = await _mediator.Send(new DeleteReviewCommand(id));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            var result = await _mediator.Send(new GetMyReviewsQuery());
            return Ok(result);
        }

        [HttpGet("{id}/average-rating")]
        public async Task<IActionResult> GetAverageMealRating(Guid id)
        {
            var result = await _mediator.Send(new GetAverageMealRatingQuery(id));
            return Ok(result);
        }
    }
}
