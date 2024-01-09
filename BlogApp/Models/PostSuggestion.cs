using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class PostSuggestion
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string SuggestionText { get; set; }

        public SuggestionStatus suggestionStatus { get; set; }

        public virtual ICollection<SuggestionReply> replies { get; set; }

    }

    public enum SuggestionStatus
    {
        Live,
        Accepted,
        Rejected
    }
}