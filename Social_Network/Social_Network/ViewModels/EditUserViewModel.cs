using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Social_Network.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [StringLength(10, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 10 characters long")]
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile NewProfileImage { get; set; }
        public string ProfileImage { get; set; }
    }
}
