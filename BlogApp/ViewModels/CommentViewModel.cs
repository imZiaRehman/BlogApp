using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }

    }
}