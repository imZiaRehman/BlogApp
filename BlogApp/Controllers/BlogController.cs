using BlogApp.Repositories;
using BlogApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class BlogController : Controller
    {

        private readonly BlogRepository _blogRepository;
        public BlogController()
        {
            _blogRepository = new BlogRepository(new Data.BlogDbContext());
        }
        public ActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBlog(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var authenticatedUser = (User)Session["AuthenticatedUser"];

                // Map the PostViewModel to a Post entity and add it to the repository
                var post = new Post
                {
                    Title = postViewModel.Title,
                    Content = postViewModel.Content,
                    CreatedAt = DateTime.Now, // Set the creation timestamp
                    UserId = authenticatedUser.UserId,
                };

                _blogRepository.AddPost(post);

                return RedirectToAction("Index", "Home"); // Redirect to the home page after successful creation
            }

            return View(postViewModel);
        }

        public ActionResult Details(int id)
        {
            var post = _blogRepository.GetPostById(id);
            return View(post);
        }

        [HttpPost]
        public ActionResult LikePost(int postId)
        {
            // Assuming you have a method to get the currently logged-in user
            var currentUser = (User)Session["AuthenticatedUser"];

            // Check if the user has already liked the post
            var alreadyLiked = _blogRepository.HasUserLikedPost(currentUser.UserId,postId);

            if(!alreadyLiked)
            {
                _blogRepository.AddLike(currentUser.UserId, postId);
            } 
            else
            {
                _blogRepository.UnlikePost(currentUser.UserId,postId);
            }

            // Redirect or return a response as needed
            return RedirectToAction("Details", new { postId = postId });
        }

    }
}
