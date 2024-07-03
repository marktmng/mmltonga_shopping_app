using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelClasses;

namespace MMLTongaShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.Categories.ToList();
            
            return View(items);
        }

        // Upsert
		public IActionResult Upsert(int? id)
		{
			
            if (id == 0)
            {
                Category category = new Category();
                return View(category);
            }
            else
            {
                var items = _context.Categories.FirstOrDefault(u => u.Id == id);
                return View(items);
            }
		}

        [HttpPost]
		public async Task<IActionResult> Upsert(int? id, Category category)
		{

			if (id == null)
			{
                var foundItem = await _context.Categories.FirstOrDefaultAsync(u => u.Name == category.Name);
                if (foundItem != null)
                {
                    TempData["AlertMessage"] = category.Name + " is an existing item found in the list. so not added to the list";
                    return RedirectToAction("Index");
                }
				await _context.Categories.AddAsync(category);
                TempData["AlertMessage"] = category.Name + " has added into the category";
                //return View(category);
            }
			else
			{
				var items = await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);
                items.Name = category.Name;
                TempData["AlertMessage"] = category.Name + " has edited into the category";
                //return View(items);
            }

            await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		// GET: Delete
		public IActionResult Delete(int id)
		{
			var categoryToDelete = _context.Categories.FirstOrDefault(u => u.Id == id);

			if (categoryToDelete == null)
			{
				TempData["AlertMessage"] = "Category not found";
				return RedirectToAction(nameof(Index));
			}

			return View(categoryToDelete);
		}

		// POST: Delete
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(int id)
		{
			var categoryToDelete = _context.Categories.FirstOrDefault(u => u.Id == id);

			if (categoryToDelete != null)
			{
				_context.Categories.Remove(categoryToDelete);
				_context.SaveChanges();
				TempData["AlertMessage"] = categoryToDelete.Name + " has been deleted from the category";
			}
			else
			{
				TempData["AlertMessage"] = "Category not found";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
