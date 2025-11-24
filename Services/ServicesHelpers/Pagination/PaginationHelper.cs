using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.Pagination
{
    public static class PaginationHelper
    {
        public static PagedResult<T> Paginate<T>(
            IEnumerable<T> source,
            int pageNumber,
            int pageSize)
        {
            var count = source.Count();

            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                Items = items
            };
        }
    }
}
