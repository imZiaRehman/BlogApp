using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CkEditor.Controllers
{
    public class EditorController : Controller
    {
        // GET: Editor
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string url; // url to return
            string message; // message to display (optional)

            // path of the image
            string path = Server.MapPath("~/Content/Images/Uploads");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            // here logic to upload image
            string ImageName = upload.FileName;
            upload.SaveAs(System.IO.Path.Combine(path, ImageName));


            // and get file path of the image
            // will create http://localhost:9999/Content/Images/Uploads/ImageName.jpg
            url = Request.Url.GetLeftPart(UriPartial.Authority) + "/Content/Images/Uploads/" + ImageName;

            // passing message success/failure
            message = "Image was saved correctly";

            // since it is an ajax request it requires this string
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }

    }
}
