using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace ModelClasses
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }
        [Required]

        public string? PostalCode { get; set; }
        [Required]

        public string? State { get; set; }

        public string ContactNumber { get; set; }
        public int ShoppingActivityCounter { get; set; }
        public DateTime? LatestShopDate { get; set; }

    }
}
