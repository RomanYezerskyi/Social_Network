using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Social_Network.Models;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
   [Authorize(Roles = "User")]
   public class ProfileController : Controller
    {
        public readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {

            _userService = userService;
        }


        public async Task<IActionResult> MyProfile(int skip = 1, int take = 3)
        {
            var myProfile = await _userService.GetUserProfileAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value,skip,take);
            PaginatedListModel paginatedListModel = new PaginatedListModel(myProfile.countPosts, skip, take);
            PageProfileViewModel viewModel = new PageProfileViewModel()
            {
                PageViewModel = paginatedListModel,
                User = myProfile.user
            };
            return View(viewModel);
        }

        public async Task<IActionResult> EditProfile()
        {
            var user = await _userService.GetByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            UserProfileViewModel model = new UserProfileViewModel{ProfileImage = user.ProfileImage, NickName = user.UserName, Email = user.Email};
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserProfileViewModel pvm)
        {
           
            var user = _userService.GetByIdAsync((User.FindFirst(ClaimTypes.NameIdentifier).Value)).Result;
            if (ModelState.IsValid)
            {
                if (pvm.NewProfileImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(pvm.NewProfileImage.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)pvm.NewProfileImage.Length);
                    }

                    var image = Convert.ToBase64String(imageData);
                    user.ProfileImage = $"data:image/jpeg;base64,{image}";
                }   
                if (pvm.Email != null)
                {
                    user.Email = pvm.Email;
                }
                if (pvm.NickName != null)
                {
                    user.UserName = pvm.NickName;
                }
                await _userService.EditUserProfileAsync(user, pvm.Password);
            }
            else
            {
                return View(pvm);
            }

            return RedirectToAction("MyProfile");
        }

        
        public async Task<IActionResult> UserProfile(string userId, int skip = 1, int take = 5)
        {
            var myProfile = await _userService.GetUserProfileAsync(userId, skip, take);
            PaginatedListModel paginatedListModel = new PaginatedListModel(myProfile.countPosts, skip, take);
            PageProfileViewModel viewModel = new PageProfileViewModel()
            {
                PageViewModel = paginatedListModel,
                User = myProfile.user
            };
            return View(viewModel);
        }
    }
}

