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
    [Authorize]
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
                    PostStatus = Status.PendingApproval,
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

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult AdminPostDetails(int id)
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

            if(currentUser.Role == UserRole.User)
            {
                return RedirectToAction("Details", new { id = postViewModel.PostId });
            } 
            else
            {
                return RedirectToAction("AdminPostDetails", new { id = postViewModel.PostId });

            }
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
                    _blogRepository.AddCommentWithAttachment(currentUser.UserId, commentViewModel.PostId, commentViewModel.CommentText, attachmentUrl, attachment.FileName, commentViewModel.ParentCommentId);
                }
                else
                {
                    // No attachment provided, pass null or handle accordingly
                    _blogRepository.AddComment(currentUser.UserId, commentViewModel.PostId, commentViewModel.CommentText, commentViewModel.ParentCommentId);
                }

                // Redirect or return a response as needed
                return RedirectToAction("Details", new { id = commentViewModel.PostId });
            } else
            {
                return RedirectToAction("Details", new { id = commentViewModel.PostId });
                //return View(commentViewModel);
            }
        }

        [Authorize(Roles = "Admin,Moderator")]

        public ActionResult ApprovePost(int postId)
        {
            _blogRepository.ApprovePost(postId);

            return RedirectToAction("Index", "Admin");
        }

        [Authorize(Roles = "Admin,Moderator")]

        public ActionResult RejectPost(int postId)
        {
            _blogRepository.RejectPost(postId);

            return RedirectToAction("Index", "Admin");

        }

        public ActionResult ReportPostPage(int postId)
        {
            var postViewModel = _blogRepository.GetPostById(postId);

            if (postViewModel == null)
            {
                return HttpNotFound("Post not found");
            }
            var user =(User)Session["AuthenticatedUser"];
            var reportPostViewModel = new ReportPostViewModel
            {
                Post = postViewModel,
                UserId = user.UserId,
                PostId = postId
            };

            return View(reportPostViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportPostPage(ReportPostViewModel reportPostViewModel)
        {

            if (ModelState.IsValid)
            {
                _blogRepository.ReportPost(
                    reportPostViewModel.PostId,
                    reportPostViewModel.UserId,
                    reportPostViewModel.Reason
                );
                var authenticatedUser = (User)Session["AuthenticatedUser"];

                if (authenticatedUser.Role == UserRole.User)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (authenticatedUser.Role == UserRole.Admin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            reportPostViewModel.Post = _blogRepository.GetPostById(reportPostViewModel.PostId);

            return View(reportPostViewModel);
        }

        public ActionResult AddSuggestions(int postId)
        {
            var postViewModel = _blogRepository.GetPostById(postId);

            if (postViewModel == null)
            {
                return HttpNotFound("Post not found");
            }
            var user = (User)Session["AuthenticatedUser"];
            var suggestionPostViewModel = new SuggetionPageViewModel
            {
                Post = postViewModel,
                UserId = user.UserId,
                PostId = postId
            };

            return View(suggestionPostViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSuggestions(SuggetionPageViewModel suggetionPostViewModel)
        {

            if (ModelState.IsValid)
            {
                _blogRepository.AddSuggestion(
                    suggetionPostViewModel.PostId,
                    suggetionPostViewModel.UserId,
                    suggetionPostViewModel.SuggestionText
                );

                var authenticatedUser = (User)Session["AuthenticatedUser"];

                if (authenticatedUser.Role == UserRole.User)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (authenticatedUser.Role == UserRole.Admin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            suggetionPostViewModel.Post = _blogRepository.GetPostById(suggetionPostViewModel.PostId);
            // If the model state is not valid, return to the report view with the model
            return View(suggetionPostViewModel);
        }


        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult AcceptReportToPost(int postId)
        {
            // Implement logic to accept the report and take necessary actions
            _blogRepository.AcceptReportToPost(postId);

            return RedirectToAction("ReportedPost", "Admin");
        }

        [Authorize(Roles = "Admin,Moderator")]

        public ActionResult RejectReportToPost(int postId)
        {
            // Implement logic to accept the report and take necessary actions
            _blogRepository.ApprovePost(postId);

            return RedirectToAction("ReportedPost", "Admin");
        
        }

        //TODO: To be implemented.
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult AcceptReportedComment(int commentId)
        {
            _blogRepository.AcceptReportedComment(commentId);
            return RedirectToAction("ReportedPost", "Admin");
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult RejectReportedComment(int commentId)
        {
            _blogRepository.RejectReportedComment(commentId);

            return RedirectToAction("ReportedPost", "Admin");
        }

        public ActionResult LikeToComment(int commentId, int postId)
        {
            var currentUser = (User)Session["AuthenticatedUser"];
            var alreadyLiked = _blogRepository.HasUserLikedComment(currentUser.UserId, commentId);

            if (!alreadyLiked)
            {
                _blogRepository.AddLikeToComment(commentId, currentUser.UserId);
            } else
            {
                _blogRepository.RemoveLikeToComment(commentId, currentUser.UserId);
            }

            return RedirectToAction("Details", new { id = postId });
        }        

        public ActionResult ReportCommentPage(int commentId)
        {
            var commentViewModel = _blogRepository.GetCommentById(commentId);

            if (commentViewModel == null)
            {
                return HttpNotFound("Post not found");
            }
            var user = (User)Session["AuthenticatedUser"];
            var reportCommentViewModel = new ReportCommentViewModel
            {
                Comment = commentViewModel,
                UserId = user.UserId,
                CommentId = commentId
            };

            return View(reportCommentViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportCommentPage(ReportCommentViewModel reportCommentViewModel)
        {

            if (ModelState.IsValid)
            {
                _blogRepository.ReportComment(
                    reportCommentViewModel.CommentId,
                    reportCommentViewModel.UserId,
                    reportCommentViewModel.Reason
                );

                return RedirectToAction("Index", "Home");
            }

            reportCommentViewModel.Comment = _blogRepository.GetCommentById(reportCommentViewModel.CommentId);
            // If the model state is not valid, return to the report view with the model
            return View(reportCommentViewModel);
        }

        public ActionResult UserSuggestionSend()
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            var suggestions = _blogRepository.GetSuggestionsGivenByUser(currentUser.UserId);

            return View(suggestions);

        }

        public ActionResult UserSuggestionReceived()
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            var suggestions = _blogRepository.GetSuggestionsReceivedByUser(currentUser.UserId);

            return View(suggestions);
        }

        public ActionResult SuggestionDetails(int id)
        {
            
            SuggetionPageViewModel suggestion = _blogRepository.GetSuggestionById(id);

            if (suggestion == null)
            {
                return HttpNotFound();
            }

            return View(suggestion);            
        }

        public ActionResult SuggestionReceivedDetails(int id)
        { 
            SuggetionPageViewModel suggestion = _blogRepository.GetSuggestionById(id);

            if (suggestion == null)
            {
                return HttpNotFound();
            }

            return View(suggestion);
        }

        public ActionResult RejectSuggestion(int suggetionId)
        {
            _blogRepository.RejectSuggestion(suggetionId);

            var currentUser = (User)Session["AuthenticatedUser"];
            if (currentUser.Role == UserRole.User)
            {
                return RedirectToAction("Index", "Home");
            } else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult MyPost()
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            var posts = _blogRepository.GetPostsByUserId(currentUser.UserId);
            return View(posts);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult DeletePost(PostViewModel postViewModel)
        {

            var currentUser = (User)Session["AuthenticatedUser"];

            _blogRepository.DeactivatePost(postViewModel.PostId);

            if (currentUser.Role == UserRole.User)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult EditPost(int PostId)
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            var post = _blogRepository.GetPostById(PostId);
            
            if(currentUser.UserId == post.UserId)
            {
                return View(post);
            }

            if (currentUser.Role == UserRole.User)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult EditPost( PostViewModel editedPost)
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            // Check if the model is valid
            if (ModelState.IsValid)
            {
                _blogRepository.UpdatePost(editedPost);

                if (currentUser.Role == UserRole.User)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View(editedPost);
        }

        public ActionResult EditSuggestionPost(int PostId, int suggestionId)
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            var post = _blogRepository.GetPostById(PostId);
            _blogRepository.UpdateSuggestionStatus(suggestionId);

            if (currentUser.UserId == post.UserId)
            {
                return View(post);
            }

            if (currentUser.Role == UserRole.User)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult EditSuggestionPost(PostViewModel editedPost)
        {
            var currentUser = (User)Session["AuthenticatedUser"];

            // Check if the model is valid
            if (ModelState.IsValid)
            {
                _blogRepository.UpdatePost(editedPost);

                if (currentUser.Role == UserRole.User)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View(editedPost);
        }

    }
}
