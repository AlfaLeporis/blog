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
        private ICaptchaService _captchaService = null;

        public CommentsController(ICommentsService commentsService,
                                  ISecurityService securityService,
                                  ICaptchaService captchaService)
        {
            _commentsService = commentsService;
            _securityService = securityService;
            _captchaService = captchaService;
        }

        [HttpPost]
        public ActionResult AddComment(FormCollection viewModel, TargetType target)
        {
            int articleID = Convert.ToInt32(viewModel["ArticleID"]);
            String content = viewModel["CommentContent"];

            if(!_securityService.IsLogged())
            {
                var recaptcha_challenge_field = viewModel["recaptcha_challenge_field"];
                var recaptcha_response_field = viewModel["recaptcha_response_field"];
                var result = _captchaService.ValidateCode(recaptcha_response_field, recaptcha_challenge_field);
                if (!result)
                    return Content("Podany kod nie jest poprawny!");
            }

            if (content != String.Empty)
            {
               
                var authorName = String.Empty;
                if (viewModel.AllKeys.Contains("AuthorName"))
                {
                    authorName = viewModel["AuthorName"];
                }
                else
                {
                    var authorViewModel = _securityService.Get(_securityService.GetCurrentID());
                    authorName = authorViewModel.Name;
                }

                var commentVM = new CommentViewModel()
                {
                    ArticleID = articleID,
                    Content = content,
                    PublishDate = DateTime.Now,
                    AuthorID = _securityService.GetCurrentID(),
                    Target = target,
                    AuthorName = authorName
                };

                bool result = _commentsService.Add(commentVM);

                if (!result)
                    throw new Exception("Błąd w trakcie próby dodania komentarza");
            }
            else
            {
                return Content("Pole komentarza musi mieć zawartość!");
            }

            return JavaScript("location.reload(true);");
        }

        [HttpGet]
        public ActionResult RemoveComment(int id, int articleID)
        {
            var comment = _commentsService.Get(id);
            if (comment.AuthorID != _securityService.GetCurrentID())
                throw new Exception("Brak uprawnień.");

            var result = _commentsService.Remove(id);

            if (!result)
                throw new Exception("Komentarz o podanym id (" + id + ") nie może zostać usunięty ponieważ nie istnieje.");

            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }
	}
}