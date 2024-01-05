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
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly BlogRepository _blogRepository;

        public AdminController()
        {
            _blogRepository = new BlogRepository(new Data.BlogDbContext());
        }

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

        public ActionResult ManagePost()
        {
            var posts = _blogRepository.GetAllPendingPosts();
            return View(posts);
        }

        public ActionResult Details(int id)
        {
            var post = _blogRepository.GetPostById(id);
            return View(post);
        }

        public ActionResult ReportedPost()
        {
            var reportedPost = _blogRepository.GetReportedDetails();
            return View(reportedPost);
        }
    }
}