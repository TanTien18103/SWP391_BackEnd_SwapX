using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.RatingRepo
{
    public class RatingRepo: IRatingRepo
    {
        private readonly SwapXContext _context;
        public RatingRepo(SwapXContext context)
        {
            _context = context;
        }


        public async Task<Rating> AddRating(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            await UpdateStationAverageRating(rating.StationId);
            return rating;
        }

        public async Task<List<Rating>> GetAllRatings()
        {
            return await _context.Ratings.Include(a => a.Station).Include(b => b.Account).ToListAsync();
        }

        public async Task<List<Rating>> GetRatingByAccountIdAndStationId(string accountId, string stationId)
        {
            return await _context.Ratings.Where(a=>a.Account.AccountId==accountId).Where(b=>b.Station.StationId==stationId).Where(c=>c.Status==RatingStatusEnums.Active.ToString()).ToListAsync();
        }

        public async Task<Rating> GetRatingById(string ratingId)
        {
            return await _context.Ratings.Include(a=>a.Station).Include(b=>b.Account).FirstOrDefaultAsync(r => r.RatingId == ratingId);
        }

        public async Task<Rating> UpdateRating(Rating rating)
        {   
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
            await UpdateStationAverageRating(rating.StationId);
            return rating;
        }

        public async Task UpdateStationAverageRating(string stationId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.StationId == stationId && r.Rating1 != null&&r.Status==RatingStatusEnums.Active.ToString())
                .ToListAsync();

            decimal avg = 0m;
            if (ratings.Count > 0)
            {
                avg = ratings.Average(r => r.Rating1.Value);
            }

            var station = await _context.Stations.FirstOrDefaultAsync(s => s.StationId == stationId);
            if (station != null)
            {
                station.Rating = avg;
                await _context.SaveChangesAsync();
            }
        }


    }
}
