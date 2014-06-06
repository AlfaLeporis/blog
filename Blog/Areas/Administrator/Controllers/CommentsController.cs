using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.Practices.Unity;

namespace Blog.Areas.Administrator.Controllers
{
    public class CommentsController : Controller
    {
        private ICommentsService _commentsService = null;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpGet]
        public ActionResult Comments()
        {
            var comments = _commentsService.GetAll();
            return View(comments);
        }

        [HttpGet]
        public ActionResult RemoveComment(int id)
        {
            bool result = _commentsService.Remove(id);

            if (!result)
                throw new HttpException(404, "Usunięcie komentarza (" + id + ") nie powiodło się.");

            return RedirectToAction("Comments");
        }

        [HttpGet]
        public ActionResult EditComment(int id)
        {
            var viewModel = _commentsService.Get(id);

            if (viewModel == null)
                throw new HttpException(404, "Dany komentarz (" + id + ") nie nie istnieje.");

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditComment(CommentViewModel viewModel)
        {
            bool result;
            if (viewModel.ID.HasValue)
            {
                var comment = _commentsService.Get(viewModel.ID.Value);
                comment.Content = viewModel.Content;
                result = _commentsService.Edit(comment);
            }
            else
                result = _commentsService.Add(viewModel);

            if (result)
            {
                return RedirectToAction("Comments");
            }
            else
            {
                TempData.Add("ErrorMsg", "Wystąpił błąd podczas próby edycji komentarza. Spróbuj ponownie.");
                return View(viewModel);
            }
        }
	}
}