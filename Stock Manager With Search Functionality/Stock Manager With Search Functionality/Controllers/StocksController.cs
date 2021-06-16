using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stock_Manager_With_Search_Functionality.Data;
using Stock_Manager_With_Search_Functionality.Models;
using Stock_Manager_With_Search_Functionality.Services;
using Stock_Manager_With_Search_Functionality.ViewModels;
using static Stock_Manager_With_Search_Functionality.Services.CacheService;

namespace Stock_Manager_With_Search_Functionality.Controllers
{
    public class StocksController : Controller
    {
        private readonly Stock_Manager_With_Search_FunctionalityContext _context;
        private readonly CacheService _cacheService;
        public StocksController(Stock_Manager_With_Search_FunctionalityContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // GET: Stocks Suppliers Products
        public async Task<IActionResult> Index(string SearchString, string supplier)
        {
            
            IEnumerable<StockManage> stockManageList = await GetAllStock(SearchString, supplier);
            IEnumerable<Supplier> suppliers = await _cacheService.SupplierCache(CacheServiceOptionPreset.Default);

            ViewBag.supplierList = suppliers;

            return View(stockManageList);
        }

        public async Task<IEnumerable<StockManage>> GetAllStock(string searchString, string supplier)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["SupplierFilter"] = supplier;

            IEnumerable<StockManage> stockManageList = await _context.Stock.Join(
            _context.Product,
            product => product.ProductID,
            stock => stock.Id,
            (stock, product) => new StockManage
            {
                ProductCatagory = product.Category,
                ProductID = product.Id,
                ProductName = product.Name,
                ProductPrice = product.Price,
                SupplierID = product.Supplier,
                StockID = stock.Id,
                StockQuantity = stock.Quantity,
                DateUpdated = stock.DateUpdated,
            }
         ).ToArrayAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                stockManageList = stockManageList.Where(s => s.ProductName.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(supplier))
            {
                int supplierID = -1;
                if (int.TryParse(supplier, out supplierID))
                {
                    stockManageList = stockManageList.Where(s => s.SupplierID == Convert.ToInt32(supplierID));
                }
                
            }



            return stockManageList;
        }

        [HttpPost]
        public async Task<IActionResult> Export(string searchString, string supplier)
        {
            // Tutorial demonstrates clear way to export files to client.
            // However exports are determinstic on search params,
            // the data shown in view should be the data exported as csv
            // https://www.aspforums.net/Threads/204925/Export-HTML-Table-to-CSV-file-in-ASPNet-Core-MVC/
            ViewData["CurrentFilter"] = searchString;
            ViewData["SupplierFilter"] = supplier;

            IEnumerable<StockManage> stocks = await GetAllStock(searchString, supplier);
            StringBuilder stringBuilder = new();
            PropertyInfo[] propInfos = new StockManage().GetType().GetProperties();

            foreach (PropertyInfo propInfo in propInfos)
            {
                stringBuilder.Append($"{propInfo.Name},");
            }

            stringBuilder.AppendLine();

            for (int i = 0; i < stocks.Count(); i++)
            {
                StockManage stock = stocks.ElementAt(i);

                foreach (PropertyInfo prop in stock.GetType().GetProperties())
                {
                    stringBuilder.Append($"{prop.GetValue(stock)},");
                }
                stringBuilder.AppendLine();

            }
            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", $"Stock ({DateTime.Now}).csv");
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stock
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // Generic Search properties
            // Richer search tools structured by property and type

            // Contains is an not an effective search option
            // Alternative: 
            //      -> Split array,
            //      -> Intersection of individual values O(n^2),
            //      -> Condition a cost to match

            if (!String.IsNullOrEmpty(searchString))
            {
                IEnumerable<Product> products = _context.Product;
                products = products.Where(s => s.Name.Contains(searchString)
                                        || s.Category.Contains(searchString));
                ViewBag.productResults = products;
            }



            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductID,Quantity")] Stock stock)
        {
            stock.DateCreated = DateTime.Now;
            stock.DateUpdated = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stock.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductID,Quantity,DateCreated")] Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    stock.DateUpdated = DateTime.Now;
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.Id))
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
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stock
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stock.FindAsync(id);
            _context.Stock.Remove(stock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stock.Any(e => e.Id == id);
        }
    }
}
