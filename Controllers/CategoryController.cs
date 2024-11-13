using KnowIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KnowIT.Controllers
{
	public class CategoryController : Controller
	{
		private readonly KnowledgeDbContext _context;
		private readonly ILogger<CategoryController> _logger;

		public CategoryController(ILogger<CategoryController> logger, KnowledgeDbContext context)
		{
			_context = context;
			_logger = logger;
		}

		// GET: Categories
		public async Task<IActionResult> Index(int? selectedCategoryId)
		{
            var categories = await _context.Categories.ToListAsync();
            var articles = await _context.Articles.ToListAsync();

            // Return as a tuple to match the expected model in the Index view
            var model = new Tuple<IEnumerable<Category>, IEnumerable<Article>>(categories, articles);

            ViewBag.SelectedCategoryId = selectedCategoryId;

            return View(model);
        }

        public async Task<IActionResult> ShowArticles(int? id)
        {
            // Retrieve all categories, including a "No Category" option.
            var allCategories = await _context.Categories.ToListAsync();

            // If id is null or 0, fetch articles without a category.
            IEnumerable<Article> articles;
            if (id == null || id == 0)
            {
                articles = await _context.Articles
                    .Where(a => a.CategoryID == null)
                    .ToListAsync();
            }
            else
            {
                // Fetch articles with the specified category ID
                articles = await _context.Articles
                    .Where(a => a.CategoryID == id)
                    .ToListAsync();
            }

            // Pass all categories and filtered articles to the view
            var model = new Tuple<IEnumerable<Category>, IEnumerable<Article>>(allCategories, articles);
            return View("Index", model); // Use the same Index view
        }

        // GET: Category/Manage
        public async Task<IActionResult> Manage()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }


        // GET: Category/Create
        public IActionResult Create()
		{
			return View();
		}

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                // Add the category to the database
                _context.Add(category);
                await _context.SaveChangesAsync();

                // Redirect to the Index view after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, return the view to display errors
            return View(category);
        }

        // GET: Category/Edit
        public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

        // POST: Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Check for errors in the ModelState
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);  // You can log these errors or use a debugger
                }

                // If the ModelState is invalid, return the view with the current category model
                return View(category);
            }

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
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

        public IActionResult Cancel()
        {
            // Fetch both categories and articles to pass to the shared index view.
            var categories = _context.Categories.ToList();
            var articles = _context.Articles.ToList();

            // Create the Tuple
            var model = new Tuple<IEnumerable<Category>, IEnumerable<Article>>(categories, articles);

            return View("Index", model);
        }

        // GET: Category/Delete
        public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories
				.FirstOrDefaultAsync(m => m.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}

        // POST: Category/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the category to be deleted
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                // Find all articles with this category
                var associatedArticles = await _context.Articles
                    .Where(a => a.CategoryID == id)
                    .ToListAsync();

                // Set CategoryID to null for each associated article
                foreach (var article in associatedArticles)
                {
                    article.CategoryID = null;
                }

                // Save the updated articles before deleting the category
                await _context.SaveChangesAsync();

                // Remove the category
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            // Redirect to the Index view after deletion
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
		{
			return _context.Categories.Any(e => e.Id == id);
		}
	}
}
