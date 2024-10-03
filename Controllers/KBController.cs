using KnowIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;

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
            return View(await _context.articles.ToListAsync());
        }

        // GET: Create Knowledge
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Validation around knowledge creation
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.DateCreated = DateTime.Now; //Sets the creation date/time
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

            var article = await _context.articles.FindAsync(id);
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
            var article = await _context.articles
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
            var article = await _context.articles.FindAsync(id);
            _context.articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.articles.Any(m => m.Id == id);
        }

		//[HttpGet("Error")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
