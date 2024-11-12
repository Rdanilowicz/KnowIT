using KnowIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KnowIT.Controllers
{
    public class HomeController : Controller
    {
        private readonly KnowledgeDbContext _context;

        public HomeController(KnowledgeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredCategories = await _context.Categories.Take(3).ToListAsync(); // Example for showing 3 popular categories
            var model = new HomeViewModel
            {
                FeaturedCategories = featuredCategories
            };

            return View(model);
        }
        public class HomeViewModel
        {
            public IEnumerable<Category> FeaturedCategories { get; set; }
        }

    }
}
