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
            return rating;
        }

        public Task<List<Rating>> GetAllRatingByStationId(string stationId)
        {
            return _context.Ratings.Where(r=>r.StationId==stationId).Where(r=>r.Status==RatingStatusEnums.Active.ToString()).ToListAsync();
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
            return rating;
        }
    }
}
