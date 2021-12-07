using System;

namespace Social_Network.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
//if (messageText != null)
//{
//    var message = await _messageService.CreateMessageAsync(chatId, messageText, User.Identity.Name);
//    //var message = await _messageService.GetMessageInChatAsync(chatId, messageText, User.Identity.Name);
//    await _chat.Clients.Groups(chatId.ToString()).SendAsync("RecieveMessage", new
//    {
//        Text = message.Text,
//        Name = message.UserName,
//        TimeStamp = message.TimeStamp.ToString("dd/MM/yyy hh:mm:ss")
//    });
//    return RedirectToAction("Chat", "Chats", new { id = chatId });
//}

//return NoContent();