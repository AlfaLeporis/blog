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

namespace Blog.Services
{
    public class SecurityService : ISecurityService
    {
        public bool Login(AuthViewModel viewModel)
        {
            if (viewModel.Login == String.Empty)
                throw new ArgumentNullException("Login");

            if (viewModel.Password == String.Empty)
                throw new ArgumentNullException("Password");

            return WebSecurity.Login(viewModel.Login, viewModel.Password);
        }

        public void LogOut()
        {
            WebSecurity.Logout();
        }

        public bool Register(RegisterViewModel viewModel, SettingsViewModel settings)
        {
            using(var db = new DatabaseContext())
            {
                if(db.Users.Any(p => p.Name == viewModel.UserName || p.EMail == viewModel.EMail))
                    return false;

                var additionalProperties = new Dictionary<String, object>();
                additionalProperties.Add("EMail", viewModel.EMail);
                
                var token = WebSecurity.CreateUserAndAccount(viewModel.UserName, viewModel.Password, additionalProperties, true);

                var messageContentReader = new StreamReader(HttpContext.Current.Server.MapPath("~/ActivationMailTemplate.txt"));
                var content = messageContentReader.ReadToEnd();
                content = content.Replace("{1}", viewModel.UserName);
                content = content.Replace("{2}", viewModel.EMail);
                content = content.Replace("{3}", token);

                var msg = new MailMessage(settings.SMTPUserAdress, viewModel.EMail, "Aktywacja konta", content);
                var smtp = new SmtpClient(settings.SMTPAdress, Convert.ToInt32(settings.SMTPPort));
                smtp.Credentials = new NetworkCredential(settings.SMTPUserName, settings.SMTPPassword);
                smtp.Send(msg);
            }

            return true;
        }

        public bool ChangePassword(EditPasswordViewModel viewModel)
        {
            var currentUserName = WebSecurity.CurrentUserName;
            bool result = WebSecurity.ChangePassword(currentUserName, viewModel.OldPassword, viewModel.NewPassword);

            return result;
        }
    }
}