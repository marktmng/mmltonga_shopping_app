using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMLTongaShop.Models;
using MMLTongaShop.Utility;
using ModelClasses.ViewModel;
using System.Diagnostics;

namespace MMLTongaShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // home page and search functionality
        public IActionResult Index(string? searchByName, string? searchByCategory)
        {

            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var count = _db.userCarts.Where(u => userId.Contains(userId)).Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);

            }


            HomePageVM vm = new HomePageVM();
            if (searchByName != null)
            {
                vm.ProductList = _db.Products.Where(productName => EF.Functions.Like(productName.Name, $"%{searchByName}%")).ToList();
                vm.Categories = _db.Categories.ToList();
            }
            else if (searchByCategory != null)
            {
                var searchByCategoryName = _db.Categories.FirstOrDefault(u => u.Name == searchByCategory);
                vm.ProductList = _db.Products.Where(u => u.CategoryId == searchByCategoryName.Id).ToList();
                vm.Categories = _db.Categories.Where(u => u.Name.Contains(searchByCategory));
            }
            else
            {
                vm.ProductList = _db.Products.ToList();
                vm.Categories = _db.Categories.ToList();
            }


            return View(vm);
        }


        public IActionResult Home()
        {
            // Create an instance of HomePageVM
            HomePageVM viewModel = new HomePageVM();

            // Fetch the list of products from the database
            // Replace this with your actual database fetching logic
            viewModel.ProductList = _db.Products.ToList(); // Assuming _db is your database context

            // Since this example focuses on products, Categories and searchByName are not used here.
            // If needed, you can still initialize them as shown in the previous example.

            // Pass the ViewModel to the view
            return View(viewModel);
        }

        public IActionResult ProductDetails(int id)
        {
            var product = _db.Products
                .Include(p => p.category)
                .Include(p => p.ImgUrls)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //FOR TESTING ERRORS
        public IActionResult TriggerError401()
        {
            return StatusCode(401); // Triggers Error 401
        }

        public IActionResult TriggerError403()
        {
            return StatusCode(403); // Triggers Error 403
        }

        public IActionResult TriggerError404()
        {
            return StatusCode(404); // Triggers Error 404
        }

        public IActionResult TriggerError500()
        {
            return StatusCode(500); // Triggers Error 500
        }
    }
}