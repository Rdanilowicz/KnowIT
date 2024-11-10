using KnowIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;
using System.Linq;

namespace KnowIT.Controllers
{
    //[ApiController]
    public class KBController : Controller
    {
        private readonly ILogger<KBController> _logger;
        private readonly KnowledgeDbContext _context;

        public KBController(ILogger<KBController> logger, KnowledgeDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        //[HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Knowledge
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var model = Tuple.Create((IEnumerable<KnowIT.Models.Category>)categories, (IEnumerable<KnowIT.Models.Article>)articles);

            return View(model);
        }

        // GET: Create Knowledge
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            _logger.LogInformation($"Categories loaded: {categories.Count} categories");

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }


        // POST: Validation around knowledge creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CategoryID")] Article article)
        {
            // Log the received article data
            _logger.LogInformation($"Received Article: {article.Title}, {article.Content}, CategoryID: {article.CategoryID}");

            // Explicitly check if CategoryID is valid
            if (article.CategoryID == 0)
            {
                ModelState.AddModelError("CategoryID", "Please select a category.");
            }

            // Check if ModelState is valid before proceeding
            if (ModelState.IsValid)
            {
                // Explicitly set the Category navigation property based on CategoryID
                var category = await _context.Categories.FindAsync(article.CategoryID);
                if (category != null)
                {
                    article.Category = category;  // Assign the Category object to the navigation property
                }

                // Set the creation date
                article.DateCreated = DateTime.Now;

                // Add the article to the context and save
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                // Redirect to the Index view after successful creation
                return RedirectToAction("Index");
            }

            // If validation fails, populate ViewBag with categories for the dropdown
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            // Return the view with the article data (including validation errors)
            return View(article);
        }

        // GET: Knowledge/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Knowledge/Edit
        [HttpPost]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Knowledge/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        //POST: Knowledge/Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(m => m.Id == id);
        }

		//[HttpGet("Error")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
