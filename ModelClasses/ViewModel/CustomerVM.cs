using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModel
{
    public class CustomerVM
    {
        public ApplicationUser applicationUser { get; set; }
        public string Id { get; set; } // User ID
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public int ShoppingActivityCounter { get; set; }
        public DateTime? LatestShopDate { get; set; }
    }

}
