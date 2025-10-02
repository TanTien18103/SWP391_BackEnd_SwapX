using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<List<Rating>> GetAllRatings()
        {
            return await _context.Ratings.Include(a => a.Station).Include(b => b.Account).ToListAsync();
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
