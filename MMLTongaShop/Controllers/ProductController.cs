using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ModelClasses;
using ModelClasses.ViewModel;

namespace MMLTongaShop.Controllers
{
    public class ProductController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _HostEnvironment; //for image upload

		public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_HostEnvironment = hostEnvironment;
		}

        //This is for the Index Page//

        //public IActionResult Index()
        //      {
        //	var products = _context.Products.Include(u=> u.category).ToList();
        //          return View(products);
        //      }

        public IActionResult Index(string? searchByName, int? searchByCategoryId)
        {
            IQueryable<ModelClasses.Product> products = _context.Products.Include(u => u.category);

            if (!string.IsNullOrEmpty(searchByName))
            {
                products = products.Where(p => EF.Functions.Like(p.Name, $"%{searchByName}%"));
            }

            if (searchByCategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == searchByCategoryId.Value);
            }

            var categories = _context.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            ViewBag.Categories = categories;

            return View(products.ToList());
        }




        //This is for the CREATE Function //

        [HttpGet]
		public IActionResult Create()
		{
			ProductVM productsVM = new ProductVM()
			{
				Inventories = new Inventory(),
				PImages = new PImages(),
				CategoriesList = _context.Categories.ToList().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
			};
			return View(productsVM);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductVM productVM)
		{
			string homeImageUrl = "";
			if (productVM.Images != null)
			{
				foreach (var image in productVM.Images)
				{
					homeImageUrl = image.FileName;
					if (homeImageUrl.Contains("Home"))
					{
						homeImageUrl = UploadFiles(image);
						break;
					}
				}

			}
			productVM.Products.HomeImgUrl = homeImageUrl;
			await _context.AddAsync(productVM.Products);
			await _context.SaveChangesAsync();
			var NewProduct = await _context.Products.Include(u => u.category).FirstOrDefaultAsync(u => u.Name == productVM.Products.Name);
			productVM.Inventories.Name = NewProduct.Name;
			productVM.Inventories.Category = NewProduct.category.Name;
			await _context.Inventories.AddAsync(productVM.Inventories);
			await _context.SaveChangesAsync();

			if (productVM.Images != null)
			{
				foreach (var image in productVM.Images)
				{
					string tempFileName = image.FileName;
					if (!tempFileName.Contains("Home"))
					{
						string stringFileName = UploadFiles(image);
						var addressImage = new PImages
						{
							ImageUrl = stringFileName,
							ProductId = NewProduct.Id,
							ProductName = NewProduct.Name
						};
						await _context.PImages.AddAsync(addressImage);
					}
				}
			}
			await _context.SaveChangesAsync();

			return RedirectToAction("Index", "Product");


		}

		//This is for the UPDATE/EDIT Function //
		[HttpGet]
		public IActionResult Edit(int Id)
		{
			ProductVM productsVM = new ProductVM()
			{
				Products = _context.Products.FirstOrDefault(p => p.Id == Id),
				CategoriesList = _context.Categories.ToList().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
			};
			productsVM.Products.ImgUrls = _context.PImages.Where(u=> u.ProductId == Id).ToList();

			return View(productsVM);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM)
        {

            var ProductToEdit = _context.Products.FirstOrDefault(u => u.Id == productVM.Products.Id);
            if (ProductToEdit != null)
            {
                ProductToEdit.Name = productVM.Products.Name;
                ProductToEdit.Price = productVM.Products.Price;
                ProductToEdit.Description = productVM.Products.Description;
                ProductToEdit.CategoryId = productVM.Products.CategoryId;
                if (productVM.Images != null)
                {
                    foreach (var item in productVM.Images)
                    {
                        string tempFileName = item.FileName;
                        if (!tempFileName.Contains("Home"))
                        {
                            string stringFileName = UploadFiles(item);
                            var addressImage = new PImages
                            {
                                ImageUrl = stringFileName,
                                ProductId = productVM.Products.Id,
                                ProductName = productVM.Products.Name,
                            };
                            _context.PImages.Add(addressImage);
                        }
                        else
                        {
                            if (ProductToEdit.HomeImgUrl == "")
                            {
                                string homeImageUrl = item.FileName;
                                if (homeImageUrl.Contains("Home"))
                                {
                                    homeImageUrl = UploadFiles(item);
                                    ProductToEdit.HomeImgUrl = homeImageUrl;
                                }
                            }
                        }
                    }
                }
                _context.Products.Update(ProductToEdit);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Product");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(ProductVM productVM)
        //{

        //	var ProductToEdit = _context.Products.FirstOrDefault(u => u.Id == productVM.Products.Id);
        //          if (ProductToEdit != null) 
        //	{
        //		ProductToEdit.Name = productVM.Products.Name;
        //		ProductToEdit.Price = productVM.Products.Price;
        //		ProductToEdit.Description = productVM.Products.Description;
        //		ProductToEdit.CategoryId = productVM.Products.CategoryId;
        //		if(productVM.Images != null) 
        //		{
        //			foreach (var item in productVM.Images) 
        //			{
        //				string tempFileName = item.FileName;
        //				if (!tempFileName.Contains("Home"))
        //				{
        //					string stringFileName = UploadFiles(item);
        //					var addressImage = new PImages
        //					{
        //						ImageUrl = stringFileName,
        //						ProductId = productVM.Products.Id,
        //						ProductName = productVM.Products.Name,
        //					};
        //					_context.PImages.Add(addressImage);
        //				}
        //				else
        //				{
        //					if(ProductToEdit.HomeImgUrl == "")
        //					{								
        //						string homeImageUrl = item.FileName;
        //						if (homeImageUrl.Contains("Home"))
        //						{
        //							homeImageUrl = UploadFiles(item);
        //							ProductToEdit.HomeImgUrl = homeImageUrl;
        //						}
        //					}
        //				}
        //			}
        //			_context.Products.Update(ProductToEdit);
        //			_context.SaveChanges();
        //		}
        //	}
        //	return RedirectToAction("Index", "Product");
        //}


        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.category)
                .Include(p => p.ImgUrls)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        //This is for the DELETE Function //
        [HttpDelete]
        public IActionResult Delete(int Id)
        {
			if(Id != 0)
			{
				var productToDelete = _context.Products.FirstOrDefault(x => x.Id == Id);
				var ImagesToDelete = _context.PImages.Where(u => u.ProductId == Id).Select(u => u.ImageUrl);
				foreach (var image in ImagesToDelete)
				{
					string imageUrl = "Images\\" + image;
					var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
					DeleteAImage(toDeleteImageFromFolder);
				}

				if (productToDelete.HomeImgUrl != "")
				{
					string imageUrl = "Images\\" + productToDelete.HomeImgUrl;
					var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
					DeleteAImage(toDeleteImageFromFolder);
				}
				_context.Products.Remove(productToDelete);
				_context.SaveChanges();
			}
			else
			{
				return Json(new {success= false, message = "Failed to Delete the item"});
			}
			return Json(new { success = true, message = "Item Deleted Successfully!" });
        }

		// delete function if someone uploaded multiple image by Mark
		public IActionResult DeleteAImg(string Id)
		{
			int routeId  = 0;
			if (Id != null)

			{
				if (!Id.Contains("Home"))
				{
					var ImageToDeleteFromPImage = _context.PImages.FirstOrDefault(u => u.ImageUrl == Id);
					if (ImageToDeleteFromPImage != null)
					{
						routeId = ImageToDeleteFromPImage.ProductId;
						_context.PImages.Remove(ImageToDeleteFromPImage);
					}
				}
				else
				{
					var ImageToDeleteFromProduct = _context.Products.FirstOrDefault(u => u.HomeImgUrl == Id);
					if (ImageToDeleteFromProduct != null)
					{
						ImageToDeleteFromProduct.HomeImgUrl = "";
						routeId = ImageToDeleteFromProduct.Id;
						_context.Products.Update(ImageToDeleteFromProduct);
					}
				}
				string ImageUrl = "Images\\" + Id;
				var toDeleteImageFromFolder = Path.Combine(_HostEnvironment.WebRootPath, ImageUrl);
				DeleteAImage(toDeleteImageFromFolder);
				_context.SaveChanges();

				return Json(new { success = true, message = "Picture is deleted successfully.", id = routeId });
			}
			return Json(new { success = false, message = "failed to Delete the item." });
		}


		private void DeleteAImage(string toDeleteImageFromFolder)
        {
           if (System.IO.File.Exists(toDeleteImageFromFolder))
			{
				System.IO.File.Delete(toDeleteImageFromFolder);
			}
        }


		//This is for the UPLOADING IMAGES Function //
		private string UploadFiles(IFormFile image)
		{
			string fileName = null;
			if (image != null)
			{
				string uploadDirLocation = Path.Combine(_HostEnvironment.WebRootPath, "Images");
				fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
				string filePath = Path.Combine(uploadDirLocation, fileName);
				using(var fileStream = new FileStream(filePath, FileMode.Create))
				{
					image.CopyTo(fileStream);
				}
			}
			return fileName;
		}
	}
}
