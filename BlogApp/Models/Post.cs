using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BlogApp.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public Status PostStatus { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public virtual User user { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }

    }

    public enum Status
    {
        PendingApproval,
        Reported,
        Rejected,
        Deleted,
        DeletedAfterReport,
        Live
    }
}