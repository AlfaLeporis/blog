using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.ViewModels;
using System.IO;

namespace Blog.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EditorController : Controller
    {
        public ActionResult FilesBrowser(int CKEditorFuncNum)
        {
            var viewModel = new List<ImageFileViewModel>();
            var path = Server.MapPath("~/Uploaded");

            var files = Directory.GetFiles(path);
            
            for (int i = 0; i < files.Count(); i++)
            {
                var singleFile = new FileInfo(files[i]);
                var file = new ImageFileViewModel()
                {
                    Name = singleFile.Name,
                    Url = "\\Uploaded\\" + singleFile.Name,
                    Size = singleFile.Length
                };

                viewModel.Add(file);
            }

            ViewBag.CKEditorFuncNum = CKEditorFuncNum;

            return View(viewModel);
        }

        public ActionResult Uploader()
        {
            var file = HttpContext.Request.Files[0];
            var path = Server.MapPath("~/Uploaded");

            file.SaveAs(path + "/" + file.FileName);

            return File(file.InputStream, file.ContentType);
        }
	}
}