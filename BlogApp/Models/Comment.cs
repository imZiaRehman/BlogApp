using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        [Required(ErrorMessage = "CommentText is required.")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public CommentStatus _CommentStatus { get; set; }
        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; } // Reference to the parent comment

        public virtual ICollection<Models.CommentAttachment> commentAttachments { get; set; }
        public virtual ICollection<CommentLike> Likes { get; set; }

        // Navigation property for the parent comment
        public virtual Comment ParentComment { get; set; }

        // Navigation property for child comments
        public virtual ICollection<Comment> ChildComments { get; set; }
    }

    
    public enum CommentStatus
    {
        Reported,
        Deleted,
        DeletedAfterReport,
        Live
    }
}