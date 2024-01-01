using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public int PostId { get; set; }

    }
}