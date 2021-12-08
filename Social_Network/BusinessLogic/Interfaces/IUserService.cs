using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
        Task<(IEnumerable<User> users, int count)> GetAllAsync(int page, int limit, string search);
        Task EditUserProfileAsync(User user, string password);
        Task<(User user, int countPosts)> GetUserProfileAsync(string userId, int? skip, int take);

        //Admin
        Task CreateUserAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}
