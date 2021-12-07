using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Social_Network.ViewModels
{
    public class EditPostViewModel
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 100 characters long")]
        public string Content { get; set; }
        public ICollection<PostImages> Images { get; set; }
        public ICollection<IFormFile> NewImages { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
