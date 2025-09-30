using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.PackageRepo
{
    public class PackageRepo: IPackageRepo
    {
        private readonly SwapXContext _context;
        public PackageRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<Package> AddPackage(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }
        public async Task<List<Package>> GetAllPackages()
        {
            return await _context.Packages.Include(a=>a.Battery).ToListAsync();
        }
        public async Task<Package> GetPackageById(string packageId)
        {
            return await _context.Packages.Include(a=>a.Battery).FirstOrDefaultAsync(p => p.PackageId == packageId);
        }
        public async Task<Package> UpdatePackage(Package package)
        {
            _context.Packages.Update(package);
            await _context.SaveChangesAsync();
            return package;
        }
    }
}
