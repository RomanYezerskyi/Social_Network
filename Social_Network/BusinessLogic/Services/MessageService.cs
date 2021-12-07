using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public class MessageService: IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImagesService _imagesService;
        public MessageService(IUnitOfWork unitOfWork, IImagesService imagesService)
        {
            _unitOfWork = unitOfWork;
            _imagesService = imagesService;
        }
        public async Task<Message> CreateMessageAsync(int chatId, string messageText, string userName, ICollection<IFormFile> files)
        {
            var message = new Message
            {
                ChatId = chatId,
                Text = messageText,
                UserName = userName,
                TimeStamp = DateTime.Now.ToLocalTime()
            };
            await _unitOfWork.Messages.InsertAsync(message);
            if (files.Count > 0)
            {
                var collectionImages = await _imagesService.AddImagesToMessageAsync(files, message.Id);
                message.Images = collectionImages;
                await _unitOfWork.Messages.UpdateAsync(message);
            }

            return message;
        }
    }
}
