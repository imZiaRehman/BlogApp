using BlogApp.Models;
using BlogApp.Repositories;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BlogRepository _blogRepository;

        public HomeController()
        {
            _blogRepository = new BlogRepository(new Data.BlogDbContext());
        }

        [HttpGet]
        public ActionResult Index()
        {

            var authenticatedUser = (User)Session["AuthenticatedUser"];
            if (authenticatedUser != null)
            {
                var posts = _blogRepository.GetAllPosts();
                return View(posts);

                //return View();
            }

            //If User is not authenticated redirect to login page.
            return RedirectToAction("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddBlog()
        {
            return null;
        }
    }

  

}