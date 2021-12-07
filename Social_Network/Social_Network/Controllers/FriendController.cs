using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Social_Network.Models;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
    //[Authorize(Roles = "User")]
    [Authorize]
    public class FriendController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly int _sizeLimit = 4;
        public FriendController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        public async Task<IActionResult> FindUsers(string search, string sortOrder, int pageNum = 1)
        {
            ViewData["CurrentFilter"] = search;
            pageNum = pageNum == 0 ? 1 : pageNum;
            
            var users = await _friendshipService.SearchUsersAsync(page: pageNum, limit: _sizeLimit, search: search, sortOrder: sortOrder,
                userId: User.FindFirst(ClaimTypes.NameIdentifier).Value);
            PaginatedListModel paginatedListModel = new PaginatedListModel(users.count, pageNum, _sizeLimit);
            PageUsersViewModel viewModel = new PageUsersViewModel
            {
                PageViewModel = paginatedListModel,
                Users = users.users
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> RequestForFriendshipAsync(string userId)
        {
            await _friendshipService.RequestForFriendshipAsync(userId, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("FindUsers");
        }

        public async Task<IActionResult> Friends(string search, string sortOrder, int pageNum = 1)
        {
            ViewData["CurrentFilter"] = search;
            pageNum = pageNum == 0 ? 1 : pageNum;
            var friends = await _friendshipService.SearchFriendsAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value,
                page: pageNum,
                limit: _sizeLimit, search: search);
            PaginatedListModel paginatedListModel = new PaginatedListModel(friends.count, pageNum, _sizeLimit);
            PageFriendRequestsViewModel viewModel = new PageFriendRequestsViewModel
            {
                Friendships = friends.friends,
                PageViewModel = paginatedListModel
            };
            return View(viewModel);
        }

        public async Task<IActionResult> FriendRequestsToMe(int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
           
            var requests = await 
                _friendshipService.GetFriendRequestsToUserAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value , page: pageNum, limit: _sizeLimit);
            PaginatedListModel paginatedListModel = new PaginatedListModel(requests.count, pageNum, _sizeLimit);
            PageFriendRequestsViewModel viewModel = new PageFriendRequestsViewModel
            {
                Friendships = requests.requests,
                PageViewModel = paginatedListModel
            };
            return View(viewModel);
        }
        public async Task<IActionResult> FriendRequestsFromMe(int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
    
            var requests = await 
                _friendshipService.GetFriendRequestsFromUserAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value, page: pageNum, limit: _sizeLimit);
            PaginatedListModel paginatedListModel = new PaginatedListModel(requests.count, pageNum, _sizeLimit);
            PageFriendRequestsViewModel viewModel = new PageFriendRequestsViewModel
            {
                Friendships = requests.requests,
                PageViewModel = paginatedListModel
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> AddToFriend(string userId)
        {
            await _friendshipService.AddFriendAsync(userId, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("Friends");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteFromFriend(string userId)
        {
            await _friendshipService.DeleteFromFriend(userId, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction("Friends");
        }
    }
}
