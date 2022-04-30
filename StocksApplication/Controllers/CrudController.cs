using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
    public class CrudController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public CrudController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _appDbContext.Companies.ToListAsync());
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                return NotFound();
            }

            var company = await _appDbContext.Companies.FirstOrDefaultAsync(m => m.Symbol == symbol);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Date,Type,Iexid,IsEnabled")] Company company)
        {
            company.Symbol = Guid.NewGuid().ToString();
            _appDbContext.Add(company);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                return NotFound();
            }

            var company = await _appDbContext.Companies.FindAsync(symbol);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string symbol, [Bind("Symbol,Name,Date,Type,Iexid,IsEnabled")] Company company)
        {
            if (!symbol.Equals(company.Symbol, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _appDbContext.Update(company);
                    await _appDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Symbol))
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


        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                return NotFound();
            }

            var company = await _appDbContext.Companies.FirstOrDefaultAsync(m => m.Symbol == symbol);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string symbol)
        {
            var company = await _appDbContext.Companies.FindAsync(symbol);
             _appDbContext.Companies.Remove(company);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(string symbol)
        {
            return _appDbContext.Companies.Any(x => x.Symbol == symbol);
        }
    }
}
