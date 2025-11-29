using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Data;
using SSD_Lab1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSD_Lab1.Data
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Company> GetByIdAsync(Guid id)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task AddAsync(Company company)
        {
            company.CreatedAt = DateTime.UtcNow;
            company.CreatedBy = "System"; // You can set this to current user
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            company.ModifiedAt = DateTime.UtcNow;
            company.ModifiedBy = "System"; // You can set this to current user
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var company = await GetByIdAsync(id);
            if (company != null)
            {
                company.IsDeleted = true;
                company.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}