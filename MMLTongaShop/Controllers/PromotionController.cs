using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using ModelClasses.ViewModel;
using System.Linq;

namespace MMLTongaShop.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder)
        {
            var promoProducts = _context.Products
                .Where(p => p.Name.Contains("promo"));

            switch (sortOrder)
            {
                case "name_asc":
                    promoProducts = promoProducts.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    promoProducts = promoProducts.OrderByDescending(p => p.Name);
                    break;
                case "price_asc":
                    promoProducts = promoProducts.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    promoProducts = promoProducts.OrderByDescending(p => p.Price);
                    break;
            }

            var viewModel = new HomePageVM
            {
                ProductList = promoProducts.ToList()
                // Include other necessary properties if needed
            };

            return View(viewModel);
        }
    }
}
