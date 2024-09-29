using KnowIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KnowIT.Controllers
{
    public class KBController : Controller
    {
        private readonly ILogger<KBController> _logger;
        private readonly KnowledgeDbContext _context;

        public KBController(ILogger<KBController> logger, KnowledgeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Knowledge
        public async Task<IActionResult> Index()
        {
            return View(await _context.articles.ToListAsync());
        }

        // GET: Create Knowledge
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
            return View (article);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
