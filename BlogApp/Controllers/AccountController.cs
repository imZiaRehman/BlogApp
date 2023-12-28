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
                    var user = new User
                    {
                        Email = newUser.Email,
                        Password = newUser.Password,
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
            // Validate login credentials and redirect accordingly
            var user = _userRepository.GetUserByEmail(loginUser.Email);

            if (user != null && user.Password == loginUser.Password)
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

                // Successful login, redirect to home
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login credentials");
                return View(loginUser);
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // Redirect to the login page
            return RedirectToAction("Login", "Account");
        }

    }

}