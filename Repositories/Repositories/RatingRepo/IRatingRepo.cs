using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.RatingRepo
{
    public interface IRatingRepo
    {
        Task<Rating> GetRatingById(string ratingId);
        Task<List<Rating>> GetAllRatings();
        Task<Rating> AddRating(Rating rating);
        Task<Rating> UpdateRating(Rating rating);
        Task <List<Rating>> GetRatingByAccountIdAndStationId(string accountId, string stationId);
        Task<List<Rating>> GetAllRatingByStationId(string stationId);
    }
}
