using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class CommentLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int commentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}