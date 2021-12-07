using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Interfaces
{
    public interface IMessageService
    {
        Task<Message> CreateMessageAsync(int chatId, string messageText, string userName, ICollection<IFormFile> files);
    }
}
