using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Data.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Social_Network.Models;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
    [Authorize(Roles = "User")]
    public class ChatsController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IFriendshipService _friendshipService;
        private readonly int _sizeLimit = 5;
        public ChatsController(IChatService chatService, IMessageService messageService,
            IUserService userService, IFriendshipService friendshipService)
        {
            _chatService = chatService;
            _messageService = messageService;
            _userService = userService;
            _friendshipService = friendshipService;
        }

        public IActionResult CreatePublicChat()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublicChat(CreateEditChatViewModel chatModel)
        {
            if (ModelState.IsValid)
            {
                if (chatModel.NewChatImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(chatModel.NewChatImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int) chatModel.NewChatImage.Length);
                    }
                    var image = Convert.ToBase64String(imageData);
                    chatModel.ChatImage = $"data:image/jpeg;base64,{image}";
                }
                await _chatService.CreateChatPublicAsync(chatModel.ChatName, chatModel.AboutChat,
                    User.FindFirst(ClaimTypes.NameIdentifier).Value, chatModel.ChatImage);
                return RedirectToAction("Index", "Home");
            }
            return View(chatModel);
        }

        public async Task<IActionResult> EditChat(int chatId)
        {
            var chat = await _chatService.GetChatAsync(chatId);
            if (chat == null)
                return NotFound();
            CreateEditChatViewModel model = new CreateEditChatViewModel
            {
                chatId = chat.Id,
                AboutChat = chat.AboutChat,
                ChatName = chat.ChatName,
                ChatImage = chat.ChatImage,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditChat(CreateEditChatViewModel chatModel)
        {
            if (ModelState.IsValid)
            {
                if (chatModel.NewChatImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(chatModel.NewChatImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)chatModel.NewChatImage.Length);
                    }
                    var image = Convert.ToBase64String(imageData);
                    chatModel.ChatImage = $"data:image/jpeg;base64,{image}";
                }
                await _chatService.EditChat(chatModel.chatId,chatModel.ChatName, chatModel.AboutChat, chatModel.ChatImage);
                return RedirectToAction("Chat", "Chats", new {id = chatModel.chatId});
            }
            return View(chatModel);
        }

        public async Task<IActionResult> GoToChatPrivate(string userId)
        {
            var chat = await _chatService.GoToChatPrivateAsync(userId, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("Chat", new { id = chat });
        }


        [HttpGet("Chat/{id}")]
        public async Task<IActionResult> Chat(int id, int skip=1)
        {
            int take = 10;
            var chat = await _chatService.GetChatAsync(id, skip, take);
            PaginatedListModel paginatedListModel = new PaginatedListModel(chat.countMessages, skip, take);
            PageOfChatPageViewModel viewModel = new PageOfChatPageViewModel()
            {
                PageMessagesViewModel = paginatedListModel,
                Chat = chat.chat
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> JoinChat(int id)
        {
            await _chatService.JoinPublicChatAsync(id, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("Chat", "Chats", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> LeaveChat(int id)
        {
            await _chatService.LeaveChatAsync(id, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteChat(int id)
        {
            await _chatService.DeleteChatAsync(id, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ClearChat(int id)
        {
            await _chatService.CleanChatAsync(id);
            return RedirectToAction("Chat", new { id = id });
        }

        public async Task<IActionResult> Private(int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
            var chats = await _chatService.AllChatsPrivateAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value,
                page: pageNum, limit: _sizeLimit);
            PaginatedListModel paginatedListModel = new PaginatedListModel(chats.count, pageNum, _sizeLimit);
            PageListChatsViewModel viewModel = new PageListChatsViewModel()
            {
                PageViewModel = paginatedListModel,
                Chats = chats.chats
            };
            return View(viewModel);
        }
        public IActionResult CreatePrivateRoom()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePrivateRoom(CreateEditChatViewModel chatModel)
        {
            if (ModelState.IsValid)
            {
                if (chatModel.NewChatImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(chatModel.NewChatImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)chatModel.NewChatImage.Length);
                    }

                    var image = Convert.ToBase64String(imageData);
                    chatModel.ChatImage = $"data:image/jpeg;base64,{image}";
                }

                chatModel.chatId = await _chatService.CreatePrivateRoomAsync(chatModel.ChatName, chatModel.AboutChat,
                    User.FindFirst(ClaimTypes.NameIdentifier).Value, chatModel.ChatImage);
                return RedirectToAction("Chat", "Chats", new { id = chatModel.chatId });
            }

            return View(chatModel);
        }
        [HttpGet]
        public async Task<IActionResult> AddUsersToRoom(string userId,int chatId, int pageNum = 1)
        {
            if(chatId != 0)
                @ViewData["CurrentFilter"] = chatId;
            if (userId != null)
            {
                await _chatService.AddUserToPrivateRoomAsync(chatId, userId);
            }
            pageNum = pageNum == 0 ? 1 : pageNum;
            var friends = await _friendshipService.GetFriendsAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value,
                page: pageNum,
                limit: _sizeLimit);
            PaginatedListModel paginatedListModel = new PaginatedListModel(friends.count, pageNum, _sizeLimit);
            PageFriendRequestsViewModel viewModel = new PageFriendRequestsViewModel
            {
                Friendships = friends.friends,
                PageViewModel = paginatedListModel
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserFromChat(string userId, int chatId)
        {
            await _chatService.DeleteUserFromChat(chatId, userId);
            return RedirectToAction("UsersInChat", new {chatId = chatId});
        }

        [HttpGet("/UsersInChat/{chatId}")]
        public async Task<ViewResult> UsersInChat(int chatId, int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
            var users = await _chatService.GetUsersInChatAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value, page: pageNum, limit: _sizeLimit, chatId);
            PaginatedListModel paginatedListModel = new PaginatedListModel(users.count, pageNum, _sizeLimit);
            PageUsersInChatViewModel viewModel = new PageUsersInChatViewModel
            {
                PageViewModel = paginatedListModel,
                Users = users.users
            };
            return View(viewModel);
        }

        public async Task<IActionResult> EditUserRole(string userId, int chatId, int role)
        {
            var user = await _chatService.EditUserRole(chatId, userId, role);
            if (user == null)
                return NotFound();
            return View(user);
        }
    }
}
