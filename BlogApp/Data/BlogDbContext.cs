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
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }

        public DbSet<Comment> Comments { get; set; }
                                    
        public DbSet<CommentAttachment> CommentsAttachment { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<ReportPost> ReportPosts { get; set; }

        public DbSet<CommentLike> CommentLikes { get; set; }

        public DbSet<ReportComment> ReportComments { get; set; }

        public DbSet<PostSuggestion> PostSuggestions { get; set; }

        public DbSet<SuggestionReply> SuggestionReplies { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the model, e.g., set the primary key, unique constraints, etc.
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            base.OnModelCreating(modelBuilder);
        }

    }
}