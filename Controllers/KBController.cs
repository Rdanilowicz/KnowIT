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

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }


        // POST: Validation around knowledge creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CategoryID")] Article article)
        {
            // Log ModelState errors (if any) before validation check
            _logger.LogInformation("ModelState Errors: ");
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    _logger.LogInformation($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            // Check if CategoryID is set correctly
            _logger.LogInformation($"CategoryID received in POST: {article.CategoryID}");

            // Validate CategoryID
            if (article.CategoryID == 0)
            {
                ModelState.AddModelError("CategoryID", "Please select a category.");
            }

            // If the model state is valid, save the article
            if (ModelState.IsValid)
            {
                article.DateCreated = DateTime.Now;
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // If invalid, repopulate the categories in the ViewBag and return the view
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(article);
        }


        // GET: Knowledge/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Pass the list of categories to the ViewBag
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", article.CategoryID);

            return View(article);
        }


        // POST: Knowledge/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Article article)
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
                    if (!_context.Articles.Any(e => e.Id == id))
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

            // Reload categories if model state is invalid
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", article.CategoryID);
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
