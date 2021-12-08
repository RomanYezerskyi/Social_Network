using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Social_Network.Hubs;

namespace Social_Network.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IHubContext<ChatHub> _chat;
        private readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService, IHubContext<ChatHub> chat)
        {
            _messageService = messageService;
            _chat = chat;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string text, string userName, string chatId, ICollection<IFormFile> images)
        {
            var message = await _messageService.CreateMessageAsync(Convert.ToInt32(chatId), text, User.Identity.Name, images);
            IEnumerable<string> imgData = message.Images?.Select(x => x.ImageData);
            await _chat.Clients.Group(chatId).SendAsync("Send",
                message.Text,
                message.UserName,
                message.TimeStamp.ToString("dd/MM/yyyy HH:mm:ss"),
                imgData
            );
            return RedirectToAction("Chat", "Chats", new { id = chatId });
        }
    }
}