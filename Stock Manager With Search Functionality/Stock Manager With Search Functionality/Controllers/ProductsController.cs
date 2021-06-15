using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stock_Manager_With_Search_Functionality.Data;
using Stock_Manager_With_Search_Functionality.Helpers;
using Stock_Manager_With_Search_Functionality.Models;

namespace Stock_Manager_With_Search_Functionality.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Stock_Manager_With_Search_FunctionalityContext _context;

        public ProductsController(Stock_Manager_With_Search_FunctionalityContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            IEnumerable<Product> products = await _context.Product.ToListAsync();
            // Microsoft Docs solution to sorting
            // Not the most elegant solution but it gets the job done.
            // https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-5.0 
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CataSortParam"] = sortOrder == "Category" ? "cata_desc" : "Category";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";

            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString)
                                       || s.Category.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(product => product.Name);
                    break;
                case "Price":
                    products = products.OrderBy(product => product.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(product => product.Price);
                    break;
                case "Category":
                    products = products.OrderBy(product => product.Category);
                    break;
               case "cata_desc":
                    products = products.OrderByDescending(product => product.Category);
                    break;
                default:
                    products = products.OrderBy(product => product.Name);
                    break;
            }

            // List of suppliers (Assuming suppliers has low sparsity)
            // Alternatively use memcache request first
            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-5.0

            List<Supplier> suppliers = await _context.Supplier.ToListAsync();
            Dictionary<int, string> supplierDictionary = new();
            
            foreach (Supplier supplier in suppliers)
            {
                supplierDictionary.Add(supplier.Id, supplier.Name);
            }
            ViewBag.supplierDictionary = supplierDictionary;

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var suppliers = _context.Supplier.ToList();
            ViewBag.Suppliers = suppliers;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Catagory,Price,Supplier")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Price,Supplier")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
