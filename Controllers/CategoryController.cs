using KnowIT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KnowIT.Controllers
{
	public class CategoryController : Controller
	{
		private readonly KnowledgeDbContext _context;
		private readonly ILogger<CategoryController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

		public CategoryController(ILogger<CategoryController> logger, KnowledgeDbContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_logger = logger;
            _userManager = userManager;
		}

        // Helper method to check if the user is an admin
        private async Task<bool> IsUserAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        // GET: Categories
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? selectedCategoryId)
		{
            var categories = await _context.Categories.ToListAsync();
            var articles = await _context.Articles.ToListAsync();

            // Return as a tuple to match the expected model in the Index view
            var model = new Tuple<IEnumerable<Category>, IEnumerable<Article>>(categories, articles);

            ViewBag.SelectedCategoryId = selectedCategoryId;

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ShowArticles(int? id)
{
    var allCategories = await _context.Categories.ToListAsync();
    IEnumerable<Article> articles;

    // Fetch the articles based on the category ID, including "No Category"
    if (id == null || id == 0)
    {
        articles = await _context.Articles.Where(a => a.CategoryID == null).ToListAsync();
    }
    else
    {
        articles = await _context.Articles.Where(a => a.CategoryID == id).ToListAsync();
    }

    // Set the selected category ID in ViewBag to apply the highlight in the view
    ViewBag.SelectedCategoryId = id ?? 0;

    var model = new Tuple<IEnumerable<Category>, IEnumerable<Article>>(allCategories, articles);
    return View("Index", model); // Pass the model to the shared view
}


        // GET: Category/Manage
        public async Task<IActionResult> Manage()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }


        // GET: Category/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
		{
			return View();
		}

        // POST: Category/Create
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Edit
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the category in the database
                    _context.Categories.Update(category);

                    // Save the changes to the category
                    await _context.SaveChangesAsync();

                    var updatedArticles = await _context.Articles
                        .Where(a => a.CategoryID == category.Id)
                        .ToListAsync();
                                                          
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the index page or any other page after successfully updating
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
