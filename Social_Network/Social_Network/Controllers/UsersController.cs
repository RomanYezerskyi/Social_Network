using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Social_Network.Models;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly int _sizeLimit = 7;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        
        }

        public async Task<IActionResult> AllUsers(string search, int pageNum = 1)
        {
            ViewData["CurrentFilter"] = search;
            pageNum = pageNum == 0 ? 1 : pageNum;
            var users = await _userService.GetAllAsync(page: pageNum, limit: _sizeLimit, search);
            PaginatedListModel paginatedListModel = new PaginatedListModel(users.count, pageNum, _sizeLimit);
            PageUsersViewModel viewModel = new PageUsersViewModel
            {
                PageViewModel = paginatedListModel,
                Users = users.users
            };
            return View(viewModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User {UserName = model.NickName, Email = model.Email, MemberSince = DateTime.Now};
                if (model.ProfileImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.ProfileImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int) model.ProfileImage.Length);
                    }

                    var image = Convert.ToBase64String(imageData);
                    user.ProfileImage = $"data:image/jpeg;base64,{image}";
                }

                await _userService.CreateUserAsync(user, model.Password);

                return RedirectToAction("AllUsers", "Users");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new EditUserViewModel
                {Id = user.Id, NickName = user.UserName, Email = user.Email, ProfileImage = user.ProfileImage};


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = _userService.GetByIdAsync(model.Id).Result;
            if (ModelState.IsValid)
            {
                if (model.NewProfileImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.NewProfileImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.NewProfileImage.Length);
                    }

                    var image = Convert.ToBase64String(imageData);
                    user.ProfileImage = String.Format("data:image/jpeg;base64,{0}", image);
                }
                else if (model.Email != null)
                {
                    user.Email = model.Email;
                }
                else if (model.NickName != null)
                {
                    user.UserName = model.NickName;

                }
                await _userService.UpdateUserAsync(user);
                return RedirectToAction("AllUsers", "Users");
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userService.GetByIdAsync(id);
            if (user != null)
            {
                await _userService.DeleteUserAsync(user);
            }
            return RedirectToAction("AllUsers", "Users");
        }
    }
}
