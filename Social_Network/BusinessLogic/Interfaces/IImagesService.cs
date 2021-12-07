using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Interfaces
{
    public interface IImagesService
    {
        Task<ICollection<PostImages>> AddImagesToPostAsync(ICollection<IFormFile> formFiles, int postId);
        Task<ICollection<MessageImages>> AddImagesToMessageAsync(ICollection<IFormFile> formFiles, int messageId);
    }
}
