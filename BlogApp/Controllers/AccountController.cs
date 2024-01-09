using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Repositories;
using BlogApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace BlogApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController()
        {
            _userRepository = new UserRepository(new Data.BlogDbContext());
        }

        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(UserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.GetUserByEmail(newUser.Email) == null)
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                    var user = new User
                    {
                        Email = newUser.Email,
                        Password = hashedPassword,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Role = UserRole.User
                    };

                    _userRepository.AddUser(user);

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                }
            }

            return View(newUser);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserViewModel loginUser)
        {
            System.Diagnostics.Debug.WriteLine("Debugging message: Inside SomeAction");

            // Validate login credentials and redirect accordingly
            var user = _userRepository.GetUserByEmail(loginUser.Email);

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);

            if (user != null && isPasswordCorrect)
            {
                var authenticatedUser = new User
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                    FirstName = user.FirstName,
                };

                // Storing user information in the session
                Session["AuthenticatedUser"] = authenticatedUser;

                FormsAuthentication.SetAuthCookie(user.Email, false);
                System.Diagnostics.Debug.WriteLine("User Role: " + authenticatedUser.Role);


                //On Successfull login redirect to Home page As per the Role of the User.
                if (authenticatedUser.Role == UserRole.User)
                {
                    return RedirectToAction("Index", "Home");
                } else if (authenticatedUser.Role == UserRole.Admin) {
                    return RedirectToAction("Index", "Admin");
                } else
                {
                    return RedirectToAction("Index", "Admin");
                }

            }
            else
            {
                ModelState.AddModelError("", "Invalid login credentials");
                return View(loginUser);
            }
        }

        public ActionResult Logout()
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            // Redirect to the login page
            return RedirectToAction("Login");
        }

    }

}