using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.DAL;
using Blog.ViewModels;
using Blog.Models;
using Blog.App_Start;
using Omu.ValueInjecter;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Services
{
    public class CommentsService : ICommentsService
    {
        private ISecurityService _securityService = null;

        public CommentsService(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public bool Add(CommentViewModel viewModel)
        {
            using (var db = new DatabaseContext())
            {
                var model = new CommentModel();
                model.InjectFrom(viewModel);

                db.Comments.Add(model);
                db.SaveChanges();
            }

            return true;
        }

        public bool Edit(CommentViewModel viewModel)
        {
            using (var db = new DatabaseContext())
            {
                var model = db.Comments.FirstOrDefault(p => p.ID == viewModel.ID);

                if (model == null)
                    return false;

                model.InjectFrom(viewModel);

                db.SaveChanges();
            }

            return true;
        }

        public bool Remove(int id)
        {
            using (var db = new DatabaseContext())
            {
                var model = db.Comments.FirstOrDefault(p => p.ID == id);

                if (model == null)
                    return false;

                db.Comments.Remove(model);

                db.SaveChanges();
            }

            return false;
        }

        public CommentViewModel Get(int id)
        {
            using (var db = new DatabaseContext())
            {
                var model = db.Comments.FirstOrDefault(p => p.ID == id);

                if (model == null)
                    return null;

                var viewModel = new CommentViewModel();
                viewModel.InjectFrom<CustomInjection>(model);
                viewModel.AvatarSource = GetAvatarSourceByEMail(_securityService.GetEMailByID(viewModel.AuthorID));
                viewModel.AuthorName = _securityService.GetUserNameByID(viewModel.AuthorID);

                return viewModel;
            }
        }

        public List<CommentViewModel> GetAll()
        {
            using (var db = new DatabaseContext())
            {
                var comments = db.Comments.ToList();
                var viewModels = new List<CommentViewModel>();

                for (int i = 0; i < comments.Count; i++)
                {
                    var vm = new CommentViewModel();
                    vm.InjectFrom<CustomInjection>(comments[i]);
                    vm.AvatarSource = GetAvatarSourceByEMail(_securityService.GetEMailByID(vm.AuthorID));
                    vm.AuthorName = _securityService.GetUserNameByID(vm.AuthorID);

                    viewModels.Add(vm);
                }

                return viewModels;
            }
        }


        public List<CommentViewModel> GetByTargetID(int id, CommentTarget target)
        {
            using(var db = new DatabaseContext())
            {
                var viewModels = new List<CommentViewModel>();
                var comments = db.Comments.Where(p => p.ArticleID == id && p.Target == target).ToList();

                for (int i = 0; i < comments.Count; i++)
                {
                    var vm = new CommentViewModel();
                    vm.InjectFrom<CustomInjection>(comments[i]);

                    vm.AvatarSource = GetAvatarSourceByEMail(_securityService.GetEMailByID(vm.AuthorID));
                    vm.AuthorName = _securityService.GetUserNameByID(vm.AuthorID);

                    viewModels.Add(vm);
                }

                return viewModels;
            }
        }

        private String GetAvatarSourceByEMail(String email)
        {
            email = email.ToLower();

            var hash = MD5.Create();
            var bytes = ASCIIEncoding.ASCII.GetBytes(email);

            var bytesHash = hash.ComputeHash(bytes);
            String emailHash = BitConverter.ToString(bytesHash).Replace("-", "").ToLower();

            return "http://www.gravatar.com/avatar/" + emailHash;
        }
    }
}