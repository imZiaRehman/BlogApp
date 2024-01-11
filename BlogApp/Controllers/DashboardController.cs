using BlogApp.Repositories;
using BlogApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {

        private readonly BlogRepository _blogRepository;
        public DashboardController()
        {
            _blogRepository = new BlogRepository(new Data.BlogDbContext());
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            // Get the most recent posts, comments, and likes from the repository

            var recentPosts = _blogRepository.GetMostRecentPosts(5);
            var recentComments = _blogRepository.GetMostRecentComments(5);
            var recentLikes = _blogRepository.GetMostRecentLikes(5);

            // Create the dashboard view model
            var dashboardViewModel = new DashboardViewModel
            {
                RecentPosts = recentPosts,
                RecentComments = recentComments,
                RecentLike = recentLikes
            };

            return View(dashboardViewModel);

        }


    }
}