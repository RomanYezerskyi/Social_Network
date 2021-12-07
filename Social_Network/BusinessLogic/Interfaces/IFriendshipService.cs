using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace BusinessLogic.Interfaces
{
    public interface IFriendshipService
    {
        Task AddFriendAsync(string friendId, string mainUserId);
        Task RequestForFriendshipAsync(string friendId, string mainUserId);
        Task<(IEnumerable<Friendship> friends, int count)> GetFriendsAsync(string mainUserId, int page, int limit);
        Task<(IEnumerable<Friendship> friends, int count)> SearchFriendsAsync(string mainUserId, int page,
            int limit, string search = null );
        Task<(IEnumerable<Friendship> requests, int count)> GetFriendRequestsToUserAsync(string userId, int page,
            int limit);
        Task<(IEnumerable<Friendship> requests, int count)> GetFriendRequestsFromUserAsync(string userId, int page,
            int limit);
        Task DeleteFromFriend(string friendId, string mainUserId);
        Task<(IEnumerable<User> users, int count)> GetUsersAsync(string userId, int page, int limit);
        Task<(IEnumerable<User> users, int count)> SearchUsersAsync(int page, int limit, string search = null,
            string sortOrder = null, string userId = null);
    }
}
