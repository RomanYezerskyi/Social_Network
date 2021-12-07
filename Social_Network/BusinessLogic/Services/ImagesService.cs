using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public class ImagesService: IImagesService
    {
        public async Task<ICollection<PostImages>> AddImagesToPostAsync(ICollection<IFormFile> formFiles, int postId)
        {
            ICollection<PostImages> images = new List<PostImages>();
            if (formFiles.Count > 0)
            {
                foreach (var file in formFiles)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)file.Length);
                    }

                    var imageDataString = Convert.ToBase64String(imageData);
                    var imageResult = $"data:image/jpeg;base64,{imageDataString}";
                    var image = new PostImages
                    {
                        ImageData = imageResult,
                        PostId =postId,
                    };
                    images.Add(image);
                }

                return images;
            }

            return images;
        }

        public async Task<ICollection<MessageImages>> AddImagesToMessageAsync(ICollection<IFormFile> formFiles, int messageId)
        {
            ICollection<MessageImages> images = new List<MessageImages>();
            if (formFiles.Count > 0)
            {
                foreach (var file in formFiles)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)file.Length);
                    }

                    var imageDataString = Convert.ToBase64String(imageData);
                    var imageResult = $"data:image/jpeg;base64,{imageDataString}";
                    var image = new MessageImages
                    {
                        ImageData = imageResult,
                        MessageId = messageId,
                    };
                    images.Add(image);
                }

                return images;
            }

            return images;
        }
    }
}
