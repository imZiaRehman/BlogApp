using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class CommentAttachment
    {
        public int CommentAttachmentId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        //Forign Key
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}