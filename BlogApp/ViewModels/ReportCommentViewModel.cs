using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogApp.ViewModels
{
    public class ReportCommentViewModel
    {
        public CommentViewModel Comment { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Reason is Required Field.")]
        public string Reason { get; set; }
        public string RepoeterName { get; set; }
    }
}