using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class ReportComment
    {
        public int Id { get; set; }
        public int CommenntId { get; set; }
        public int UserId { get; set; }
        public string ReportReason { get; set; }
        public DateTime ReportedAt { get; set; }

    }
}