using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Social_Network.ViewModels
{
    public class UserProfileViewModel
    {
        [StringLength(10, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 10 characters long")]
        public string NickName { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password don't match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }

        public IFormFile NewProfileImage { get; set; }
        public string ProfileImage { get; set; }
    }
}
