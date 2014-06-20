using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Blog.ViewModels;
using Blog.Services;
using Blog.Filters;
using Blog.Models;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        private ICommentsService _commentsService = null;
        private ISecurityService _securityService = null;

        public CommentsController(ICommentsService commentsService,
                                  ISecurityService securityService)
        {
            _commentsService = commentsService;
            _securityService = securityService;
        }

        [HttpPost]
        public ActionResult AddComment(FormCollection viewModel, CommentTarget target)
        {
            int articleID = Convert.ToInt32(viewModel["ArticleID"]);
            String content = viewModel["CommentContent"];

            if (content != String.Empty)
            {
                var commentVM = new CommentViewModel()
                {
                    ArticleID = articleID,
                    Content = content,
                    PublishDate = DateTime.Now,
                    AuthorID = _securityService.GetCurrentID(),
                    Target = target
                };

                bool result = _commentsService.Add(commentVM);

                if (!result)
                    throw new Exception("Błąd w trakcie próby dodania komentarza");
            }
            else
            {
                TempData.Add("ErrorMsg", "Komentarz nie może być pusty!");
            }

            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }

        [HttpGet]
        public ActionResult RemoveComment(int id, int articleID)
        {
            if (id != _securityService.GetCurrentID())
                throw new Exception("Brak uprawnień.");

            var result = _commentsService.Remove(id);

            if (!result)
                throw new Exception("Komentarz o podanym id (" + id + ") nie może zostać usunięty ponieważ nie istnieje.");

            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }
	}
}