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
    }
}