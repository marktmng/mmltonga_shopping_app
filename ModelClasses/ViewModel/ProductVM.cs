using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModel
{
	public class ProductVM
	{

		public Product? Products { get; set; }

		public IEnumerable<SelectListItem>? CategoriesList { get; set; }

		public IEnumerable<IFormFile>? Images { get; set; }

		public Inventory? Inventories { get; set; }

		public PImages? PImages { get; set; }

	}
}
