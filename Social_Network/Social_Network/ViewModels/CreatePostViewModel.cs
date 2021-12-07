using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Social_Network.ViewModels
{
    public class CreatePostViewModel
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Text must be between 3 and 100 characters long")]
        public string Content { get; set; }
        public ICollection<IFormFile> Images { get; set; }
    }
}
