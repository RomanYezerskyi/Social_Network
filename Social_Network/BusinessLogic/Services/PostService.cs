using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImagesService _imagesService;

        public PostService(IUnitOfWork unitOfWork, IImagesService imagesService)
        {
            _unitOfWork = unitOfWork;
            _imagesService = imagesService;
        }
        public async Task<(IEnumerable<Post> posts, int count)> GetFriendsPostsAsync(string userId, int page, int limit)
        {
            int count = await _unitOfWork.Posts.GetCountAsync(filter: x => (x.User.Friends.Any(x => (x.UserMainId == userId) && x.Status == FriendshipStatus.Accepted)
                                                                            || x.User.FriendsIAdded.Any(x => (x.FriendId == userId) && x.Status == FriendshipStatus.Accepted)));

            var posts = await _unitOfWork.Posts.GetAsync(page: page, limit: limit,
                filter: x => (x.User.Friends.Any(x=>(x.UserMainId == userId) && x.Status == FriendshipStatus.Accepted) 
                              || x.User.FriendsIAdded.Any(x => (x.FriendId == userId) && x.Status == FriendshipStatus.Accepted)), 
                    includes: i => i.Include(u => u.User).Include(p => p.Images), 
                orderBy: orderBy => orderBy.OrderByDescending(x => x.TimeStamp));
            
            return (posts, count);
        }

        public async Task<Post> GetPostAsync(int postId, string userId)
        {
            var post = await _unitOfWork.Posts.GetAsync(filter: x => x.UserId == userId && x.Id == postId,
                includes: include => include.Include(i => i.Images));
            return post;
        }

        public async Task CreatePostAsync(string userId, string content, ICollection<IFormFile> files)
        {
            var post = new Post
            {
                UserId = userId,
                Content = content,
                TimeStamp = DateTime.Now
            };
            await _unitOfWork.Posts.InsertAsync(post);
            if (files.Count > 0)
            {
                var collectionImages = await _imagesService.AddImagesToPostAsync(files, post.Id);
                post.Images = collectionImages;
                await _unitOfWork.Posts.UpdateAsync(post);
            }
        }

        public async Task EditPostAsync(string userId, int postId, string content, ICollection<IFormFile> formFiles)
        {
            var post = await GetPostAsync(postId, userId);
            if (post != null)
            {
                if (content != null)
                {
                    post.Content = content;
                }
                post.TimeStamp = DateTime.Now;
                if (formFiles != null)
                {
                    var collection = await _imagesService.AddImagesToPostAsync(formFiles, post.Id);
                    post.Images = collection;
                }
                await _unitOfWork.Posts.UpdateAsync(post);
            }
        }

        public async Task DeletePost(int postId)
        {
            await _unitOfWork.Posts.DeleteAsync(postId);
        }
    }
}