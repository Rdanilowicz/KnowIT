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
        public IActionResult Create()
        {
			ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
			return View(new Article());
        }

        // POST: Validation around knowledge creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.DateCreated = DateTime.Now; //Sets the creation date/time
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
			ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", article.CategoryID);
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
