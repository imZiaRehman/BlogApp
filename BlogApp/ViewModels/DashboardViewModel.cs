using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.ViewModels
{
    public class DashboardViewModel
    {
        public List<PostViewModel> RecentPosts { get; set; }
        public List<CommentViewModel> RecentComments { get; set; }

        public List<LikeViewModel> RecentLike { get; set; }
    }
}