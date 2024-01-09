using BlogApp.Data;
using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Repositories
{
    public class UserRepository
    {
        private readonly BlogDbContext _context;

        public UserRepository(BlogDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        // Function to get all users
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        public void UpdateUserStatus(int userId, CurrentStatus newStatus)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.currentStatus = newStatus;
                _context.SaveChanges();
            }
        }
        public void UpdateUserRole(int userId, UserRole newRole)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.Role = newRole;
                _context.SaveChanges();
            }
        }
    }
}