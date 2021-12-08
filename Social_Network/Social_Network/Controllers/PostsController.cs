using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Social_Network.Models;
using Social_Network.ViewModels;

namespace Social_Network.Controllers
{
    [Authorize(Roles = "User")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly int _sizeLimit = 5;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel postModel)
        {
            if (postModel.Content != null)
            {
                await _postService.CreatePostAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value, postModel.Content, postModel.Images);
            }
            return RedirectToAction("MyProfile", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            await _postService.DeletePost(postId);
            return RedirectToAction("MyProfile", "Profile");
        }


        public async Task<IActionResult> EditPost(int postId)
        {
            var post = await _postService.GetPostAsync(postId, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (post == null)
            {
                return NotFound();
            }
            EditPostViewModel model = new EditPostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                Images = post.Images,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostViewModel model)
        {
            var post = await _postService.GetPostAsync(model.Id, User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (ModelState.IsValid)
            {
                if (post != null)
                    await _postService.EditPostAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value, model.Id,
                        model.Content, model.NewImages);
            }
            return RedirectToAction("MyProfile", "Profile");
        }
 
        public async Task<IActionResult> FriendsNews(int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
            var posts = await _postService.GetFriendsPostsAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value, page: pageNum, limit: _sizeLimit);
            PaginatedListModel paginatedListModel = new PaginatedListModel(posts.count, pageNum, _sizeLimit);
            PagePostsViewModel viewModel = new PagePostsViewModel()
            {
                PageViewModel = paginatedListModel,
                Posts = posts.posts
            };
            return View(viewModel);
        }
    }
}
