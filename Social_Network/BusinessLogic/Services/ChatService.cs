using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public ChatService(IUnitOfWork unitOfWork, IMessageService messageService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _messageService = messageService;
            _userService = userService;
        }

        public async Task<(Chat chat, int countMessages)> GetChatAsync(int id, int? skip, int take)
        {
            skip ??= 1;
            var countMessages = await _unitOfWork.Messages.GetCountAsync(filter: filter => filter.ChatId == id);

            var chat = await _unitOfWork.Chats.GetAsync(filter: x => x.Id == id,
                includes: f => f.Include(m => m.Messages.OrderByDescending(x => x.TimeStamp).Skip((int) ((skip - 1) * take)).Take(take))
                    .ThenInclude(x=>x.Images)
                    .Include(u => u.Users)
                .ThenInclude(u => u.User));
            return (chat, countMessages);
        }
        public async Task<Chat> GetChatAsync(int id)
        {
            var chat = await _unitOfWork.Chats.GetAsync(filter: x => x.Id == id);
            
            return chat;
        }
        public async Task EditChat(int chatId, string name, string aboutChat, string image)
        {
            var chat = await _unitOfWork.Chats.GetAsync(filter: filter => filter.Id == chatId);
            if (name != null)
                chat.ChatName = name;
            if (aboutChat != null)
                chat.AboutChat = aboutChat;
            if (image != null)
                chat.ChatImage = image;
            await _unitOfWork.Chats.UpdateAsync(chat);
        }

        public async Task CreateChatPublicAsync(string name, string aboutChat, string userId, string image)
        {
            var chat = new Chat
            {
                ChatName = name,
                AboutChat = aboutChat,
                ChatImage = image,
                Type = ChatType.Publlic
            };
            chat.Users.Add(new UsersInChats()
            {
                UserId = userId,
                Role = UserRoleInChatType.Owner
            });
            await _unitOfWork.Chats.InsertAsync(chat);
        }

        public async Task<int> GetPrivateChatAsync(string userId, string mainUserId)
        {
            var chat = await _unitOfWork.Chats.GetAsync(filter: x =>
            x.Users.FirstOrDefault(y => y.UserId != mainUserId).UserId == userId &&
            x.Users.FirstOrDefault(y => y.UserId != userId).UserId == mainUserId && x.Type == ChatType.Private);
            if (chat != null) 
                return chat.Id;
            return 0;
        }
        public async Task<int> GoToChatPrivateAsync(string userId, string mainUserId)
        {
            var chat = await GetPrivateChatAsync(userId, mainUserId);
            if (chat == 0)
            {
                var newChat = new Chat
                {
                    Type = ChatType.Private,
                };
                newChat.Users.Add(new UsersInChats
                {
                    UserId = userId
                });
                newChat.Users.Add(new UsersInChats
                {
                    UserId = mainUserId
                });
                await _unitOfWork.Chats.InsertAsync(newChat);

                return newChat.Id;
            }
            return chat;
        }

        public async Task<(IEnumerable<Chat> chats, int count)> AllChatsPublicAsync(string userId, int page, int limit, string search)
        {
            if (search != null)
            {
                page = 1;
                var countSearch = await _unitOfWork.Chats.GetCountAsync(filter: chat => chat.Type == ChatType.Publlic && chat.ChatName == search);
                var chatsSearch = await _unitOfWork.Chats.GetAsync(page: page, limit: limit,
                    filter: chat => chat.Type == ChatType.Publlic && chat.ChatName.Contains(search),
                    includes: i => i.Include(user => user.Users).ThenInclude(u => u.User), orderBy: null);
                return (chatsSearch, countSearch);

            }
            int count = await _unitOfWork.Chats.GetCountAsync(filter: chat => chat.Type == ChatType.Publlic);
            var chats = await _unitOfWork.Chats.GetAsync(page: page, limit: limit,
                filter: chat => chat.Type == ChatType.Publlic, 
                includes: i=>i.Include(user => user.Users).ThenInclude(u=>u.User), orderBy: null);
            return (chats, count);
        }

        public async Task<(IEnumerable<Chat> chats, int count)> AllChatsPrivateAsync(string userId, int page, int limit)
        {
            int count = await _unitOfWork.Chats.GetCountAsync(filter: f => (f.Type == ChatType.Private || f.Type == ChatType.PrivateRoom)&& f.Users.Any(x => x.UserId == userId));
            var chats = await _unitOfWork.Chats.GetAsync(page: page, limit: limit,
                filter: f => (f.Type == ChatType.Private || f.Type == ChatType.PrivateRoom) && f.Users.Any(x => x.UserId == userId),
                includes: f =>
                    f.Include(y => y.Users)
                    .ThenInclude(y => y.User).Include(m=>m.Messages), orderBy: null);
            return (chats, count);
        }

        public async Task JoinPublicChatAsync(int chatId, string userId)
        {
            var user = await _unitOfWork.UsersInChats.GetAsync(filter: x => x.ChatId == chatId && x.UserId == userId);
            if (user == null)
            {
                var chatUser = new UsersInChats
                {
                    ChatId = chatId,
                    UserId = userId,
                    Role = UserRoleInChatType.Member
                };
                await _unitOfWork.UsersInChats.InsertAsync(chatUser);
            }
        }

        public async Task LeaveChatAsync(int chatId, string userId)
        {
            var user = await _unitOfWork.UsersInChats.GetAsync(filter: x => x.UserId == userId && x.ChatId == chatId);

            await _unitOfWork.UsersInChats.DeleteAsync(user);

        }

        public async Task DeleteChatAsync(int chatId, string userId)
        {
            var chat = await _unitOfWork.Chats.GetAsync(filter: x => x.Id == chatId && x.Type == ChatType.Publlic 
                && x.Users.Any(y => y.Role == UserRoleInChatType.Owner && y.UserId == userId));
            await _unitOfWork.Chats.DeleteAsync(chat);
        }

        public async Task CleanChatAsync(int chatId)
        {
            var messages = await _unitOfWork.Messages.GetAsync(filter: x => x.ChatId == chatId, orderBy: null);
            await _unitOfWork.Messages.DeleteAsync(messages);
        }

        public async Task<int> CreatePrivateRoomAsync(string name, string aboutChat, string userId, string image)
        {
            var chat = new Chat
            {
                ChatName = name,
                AboutChat = aboutChat,
                ChatImage = image,
                Type = ChatType.PrivateRoom
            };
            chat.Users.Add(new UsersInChats()
            {
                UserId = userId,
                Role = UserRoleInChatType.Owner
            });
            await _unitOfWork.Chats.InsertAsync(chat);
            return chat.Id;
        }

        public async Task AddUserToPrivateRoomAsync(int chatId, string userId)
        {
            var user = await _unitOfWork.UsersInChats.GetAsync(filter: x => x.ChatId == chatId && x.UserId == userId);
            if (user == null)
            {
                var chatUser = new UsersInChats
                {
                    ChatId = chatId,
                    UserId = userId,
                    Role = UserRoleInChatType.Member
                };
                await _unitOfWork.UsersInChats.InsertAsync(chatUser);
            }
        }

        public async Task DeleteUserFromChat(int chatId, string userId)
        {
            var userInChat = await _unitOfWork.UsersInChats.GetAsync(filter: x => x.ChatId == chatId && x.UserId == userId);
            if (userInChat != null)
            {
                await _unitOfWork.UsersInChats.DeleteAsync(userInChat);
            }
        }

        public async Task<(IEnumerable<UsersInChats> users, int count)> GetUsersInChatAsync(string userId, int page, int limit,
            int chatId)
        {
            int count = await _unitOfWork.UsersInChats.GetCountAsync(filter: x => x.ChatId == chatId);
            var usersInChat = await _unitOfWork.UsersInChats.GetAsync(page: page, limit: limit,
                filter: x => x.ChatId == chatId, 
                includes: include => include.Include(x=> x.User), 
                orderBy: orderBy => orderBy.OrderByDescending(x=>x.User.IsActive));
            if(page > 1){
                var user = await _unitOfWork.UsersInChats.GetAsync(filter: x =>
                    x.UserId == userId && x.ChatId == chatId,
                    includes: include=> include.Include(x=> x.User));
                usersInChat = usersInChat.Append(user);
            }

            return (usersInChat, count);
        }

        public async Task<UsersInChats> EditUserRole(int chatId, string userId, int role)
        {
            var userInChat = await 
                _unitOfWork.UsersInChats.GetAsync(filter: user => user.ChatId == chatId && user.UserId == userId, 
                    includes: include=> include.Include(x=> x.User));
            if (userInChat != null)
            {
                if (role != 0)
                {
                    if (role.Equals(1))
                    {
                        userInChat.Role = UserRoleInChatType.Member;
                    }
                    else if (role.Equals(2))
                    {
                        userInChat.Role = UserRoleInChatType.Admin;
                    }
                    else if (role.Equals(3))
                    {
                        userInChat.Role = UserRoleInChatType.Guest;
                    }

                    await _unitOfWork.UsersInChats.UpdateAsync(userInChat);
                    return userInChat;
                }
                return userInChat;
            }

            return null;
        }
    }
}
