using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class SuggestionReply
    {
        public int id { get; set; }

        public string SuggestionText { get; set; }
        public int userId { get; set; }
        public int SuggestionId { get; set; }
    }
}