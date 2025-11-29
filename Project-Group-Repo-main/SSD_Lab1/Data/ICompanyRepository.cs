using SSD_Lab1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSD_Lab1.Data
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> GetByIdAsync(Guid id);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Guid id);
    }
}