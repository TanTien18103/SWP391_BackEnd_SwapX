using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.RatingService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }
        [HttpPost("add_rating")]
        public async Task<IActionResult> AddRating([FromForm] Services.ApiModels.Rating.AddRatingRequest addRatingRequest)
        {
            var res = await _ratingService.AddRating(addRatingRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_rating")] 
        public async Task<IActionResult> DeleteRating([FromForm] string? ratingId)
        {
            var res = await _ratingService.DeleteRating(ratingId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_ratings")]
        public async Task<IActionResult> GetAllRatings()
        {
            var res = await _ratingService.GetAllRatings();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_rating_by_id")]
        public async Task<IActionResult> GetRatingById([FromQuery] string? ratingId)
        {
            var res = await _ratingService.GetRatingById(ratingId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_rating")]
        public async Task<IActionResult> UpdateRating([FromForm] Services.ApiModels.Rating.UpdateRatingRequest updateRatingRequest)
        {
            var res = await _ratingService.UpdateRating(updateRatingRequest);
            return StatusCode(res.StatusCode, res);
        }
    }
}
