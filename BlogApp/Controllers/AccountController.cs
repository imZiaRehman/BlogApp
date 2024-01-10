using BlogApp.Data;
using BlogApp.Email;
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
        private readonly EmailService _emailService;
        public AccountController()
        {
            _userRepository = new UserRepository(new Data.BlogDbContext());
            _emailService = new EmailService();
        }

        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        private static string GenerateToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            // Replace '-' with '+' and '_' with '/' to make the token URL-safe
            return token.Replace('-', '+').Replace('_', '/');

        }


        [HttpPost]
        public ActionResult Signup(UserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.GetUserByEmail(newUser.Email) == null)
                {
                    string token = GenerateToken();
                    //Create user object and save it in database.
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                    var user = new User
                    {
                        Email = newUser.Email,
                        Password = hashedPassword,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Role = UserRole.User,
                        EmailConfirmationToken = token
                    };

                    _userRepository.AddUser(user);

                    //Send user an email confirmation link.
                    

                    var confirmationLink = $"https://localhost:44362/Account/ConfirmEmail?token={HttpUtility.UrlEncode(token)}";
                    _emailService.SendConfirmationEmail(newUser.Email, confirmationLink);

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
            if (IsUserLockedOut())
            {
                ModelState.AddModelError("", "Account Looked. Please try again later.");
                return View(loginUser);
            }
            // Validate login credentials and redirect accordingly
            var user = _userRepository.GetUserByEmail(loginUser.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
            {
                if (user.currentStatus == CurrentStatus.DeActivatedByAdmin || user.currentStatus == CurrentStatus.DeActivated)
                {
                    ModelState.AddModelError("", "Account Deactivated");
                    return View(loginUser);
                }

                if(user.IsEmailConfirmed == false)
                {
                    ModelState.AddModelError("", "Please your email for Email Confirmation!");
                    return View(loginUser);
                }

                var authenticatedUser = new User
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                    FirstName = user.FirstName,
                };

                //On successfull login reset failed attempts.
                Session["FailedLoginAttempts"] = 0;
                // Storing user information in the session
                Session["AuthenticatedUser"] = authenticatedUser;

                FormsAuthentication.SetAuthCookie(user.Email, false);
                System.Diagnostics.Debug.WriteLine("User Role: " + authenticatedUser.Role);


                //On Successfull login redirect to Home page As per the Role of the User.
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
            else
            {
                int failedAttempts = (int)(Session["FailedLoginAttempts"] ?? 0);
                failedAttempts++;
                Session["FailedLoginAttempts"] = failedAttempts;

                if (failedAttempts >= 5)
                {
                    Session["LoginLockoutExpiration"] = DateTime.Now.AddMinutes(5);
                }

                ModelState.AddModelError("", "Invalid login credentials");
                return View(loginUser);
            }
            
        }
        private bool IsUserLockedOut()
        {
            var lockoutExpiration = Session["LoginLockoutExpiration"] as DateTime?;
            return lockoutExpiration.HasValue && lockoutExpiration > DateTime.Now;
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

        [HttpGet]
        public ActionResult ConfirmEmail(string token)
        {
            try
            {
                // Decode the URL-encoded token
                string decodedToken = HttpUtility.UrlDecode(token);

                // Validate and extract the time and key parts of the token
                byte[] data = Convert.FromBase64String(decodedToken);
                DateTime timestamp = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                Guid key = new Guid(data.Skip(sizeof(long)).ToArray());

                // Check if the token has expired (adjust the expiration time as needed)
                //if (DateTime.UtcNow.Subtract(timestamp).TotalHours > 24)
                //{
                // Token has expired
                //  return View("TokenExpired"); // You can create a TokenExpired view
                //}

                // Find the user associated with the key
                var user = _userRepository.GetUserByToken(token);
                if (user == null)
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
                _userRepository.ConfirmUserToken(token);

                // Redirect to a success or login page
                return View("ConfirmEmailSuccess");
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

    }

}