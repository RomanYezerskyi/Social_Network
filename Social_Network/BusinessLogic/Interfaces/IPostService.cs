using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Interfaces
{
    public interface IPostService
    {
        Task<(IEnumerable<Post> posts, int count)> GetFriendsPostsAsync(string userId, int page, int limit);
        Task<Post> GetPostAsync(int postId, string userId);
        Task CreatePostAsync(string userId, string content, ICollection<IFormFile> files);
        Task EditPostAsync(string userId, int postId, string content, ICollection<IFormFile> modelNewImages);
        Task DeletePost(int postId);
    }
}
