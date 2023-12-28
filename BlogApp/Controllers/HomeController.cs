using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var authenticatedUser = (User)Session["AuthenticatedUser"];
            if(authenticatedUser == null)
            {
                return View();
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
    }
}