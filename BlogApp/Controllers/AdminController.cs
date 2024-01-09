using BlogApp.Models;
using BlogApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace BlogApp.Controllers
{
    //[Authorize]
    
    public class AdminController : Controller
    {

        private readonly BlogRepository _blogRepository;
        private readonly UserRepository _userRepository;

        public AdminController()
        {
            _blogRepository = new BlogRepository(new Data.BlogDbContext());
            _userRepository = new UserRepository(new Data.BlogDbContext());
        }
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Index()
        {
            var authenticatedUser = (User)Session["AuthenticatedUser"];
            if (authenticatedUser != null)
            {
                var posts = _blogRepository.GetAllPosts();
                return View(posts);
            }

            //If User is not authenticated redirect to login page.
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult ManagePost()
        {
            var posts = _blogRepository.GetAllPendingPosts();
            return View(posts);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Details(int id)
        {
            var post = _blogRepository.GetPostById(id);
            return View(post);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult ReportedPost()
        {
            var reportedPost = _blogRepository.GetReportedDetails();
            return View(reportedPost);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult ReportedComments()
        {
            var reportedComment = _blogRepository.GetReportedComments();
            return View(reportedComment);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ManageUsers()
        {
            var users = _userRepository.GetAllUsers();
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public ActionResult UpdateUserStatus(int userId, CurrentStatus newStatus)
        {
            _userRepository.UpdateUserStatus(userId, newStatus);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public ActionResult UpdateUserRole(int userId, UserRole newRole)
        {
            _userRepository.UpdateUserRole(userId, newRole);
            return RedirectToAction("ManageUsers");
        }



    }
}