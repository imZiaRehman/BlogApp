using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BlogApp.Models;
namespace BlogApp.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        [Required (ErrorMessage ="Comment Content is required.")]
        public string CommentText { get; set; }

        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }

        public string PostTitle {  get; set; }

        public virtual ICollection<Models.CommentAttachment> commentAttachments { get; set; }

    }
}