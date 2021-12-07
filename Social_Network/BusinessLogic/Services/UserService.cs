using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<User> _userManager;
        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _unitOfWork.User.GetAsync(filter: x=> x.Id == id);
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _unitOfWork.User.GetAsync(filter: x => x.Email == email);

            return user;
        }

        public async Task<(IEnumerable<User> users, int count)> GetAllAsync(int page, int limit)
        {
            var count = await _unitOfWork.User.GetCountAsync();
            var users =await _unitOfWork.User.GetAsync(orderBy: null, page: page, limit: limit);
            return (users, count);
        }

        public async Task<(User user, int countPosts)> GetUserProfileAsync(string userId, int? skip, int take)
        {
            skip ??= 1;
            int countPosts = await _unitOfWork.Posts.GetCountAsync(filter: x => x.UserId == userId);
            
            var user = await _unitOfWork.User.GetAsync(filter: x => x.Id == userId,
                includes: x => x.Include(y => y.Posts.OrderByDescending(x=>x.TimeStamp).Skip((int)((skip - 1) * take)).Take(take)).ThenInclude(y=>y.Images));
            return (user, countPosts);
        }

        public async Task EditUserProfileAsync(User user, string password)
        {
            if (password != null)
            {
                var hasher = new PasswordHasher<IdentityUser>();
                IdentityUser identityUser = new IdentityUser(user.Id);
                user.PasswordHash = hasher.HashPassword(identityUser, password);
            }
            await _unitOfWork.User.UpdateAsync(user);
        }
        
        //Admin
        public async Task CreateUserAsync(User user, string password)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            IdentityUser identityUser = new IdentityUser(user.Id);
            user.PasswordHash = hasher.HashPassword(identityUser, password);
            await _unitOfWork.User.InsertAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _unitOfWork.User.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            var friends = await 
                _unitOfWork.Friendships.GetAsync(filter: x => x.FriendId == user.Id || x.UserMainId == user.Id, includes: null, orderBy: null);
            if (friends != null)
            {
                await _unitOfWork.Friendships.DeleteAsync(friends);
            }
            await _unitOfWork.User.DeleteAsync(user);
        }
    }
}

