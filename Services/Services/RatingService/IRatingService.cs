using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.Rating;

namespace Services.Services.RatingService
{
    public interface IRatingService
    {
        Task<ResultModel> GetRatingById(string ratingId);
        Task<ResultModel> GetAllRatings();
        Task<ResultModel> AddRating(AddRatingRequest addRatingRequest);
        Task<ResultModel> UpdateRating(UpdateRatingRequest updateRatingRequest);
        Task<ResultModel> DeleteRating(string ratingId);
        Task<ResultModel>DeleteRatingForCustomerByRatingId(DeleteRatingForCustomerByRatingId deleteRatingForCustomerByRatingId);
    }
}
