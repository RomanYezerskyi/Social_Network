using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace BusinessLogic.Interfaces
{
    public interface IChatService
    {
        Task<(Chat chat, int countMessages)> GetChatAsync(int id, int? skip, int take);
        public Task<Chat> GetChatAsync(int id);
        Task CreateChatPublicAsync(string name, string aboutChat, string userId, string image);
        Task<int> GetPrivateChatAsync(string userId, string mainUserId);
        Task<int> GoToChatPrivateAsync(string userId, string mainUserId);
        Task EditChat(int chatId, string name, string aboutChat, string image);
        Task<(IEnumerable<Chat> chats, int count)> AllChatsPublicAsync(string userId, int page, int limit, string search);
        Task<(IEnumerable<Chat> chats, int count)> AllChatsPrivateAsync(string userId, int page, int limit);
        Task JoinPublicChatAsync(int chatId, string userId);
        Task LeaveChatAsync(int chatId, string userId);
        Task DeleteChatAsync(int chatId, string userId);
        Task CleanChatAsync(int chatId);
        Task<int> CreatePrivateRoomAsync(string name, string aboutChat, string userId, string image);
        Task  AddUserToPrivateRoomAsync(int chatId, string userId);
        Task DeleteUserFromChat(int chatId, string userId);
        Task<(IEnumerable<UsersInChats> users, int count)> GetUsersInChatAsync(string userId, int page, int limit, int chatId);
        Task<UsersInChats> EditUserRole(int chatId, string userId, int role);
    }
}
