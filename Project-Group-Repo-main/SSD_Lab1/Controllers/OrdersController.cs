using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Data;
using SSD_Lab1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSD_Lab1.Controllers
{
    [Authorize(Roles = "Supervisor, Employee")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Company)
                .ToListAsync();

            return View(orders);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products
                .Include(p => p.Company)       
                .ToList();
            return View();
        }



        // POST: Orders/Create


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            // FIRST: Initialize order properties BEFORE validation
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.Now;
            order.Status = OrderStatus.Pending;
            order.OrderNumber = GenerateOrderNumber();

            // Clear any existing order items to avoid conflicts
            order.OrderItems = new List<OrderItem>();

            // Get form data manually
            var productIds = Request.Form["productIds"].ToList();
            var quantities = Request.Form["quantities"].ToList();

            // Debug output - check what we're receiving
            System.Diagnostics.Debug.WriteLine($"=== FORM DATA DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"Product IDs received: {productIds.Count}");
            System.Diagnostics.Debug.WriteLine($"Quantities received: {quantities.Count}");

            // Filter out empty entries
            var validProductIds = new List<Guid>();
            var validQuantities = new List<int>();

            for (int i = 0; i < productIds.Count; i++)
            {
                if (!string.IsNullOrEmpty(productIds[i]) && !string.IsNullOrEmpty(quantities[i]))
                {
                    if (Guid.TryParse(productIds[i], out Guid productId) && int.TryParse(quantities[i], out int quantity))
                    {
                        validProductIds.Add(productId);
                        validQuantities.Add(quantity);
                        System.Diagnostics.Debug.WriteLine($"Valid product: {productId}, quantity: {quantity}");
                    }
                }
            }

            if (validProductIds.Count == 0)
            {
                ModelState.AddModelError("", "You must select at least one product.");
                ViewBag.Products = _context.Products.Include(p => p.Company).ToList();
                return View(order);
            }

            decimal total = 0;
            bool hasStockIssues = false;

            for (int i = 0; i < validProductIds.Count; i++)
            {
                var product = await _context.Products.FindAsync(validProductIds[i]);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Product with ID {validProductIds[i]} not found.");
                    hasStockIssues = true;
                    continue;
                }

                if (validQuantities[i] <= 0)
                {
                    ModelState.AddModelError("", $"Quantity for {product.Name} must be greater than 0.");
                    hasStockIssues = true;
                    continue;
                }

                if (validQuantities[i] > product.StockQuantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {product.Name}. Available: {product.StockQuantity}, Requested: {validQuantities[i]}");
                    hasStockIssues = true;
                    continue;
                }

                // Calculate line total
                decimal lineTotal = product.Price * validQuantities[i];
                total += lineTotal;

                // Create order item
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = validQuantities[i],
                    UnitPrice = product.Price,
                    TotalPrice = lineTotal
                };

                order.OrderItems.Add(orderItem);

                // Reduce stock
                product.StockQuantity -= validQuantities[i];
            }

            order.TotalPrice = total;

            // NOW check ModelState - after we've set all required properties
            if (!ModelState.IsValid || hasStockIssues)
            {
                System.Diagnostics.Debug.WriteLine($"ModelState is invalid or has stock issues");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Model Error: {error.ErrorMessage}");
                }
                ViewBag.Products = _context.Products.Include(p => p.Company).ToList();
                return View(order);
            }

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"Order created successfully: {order.Id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving order: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                ModelState.AddModelError("", $"An error occurred while creating the order: {ex.Message}");
                ViewBag.Products = _context.Products.Include(p => p.Company).ToList();
                return View(order);
            }
        }

        private string GenerateOrderNumber()
        {
            // Shorter format to fit in database column
            return $"ORD-{DateTime.Now:yyMMddHHmm}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        }




        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,OrderDate,CustomerName,TotalPrice")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(Guid id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
