using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.ViewModels
{
    public class LikeViewModel
    {
        public string UserName { get; set; }
        public string PostTitle { get; set; }
        public int PostId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}