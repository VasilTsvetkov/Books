using Asp.Versioning;
using Books.Api.Auth;
using Books.Api.Mapping;
using Books.Application.Services;
using Books.Contracts.Requests;
using Books.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
	[ApiController]
	[ApiVersion(1.0)]
	public class RatingsController(IRatingService ratingService) : ControllerBase
	{
		[Authorize]
		[HttpPut(ApiEndpoints.Books.Rate)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> RateBook([FromRoute] Guid id, [FromBody] RateBookRequest request, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var result = await ratingService.RateBookAsync(id, request.Rating, userId!.Value, token);
			return result ? Ok() : NotFound();
		}

		[Authorize]
		[HttpDelete(ApiEndpoints.Books.DeleteRating)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var result = await ratingService.DeleteRatingAsync(id, userId!.Value, token);
			return result ? Ok() : NotFound();
		}

		[Authorize]
		[HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
		[ProducesResponseType(typeof(IEnumerable<BookRatingResponse>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetMyRatings(CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
			var ratingsResponse = ratings.MapToResponse();
			return Ok(ratingsResponse);
		}
	}
}