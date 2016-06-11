using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DarkServerWeb.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage ="The username must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The password must be {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Required]
        [Display(Name = "Security Question")]
        public string SecurityQuestion { get; set; }

        [Required]
        [Display(Name = "Answer")]
        [DataType(DataType.Password)]
        public string Answer { get; set; }
    }
}