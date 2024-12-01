using KnowIT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KnowIT.Controllers
{
    public class HomeController : Controller
    {
        private readonly KnowledgeDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(KnowledgeDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Index action to load the homepage with categories and login functionality
        public async Task<IActionResult> Index()
        {
            var featuredCategories = await _context.Categories.Take(3).ToListAsync(); // Example for showing 3 popular categories
            var model = new HomeViewModel
            {
                FeaturedCategories = featuredCategories
            };

            return View(model);
        }

        // GET Login action to display the login form
        public IActionResult Login()
        {
            return View();
        }

        // POST Login action to handle the form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index"); // Redirect to Home page after successful login
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }

            return View(); // If login fails, return to the login page with error
        }

        // POST Logout action to handle logging out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index"); // Redirect to Home page after logout
        }

        // ViewModel for the homepage
        public class HomeViewModel
        {
            public IEnumerable<Category> FeaturedCategories { get; set; }
        }
    }
}
