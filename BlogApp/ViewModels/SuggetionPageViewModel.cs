using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogApp.ViewModels
{
    public class SuggetionPageViewModel
    {
        public PostViewModel Post { get; set; }
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage = "Suggestion Text is Required Field.")]
        public string SuggestionText { get; set; }

        public string UserName { get; set; }
        public SuggestionStatus suggestionStatus { get; set; }

        public virtual ICollection<SuggestionReply> replies { get; set; }

    }
}