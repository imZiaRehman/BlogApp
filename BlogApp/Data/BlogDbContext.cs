using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BlogApp.Data
{
    public class BlogDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the model, e.g., set the primary key, unique constraints, etc.
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            base.OnModelCreating(modelBuilder);
        }

    }
}