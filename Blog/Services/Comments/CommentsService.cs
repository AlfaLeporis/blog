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
using System.Data.Entity;
using Blog.Infrastructure;

namespace Blog.Services
{
    public class CommentsService : ICommentsService
    {
        private ISecurityService _securityService = null;
        private DbContext _db = null;

        public CommentsService(ISecurityService securityService,
                               DbContext db)
        {
            _securityService = securityService;
            _db = db;
        }

        public bool Add(CommentViewModel viewModel)
        {
            var model = new CommentModel();
            model.IsRemoved = false;
            model.InjectFrom<CustomInjection>(viewModel);

            _db.Set<CommentModel>().Add(model);
            _db.SaveChanges();

            return true;
        }

        public bool Edit(CommentViewModel viewModel)
        {
            var model = _db.Set<CommentModel>().FirstOrDefault(p => p.ID == viewModel.ID);

            if (model == null)
                return false;

            model.InjectFrom<CustomInjection>(viewModel);

            _db.SaveChanges();

            return true;
        }

        public bool Remove(int id)
        {
            var model = _db.Set<CommentModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return false;

            model.IsRemoved = true;

            _db.SaveChanges();

            return true;
        }

        public CommentViewModel Get(int id)
        {
            var model = _db.Set<CommentModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return null;

            var viewModel = ConvertModel(model);

            return viewModel;
        }

        private CommentViewModel ConvertModel(CommentModel model)
        {
            var viewModel = new CommentViewModel();
            viewModel.InjectFrom<CustomInjection>(model);

            UserViewModel user = null;
            if(model.AuthorID != -1)
                user = _securityService.Get(model.AuthorID);

            if (user != null)
            {
                viewModel.AvatarSource = GetAvatarSourceByEMail(_securityService.GetEMailByID(viewModel.AuthorID));
                viewModel.AuthorSite = user.WebSite;
                viewModel.AuthorName = _securityService.GetUserNameByID(viewModel.AuthorID);
            }
            else
            {
                viewModel.AvatarSource = "http://www.gravatar.com/avatar/undefined";
                viewModel.AuthorSite = "undefined";
                viewModel.AuthorName = "Anonim";
            }

            return viewModel;
        }

        public List<CommentViewModel> GetAll()
        {
            var comments = _db.Set<CommentModel>().Where(p => !p.IsRemoved).ToList();
            var viewModels = new List<CommentViewModel>();

            for (int i = 0; i < comments.Count; i++)
            {
                viewModels.Add(ConvertModel(comments[i]));
            }

            return viewModels.OrderByDescending(p => p.PublishDate).ToList();
        }


        public List<CommentViewModel> GetByTargetID(int id, CommentTarget target)
        {
            var viewModels = new List<CommentViewModel>();
            var comments = _db.Set<CommentModel>().Where(p => p.ArticleID == id && p.Target == target).Where(p => !p.IsRemoved).ToList();

            for (int i = 0; i < comments.Count; i++)
            {
                viewModels.Add(ConvertModel(comments[i]));
            }

            return viewModels.OrderByDescending(p => p.PublishDate).ToList();
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


        public List<CommentViewModel> GetRecentComments(int count)
        {
            var recentComments = _db.Set<CommentModel>().
                Where(p => !p.IsRemoved).
                OrderByDescending(p => p.PublishDate).
                Take(count).
                ToList();

            var viewModel = new List<CommentViewModel>();

            for (int i = 0; i < recentComments.Count; i++)
            {
                viewModel.Add(ConvertModel(recentComments[i]));
            }

            return viewModel;
        }


        public List<CommentViewModel> GetByUserID(int id)
        {
            var model = _db.Set<CommentModel>().Where(p => p.AuthorID == id).Where(p => !p.IsRemoved).ToList();
            var viewModel = new List<CommentViewModel>();

            for(int i=0; i<model.Count; i++)
            {
                viewModel.Add(ConvertModel(model[i]));
            }

            return viewModel.OrderByDescending(p => p.PublishDate).ToList();
        }
    }
}