using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Social_Network.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IChatService _chatService;
        private readonly int _sizeLimit = 2;
        public HomeController(IChatService chatService)
        {
            _chatService = chatService;
        }
        public async Task<IActionResult> Index(string search, int pageNum = 1)
        {
            ViewData["CurrentFilter"] = search;
            pageNum = pageNum == 0 ? 1 : pageNum;
            //if (User.IsInRole("Admin"))
            //{
            //    return RedirectToAction("AllUsers", "Users");
            //}
            var chats = await _chatService.AllChatsPublicAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value, page: pageNum, limit: _sizeLimit, search);
            PaginatedListModel paginatedListModel = new PaginatedListModel(chats.count, pageNum, _sizeLimit);
            PageListChatsViewModel viewModel = new PageListChatsViewModel()
            {
                PageViewModel = paginatedListModel,
                Chats = chats.chats
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
