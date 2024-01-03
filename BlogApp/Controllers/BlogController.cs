using BlogApp.Repositories;
using BlogApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogApp.Models;
using BlogApp.App_Start;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mail;
using System.Xml;

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
        public ActionResult LikePost(PostViewModel postViewModel)
        {
            // Assuming you have a method to get the currently logged-in user
            var currentUser = (User)Session["AuthenticatedUser"];

            // Check if the user has already liked the post
            var alreadyLiked = _blogRepository.HasUserLikedPost(currentUser.UserId, postViewModel.PostId);

            if(!alreadyLiked)
            {
                _blogRepository.AddLike(currentUser.UserId, postViewModel.PostId);
            } 
            else
            {
                _blogRepository.UnlikePost(currentUser.UserId, postViewModel.PostId);
            }

            // Redirect or return a response as needed
            return RedirectToAction("Details", new { id = postViewModel.PostId });
        }

        public ActionResult AddCommentToPost(CommentViewModel commentViewModel, HttpPostedFileBase attachment)
        {
            if(ModelState.IsValid)
            {
                var currentUser = (User)Session["AuthenticatedUser"];
                if (attachment != null && attachment.ContentLength > 0)
                {
                    // Use Cloudinary to upload the attachment
                    var uploadResult = CloudinaryConfig.CloudinaryInstance.Upload(new CloudinaryDotNet.Actions.ImageUploadParams
                    {
                        File = new CloudinaryDotNet.Actions.FileDescription(attachment.FileName, attachment.InputStream)
                    });

                    // Retrieve the public URL of the uploaded image
                    string attachmentUrl = uploadResult.Url.ToString();

                    // Pass the attachment URL to your repository method
                    _blogRepository.AddCommentWithAttachment(currentUser.UserId, commentViewModel.PostId, commentViewModel.CommentText, attachmentUrl, attachment.FileName);
                }
                else
                {
                    // No attachment provided, pass null or handle accordingly
                    _blogRepository.AddComment(currentUser.UserId, commentViewModel.PostId, commentViewModel.CommentText);
                }

                // Redirect or return a response as needed
                return RedirectToAction("Details", new { id = commentViewModel.PostId });
            } else
            {
                return RedirectToAction("Details", new { id = commentViewModel.PostId });
                //return View(commentViewModel);
            }
        }
    }
}
