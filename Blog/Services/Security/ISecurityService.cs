using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface ISecurityService
    {
        bool Login(AuthViewModel viewModel);
        void LogOut();
        bool Register(RegisterViewModel viewModel, SettingsViewModel settings);
        bool ChangePassword(EditPasswordViewModel viewModel);
        int GetCurrentID();
        String GetEMailByID(int id);
        String GetUserNameByID(int id);
        List<UserViewModel> GetAll();
        UserViewModel Get(int id);
        bool Edit(UserViewModel viewModel);
        bool Remove(int id);
        bool IsLogged();
    }
}