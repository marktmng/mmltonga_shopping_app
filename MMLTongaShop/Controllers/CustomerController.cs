using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelClasses.ViewModel;
using ModelClasses;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MMLTongaShop.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        //// List customers
        //public IActionResult Index()
        //{
        //    var users = _userManager.Users.ToList().Select(u => {
        //        var appUser = u as ApplicationUser; // Cast to ApplicationUser
        //        return new CustomerVM
        //        {
        //            Id = appUser.Id,
        //            FirstName = appUser.FirstName,
        //            LastName = appUser.LastName,
        //            Email = appUser.Email,
        //            ContactNumber = appUser.ContactNumber, 
        //            ShoppingActivityCounter = appUser.ShoppingActivityCounter,
        //            LatestShopDate = appUser.LatestShopDate
        //        };
        //    }).ToList();

        //    return View(users);
        //}


        public IActionResult Index(string? searchByName, DateTime? startDate, DateTime? endDate, bool topCustomers = false)
        {
            var usersQuery = _userManager.Users
                .Select(u => u as ApplicationUser)
                .Where(u => u != null)
                .Select(u => new CustomerVM
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ContactNumber = u.ContactNumber,
                    ShoppingActivityCounter = u.ShoppingActivityCounter,
                    LatestShopDate = u.LatestShopDate
                });

            // Match/Like search
            if (!string.IsNullOrEmpty(searchByName))
            {
                usersQuery = usersQuery.Where(u =>
                    EF.Functions.Like(u.FirstName, $"%{searchByName}%") ||
                    EF.Functions.Like(u.LastName, $"%{searchByName}%") ||
                    EF.Functions.Like(u.Email, $"%{searchByName}%"));
            }

            // Date range search
            if (startDate.HasValue && endDate.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.LatestShopDate >= startDate && u.LatestShopDate <= endDate);
            }

            // Top 10 customers with highest ShoppingActivityCounter
            if (topCustomers)
            {
                usersQuery = usersQuery.OrderByDescending(u => u.ShoppingActivityCounter).Take(10);
            }

            var usersList = usersQuery.ToList();

            return View(usersList);
        }



        // Create/Add form
        public IActionResult Create()
        {
            return View();
        }

        // Handle Add function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerVM model)
        {

            var user = new ApplicationUser // Cast to ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                ContactNumber = model.ContactNumber,
                ShoppingActivityCounter = model.ShoppingActivityCounter,
                LatestShopDate = model.LatestShopDate
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            

            return View(model);
        }

        // Show edit form
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id) as ApplicationUser; // Cast to ApplicationUser
            if (user == null)
            {
                return NotFound();
            }

            var model = new CustomerVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ContactNumber = user.ContactNumber, 
                ShoppingActivityCounter = user.ShoppingActivityCounter,
                LatestShopDate = user.LatestShopDate
            };

            return View(model);
        }

        // Handle updates
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id) as ApplicationUser; // Cast to ApplicationUser
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email; // Ensure UserName is updated appropriately
                user.ContactNumber = model.ContactNumber; 
                user.ShoppingActivityCounter = model.ShoppingActivityCounter;
                user.LatestShopDate = model.LatestShopDate;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        // GET: Customer/DeleteConfirmation/5
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id) as ApplicationUser;
            if (user == null)
            {
                return NotFound();
            }

            var model = new CustomerVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ContactNumber = user.ContactNumber, // Use ContactNumber from ApplicationUser
                ShoppingActivityCounter = user.ShoppingActivityCounter,
                LatestShopDate = user.LatestShopDate
            };

            return View("Delete", model); // Specify the view name if it's different
        }


        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Add this for security
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id) as ApplicationUser;
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Delete", new CustomerVM { Id = id }); // Return to the confirmation view with error messages
            }
        }
    }
}
