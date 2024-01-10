using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }

        public CurrentStatus currentStatus { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string EmailConfirmationToken { get; set; }


    }
    public enum UserRole
    {
        Admin,
        Moderator,
        User
    }

    public enum CurrentStatus
    {
        Active,
        DeActivated,
        DeActivatedByAdmin,
    }
}