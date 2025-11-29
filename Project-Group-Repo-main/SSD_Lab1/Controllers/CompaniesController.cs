using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Data;
using SSD_Lab1.Models;

namespace SSD_Lab1.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        // In CompaniesController
        [Authorize(Roles = "Supervisor, Employee")]
        [ResponseCache(Duration = 30)] // Cache for 30 seconds
        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .AsNoTracking() // Improve read performance
                .ToListAsync();
            return View(companies);
        }

        // GET: Companies/Details/5
        [Authorize(Roles = "Supervisor, Employee")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Supervisor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> Create([Bind("Name,YearsInBusiness,Website,Province")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.Id = Guid.NewGuid();
                company.CreatedAt = DateTime.UtcNow;
                company.CreatedBy = User.Identity?.Name ?? "System";
                company.IsDeleted = false;

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,YearsInBusiness,Website,Province")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCompany = await _context.Companies.FindAsync(id);
                    if (existingCompany == null)
                    {
                        return NotFound();
                    }

                    // Update only the allowed fields
                    existingCompany.Name = company.Name;
                    existingCompany.YearsInBusiness = company.YearsInBusiness;
                    existingCompany.Website = company.Website;
                    existingCompany.Province = company.Province;
                    existingCompany.ModifiedAt = DateTime.UtcNow;
                    existingCompany.ModifiedBy = User.Identity?.Name ?? "System";

                    _context.Update(existingCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                company.IsDeleted = true;
                company.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(Guid id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
