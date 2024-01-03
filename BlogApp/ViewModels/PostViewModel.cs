using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogApp.ViewModels
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        [Required(ErrorMessage ="Title is required.")]
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public bool UserHasLiked {  get; set; }

        public string ShortenedContent => GetShortenedContent();

        private string GetShortenedContent()
        {
            return GetFirstTwoLines(Content);
        }

        private string GetFirstTwoLines(string content)
        {
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Environment.NewLine, lines.Take(2));
        }

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public virtual User user { get; set; }

        public string CommentText { get; set; }
        public virtual ICollection<CommentViewModel> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}