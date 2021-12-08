using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_Network.Models;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private readonly IRolesService _rolesService;
        private readonly IUserService _userService;
        private readonly int _sizeLimit = 7;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IRolesService rolesService, IUserService userService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _rolesService = rolesService;
            _userService = userService;
        }
        public async Task<IActionResult> Roles() => View(await _rolesService.GetRoles());
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _rolesService.Create(name);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityResult result = await _rolesService.Delete(id);
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Roles");
        }

        public async Task<IActionResult> UserList(string search, int pageNum = 1)
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

        public async Task<IActionResult> Edit(string userId)
        {
            User user = await _userService.GetByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _rolesService.GetUserRoles(user);
                var allRoles = await _rolesService.GetRoles();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                { 
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles.ToList()
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            User user = await _userService.GetByIdAsync(userId);
            if (user != null)
            {
                await _rolesService.EditUserRoles(user, roles);
                return RedirectToAction("UserList");
            }
            return NotFound();
        }
    }
}