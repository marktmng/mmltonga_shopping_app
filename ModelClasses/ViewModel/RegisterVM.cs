using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModel
{
    public class RegisterVM
    {
        public ApplicationUser applicationUser { get; set; }

        public string StatusMessage { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password don't match")]
        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set;}

    }
}
