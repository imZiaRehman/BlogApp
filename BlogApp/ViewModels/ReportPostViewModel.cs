using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogApp.ViewModels
{
    public class ReportPostViewModel
    {
        public PostViewModel Post { get; set; }

        public int UserId { get; set; }
        public int PostId { get; set; }
        [Required(ErrorMessage ="Reason is Required Field.") ]
        public string Reason { get; set; }

        public string RepoeterName { get; set; }
    }
}