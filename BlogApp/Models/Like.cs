using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class Like
    {
        public int LikeId { get; set; }

        // Foreign key
        public int PostId { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}