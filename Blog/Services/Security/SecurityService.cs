using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.ViewModels;
using WebMatrix.WebData;
using Blog.DAL;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Data.Entity;
using Blog.Models;
using Omu.ValueInjecter;
using Blog.App_Start;
using System.Web.Security;

namespace Blog.Services
{
    public class SecurityService : ISecurityService
    {
        private DbContext _db = null;
        //private ICommentsService _commentsService = null;

        public SecurityService(DbContext db)
        {
            _db = db;
            //_commentsService = commentsService;
        }

        public bool Login(AuthViewModel viewModel)
        {
            if (viewModel.Login == String.Empty || viewModel.Password == String.Empty)
                return false;

            var user = _db.Set<UserModel>().FirstOrDefault(p => p.Name == viewModel.Login);
            if (user == null)
                return false;

            user.LastVisit = DateTime.Now;
            _db.SaveChanges();

            return WebSecurity.Login(viewModel.Login, viewModel.Password);
        }

        public void LogOut()
        {
            WebSecurity.Logout();
        }

        public bool Register(RegisterViewModel viewModel, SettingsViewModel settings)
        {
            if(_db.Set<UserModel>().Any(p => p.Name == viewModel.UserName || p.EMail == viewModel.EMail))
                return false;

            var additionalProperties = new Dictionary<String, object>();
            additionalProperties.Add("EMail", viewModel.EMail);
            additionalProperties.Add("LastVisit", new DateTime(1970, 1, 1, 0, 0, 0));
                
            var token = WebSecurity.CreateUserAndAccount(viewModel.UserName, viewModel.Password, additionalProperties, false);

            //var messageContentReader = new StreamReader(HttpContext.Current.Server.MapPath("~/ActivationMailTemplate.txt"));
            //var content = messageContentReader.ReadToEnd();
            //content = content.Replace("{1}", viewModel.UserName);
            //content = content.Replace("{2}", viewModel.EMail);
            //content = content.Replace("{3}", token);

            //var msg = new MailMessage(settings.SMTPUserAdress, viewModel.EMail, "Aktywacja konta", content);
            //var smtp = new SmtpClient(settings.SMTPAdress, Convert.ToInt32(settings.SMTPPort));
            //smtp.Credentials = new NetworkCredential(settings.SMTPUserName, settings.SMTPPassword);
            //smtp.Send(msg);

            return true;
        }

        public bool ChangePassword(EditPasswordViewModel viewModel)
        {
            var currentUserName = WebSecurity.CurrentUserName;
            bool result = WebSecurity.ChangePassword(currentUserName, viewModel.OldPassword, viewModel.NewPassword);

            return result;
        }

        public int GetCurrentID()
        {
            return WebSecurity.CurrentUserId;
        }


        public string GetEMailByID(int id)
        {
            if (id == -1)
                return "undefined";

            var user = _db.Set<UserModel>().FirstOrDefault(p => p.ID == id);

            if (user == null)
                return "undefined";

            return user.EMail;
        }


        public string GetUserNameByID(int id)
        {
            if (id == -1)
                return "Anonim";

            var user = _db.Set<UserModel>().FirstOrDefault(p => p.ID == id);

            if (user == null)
                return "Anonim";

            return user.Name;
        }

        private UserViewModel ConvertModel(UserModel model)
        {
            var viewModel = new UserViewModel();
            viewModel.InjectFrom<CustomInjection>(model);
            return viewModel;
        }

        public List<UserViewModel> GetAll()
        {
            var users = _db.Set<UserModel>().ToList();
            var viewModel = new List<UserViewModel>();

            for(int i=0; i<users.Count; i++)
            {
                viewModel.Add(ConvertModel(users[i]));
            }

            return viewModel;
        }


        public UserViewModel Get(int id)
        {
            var model = _db.Set<UserModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                throw new Exception("Użytkownik o podanym id (" + id + ") nie istnieje.");

            var viewModel = ConvertModel(model);
            //viewModel.CommentsCount = _commentsService.GetByUserID(id).Count();
            return viewModel;
        }


        public bool Edit(UserViewModel viewModel)
        {
            int id = viewModel.ID.HasValue ? viewModel.ID.Value : WebSecurity.CurrentUserId;
            var model = _db.Set<UserModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return false;

            model.WebSite = viewModel.WebSite;
            _db.SaveChanges();

            return true;
        }

        public bool Remove(int id)
        {
            var user = _db.Set<UserModel>().FirstOrDefault(p => p.ID == id);
            if (user == null)
                return false;

            bool result = System.Web.Security.Membership.DeleteUser(user.Name);
            return result;
        }
    }
}