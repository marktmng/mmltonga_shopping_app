using DatabaseAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMLTongaShop.Utility;
using ModelClasses.ViewModel;
using ModelClasses;

namespace MMLTongaShop.Controllers
{
	public class OrderController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public OrderController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_db = db;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult orderDetailPreview()
		{
			var claim = _signInManager.IsSignedIn(User);
			if (claim)
			{
				var userId = _userManager.GetUserId(User);
				var currentUser = _db.applicationUser.FirstOrDefault(x => x.Id == userId);
				SummeryVM summeryVM = new SummeryVM()
				{
					userCartList = _db.userCarts.Include(u => u.product).Where(u => u.userId.Contains(userId)).ToList(),
					orderSummery = new UserOrderHeader(),
					cartUserId = userId,
				};

				if (currentUser != null)
				{
					summeryVM.orderSummery.DeliveryStreetAddress = currentUser.Address;
					summeryVM.orderSummery.City = currentUser.City;
					summeryVM.orderSummery.State = currentUser.City;
					summeryVM.orderSummery.PostalCode = currentUser.PostalCode;
					summeryVM.orderSummery.Name = currentUser.FirstName + " " + currentUser.LastName;
				}
				var count = _db.userCarts.Where(u => u.userId.Contains(_userManager.GetUserId(User))).ToList().Count;
				HttpContext.Session.SetInt32(cartCount.sessionCount, count);
				return View(summeryVM);
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Summery(SummeryVM summeryVMFromView)
		{
			var claim = _signInManager.IsSignedIn(User);
			if (claim)
			{
				var userId = _userManager.GetUserId(User);
				var currentUser = _db.applicationUser.FirstOrDefault(x => x.Id == userId);
				SummeryVM summeryVM = new SummeryVM()
				{
					userCartList = _db.userCarts.Include(u => u.product).Where(u => u.userId.Contains(userId)).ToList(),
					orderSummery = new UserOrderHeader(),
					
				};

				if (currentUser != null)
				{
					//assigning the user's info from the database as default address
					summeryVM.orderSummery.Name = summeryVMFromView.orderSummery.Name;
					summeryVM.orderSummery.DeliveryStreetAddress = summeryVMFromView.orderSummery.DeliveryStreetAddress;
					summeryVM.orderSummery.City = summeryVMFromView.orderSummery.City;
					summeryVM.orderSummery.State = summeryVMFromView.orderSummery.State;
					summeryVM.orderSummery.PostalCode = summeryVMFromView.orderSummery.PostalCode;
					summeryVM.orderSummery.PhoneNumber = summeryVMFromView.orderSummery.PhoneNumber;
					summeryVM.orderSummery.DateOfOrder = DateTime.Now;
					summeryVM.orderSummery.OrderStatus = "Pending";
					summeryVM.orderSummery.OrderStatus = "Not Paid";
					await _db.AddAsync(summeryVM.orderSummery);
					await _db.SaveChangesAsync();
					
				}
				if (summeryVMFromView.orderSummery.TotalOrderAmount > 0)
				{
					//var CardChargeFee = (summeryVMFromView.orderSummery.TotalOrderAmount / 100) * 2.90 + 0.30;
					//double creditCardBalance = 30.00;

					//if (creditCardBalance > summeryVMFromView.orderSummery.TotalOrderAmount + CardChargeFee)
					//{
						return RedirectToAction("OrderSuccess", new { id = summeryVM.orderSummery.Id });
					//}
					//else
					//{
					//	return RedirectToAction("OrderCancelled");
					//}
				}
			}
			return View();
		}

		public IActionResult OrderCancelled()
		{
			return RedirectToAction("CartIndex", "Cart");
		}

		public IActionResult OrderSuccess(int id)
		{
			var claim = _signInManager.IsSignedIn(User);

			if (claim)
			{
				var userId = _userManager.GetUserId(User);
				var UserCartRemove = _db.userCarts.Where(u => u.userId.Contains(userId)).ToList();
				var orderProcessed = _db.orderHeaders.FirstOrDefault(h => h.Id == id);
				//Update Payment Status
				if (orderProcessed != null)
				{
					if (orderProcessed.PaymentStatus == "Not Paid")
					{ 
						orderProcessed.PaymentStatus = "Paid";
					
					}
				}
				//Add the items from cart to Order Details table
				foreach (var list in UserCartRemove)
				{
					OrderDetails orderReceived = new OrderDetails()
					{
						OrderHeaderId = orderProcessed.Id,
						ProductId = (int)list.ProductId,
						Count = list.Quantity,
					};

					_db.orderDetails.Add(orderReceived);
				}
				//Removed items from cart for the current user after successfully completing the payment process
				_db.userCarts.RemoveRange(UserCartRemove);
				_db.SaveChanges(true);
				var count = _db.userCarts.Where(u => u.userId.Contains(_userManager.GetUserId(User))).ToList().Count;
				HttpContext.Session.SetInt32(cartCount.sessionCount, count);

			}

			return View();

		}
	}
}
