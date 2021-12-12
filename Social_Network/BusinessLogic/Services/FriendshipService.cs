using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Migrations;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FriendshipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddFriendAsync(string friendId, string mainUserId)
        {
            var friends = await _unitOfWork.Friendships.GetAsync(filter: x => x.UserMainId == friendId && x.FriendId == mainUserId);
            if (friends != null)
            {
                friends.Status = FriendshipStatus.Accepted;
                await _unitOfWork.Friendships.UpdateAsync(friends);
            }
        }
        public async Task RequestForFriendshipAsync(string friendId, string mainUserId)
        {
            var friends = new Friendship
            {
                FriendId = friendId,
                UserMainId = mainUserId,
                Status = FriendshipStatus.Request
            };
            await _unitOfWork.Friendships.InsertAsync(friends);
        }
        public async Task<(IEnumerable<Friendship> friends, int count)> GetFriendsAsync(string mainUserId, int page, int limit)
        {
            int count = await _unitOfWork.Friendships.GetCountAsync(filter: x => (x.UserMainId == mainUserId && x.Status == FriendshipStatus.Accepted) ||
                (x.FriendId == mainUserId && x.Status == FriendshipStatus.Accepted));

            var friends = await _unitOfWork.Friendships.GetAsync(page: page, limit: limit,
                filter: x => (x.UserMainId == mainUserId && x.Status == FriendshipStatus.Accepted) ||
                             (x.FriendId == mainUserId && x.Status == FriendshipStatus.Accepted),
                includes: f => f.Include(x => x.Friend).ThenInclude(x => x.Chats)
                    .Include(x => x.UserMain).ThenInclude(x => x.Chats), orderBy: null);

            return (friends, count);
        }

        public async Task<(IEnumerable<Friendship> friends, int count)> SearchFriendsAsync(string mainUserId, int page,
            int limit, string search)
        {

            Func<Friendship, bool> userMainFriend = (x => x.UserMainId == mainUserId && x.Status == FriendshipStatus.Accepted
                                                                                && (x.Friend.Email.Contains(search) || x.Friend.UserName.Contains(search)));
            Func<Friendship, bool> userFriend = (x => x.FriendId == mainUserId && x.Status == FriendshipStatus.Accepted
                                                                                && (x.UserMain.Email.Contains(search) || x.UserMain.UserName.Contains(search)));
            
            Expression<Func<Friendship, bool>> searchExpression = friendship => userMainFriend(friendship) || userFriend(friendship);
            if (search != null)
            {
                page = 1;
                int count = await _unitOfWork.Friendships.GetCountAsync(searchExpression);
                var friends = await _unitOfWork.Friendships.GetAsync(page: page, limit: limit,
                    filter: x => (x.UserMainId == mainUserId && x.Status == FriendshipStatus.Accepted
                                                         && (x.Friend.Email.Contains(search) || x.Friend.UserName.Contains(search)) ||
                                  (x.FriendId == mainUserId && x.Status == FriendshipStatus.Accepted && (x.UserMain.Email.Contains(search) || x.UserMain.UserName.Contains(search)))),
                    includes: f => f.Include(x => x.Friend).ThenInclude(x => x.Chats)
                        .Include(x => x.UserMain).ThenInclude(x => x.Chats), orderBy: null);
                return (friends, count);
            }
            return await GetFriendsAsync(mainUserId, page, limit);
        }
        public async Task<(IEnumerable<Friendship> requests, int count)> GetFriendRequestsToUserAsync(string userId,
            int page, int limit)
        {
            int count = await _unitOfWork.Friendships.GetCountAsync(filter: x => x.FriendId == userId && x.Status == FriendshipStatus.Request);
            var requests =
                await _unitOfWork.Friendships.GetAsync(page: page, limit: limit,
                    filter: x => x.FriendId == userId && x.Status == FriendshipStatus.Request,
                    includes: f => f.Include(x => x.UserMain),
                    orderBy: null);
            return (requests, count);
        }
         
        public async Task<(IEnumerable<Friendship> requests, int count)> GetFriendRequestsFromUserAsync(string userId,
            int page, int limit)
        {
            int count = await _unitOfWork.Friendships.GetCountAsync(filter: x => x.UserMainId == userId && x.Status == FriendshipStatus.Request);
            var requests =
                await _unitOfWork.Friendships.GetAsync(page: page, limit: limit,
                    filter: x => x.UserMainId == userId && x.Status == FriendshipStatus.Request,
                    includes: f => f.Include(x => x.Friend), orderBy: null);
            return (requests, count);
        }

        public async Task DeleteFromFriend(string friendId, string mainUserId)
        {
            var deleteFriend = await _unitOfWork.Friendships.GetAsync(filter: x => (x.UserMainId == mainUserId && x.FriendId == friendId)
                || (x.FriendId == mainUserId && x.UserMainId == friendId));
            await _unitOfWork.Friendships.DeleteAsync(deleteFriend);
        }
        public async Task<(IEnumerable<User> users, int count)> GetUsersAsync(string userId, int page, int limit)
        {
            int count = await _unitOfWork.User.GetCountAsync(filter: x => x.Id != userId && x.Email != "admin@admin");
            var users = await _unitOfWork.User.GetAsync(page: page, limit: limit,
                filter: x => x.Id != userId && x.Email != "admin@admin",
                includes: include =>
                    include.Include(x => x.FriendsIAdded).Include(x => x.Friends),
                orderBy: orderBy => orderBy.OrderBy(u => u.UserName));
            return (users, count);
        }
        public async Task<(IEnumerable<User> users, int count)> SearchUsersAsync(int page, int limit,
            string search = null, string sortOrder = null, string userId = null)
        {
            if (sortOrder != null)
            {
                var sortedUsers = await GetUsersAsync(userId, page, limit);
                if (sortOrder == "Email")
                {
                    sortedUsers.users = sortedUsers.users.OrderBy(x => x.Email);
                }
                else if (sortOrder == "Name")
                {
                    sortedUsers.users = sortedUsers.users.OrderBy(x => x.UserName);
                }
                return sortedUsers;
            }
            if (search != null)
            {
                page = 1;
                int count = await _unitOfWork.User.GetCountAsync(filter: filter => filter.UserName == search || filter.Email == search);
                var users = await _unitOfWork.User.GetAsync(page: page, limit: limit,
                    filter: filter => (filter.Id != userId) &&( filter.UserName.Contains(search) || filter.Email.Contains(search)),
                    includes: include =>
                        include.Include(x => x.FriendsIAdded).Include(x => x.Friends),
                    orderBy: orderBy => orderBy.OrderBy(x => x.UserName));
                return (users, count);
            }
            return await GetUsersAsync(userId, page, limit);
        }
    }
}