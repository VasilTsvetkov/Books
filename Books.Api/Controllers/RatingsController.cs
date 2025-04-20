using Books.Api.Auth;
using Books.Application.Services;
using Books.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{

	[ApiController]
	public class RatingsController(IRatingService ratingService) : ControllerBase
	{
		[Authorize]
		[HttpPut(ApiEndpoints.Books.Rate)]
		public async Task<IActionResult> RateBook([FromRoute] Guid id, [FromBody] RateBookRequest request, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var result = await ratingService.RateBookAsync(id, request.Rating, userId!.Value, token);
			return result ? Ok() : NotFound();
		}

		[Authorize]
		[HttpDelete(ApiEndpoints.Books.DeleteRating)]
		public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
		{
			var userId = HttpContext.GetUserId();
			var result = await ratingService.DeleteRatingAsync(id, userId!.Value, token);
			return result ? Ok() : NotFound();
		}
	}
}