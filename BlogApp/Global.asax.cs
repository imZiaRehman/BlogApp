using BlogApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Services.Description;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BlogApp.App_Start;


namespace BlogApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configure Cloudinary
            Account account = new Account(
                "",
                "",
                ""
            );

            var cloudinary = new Cloudinary(account);

            CloudinaryConfig.CloudinaryInstance = cloudinary;

        }
    }
}
