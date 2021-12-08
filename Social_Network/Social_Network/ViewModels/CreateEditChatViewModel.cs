using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Social_Network.ViewModels
{
    public class CreateEditChatViewModel
    {
        public int chatId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage ="The name must be between 3 and 10 characters long")]
        public string ChatName { get; set; }
        public string ChatImage { get; set; }
        [StringLength(50, MinimumLength = 0, ErrorMessage = "Next must be between 3 and 50 characters long")]
        public string AboutChat { get; set; }
        public IFormFile NewChatImage { get; set; }
    }
}
