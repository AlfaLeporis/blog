using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;

namespace Blog.Services
{
    public class SettingsService : ISettingsService
    {
        public void Save(ViewModels.SettingsViewModel viewModel)
        {
            using(var writer = new StreamWriter(HttpContext.Current.Server.MapPath("~/config.cfg")))
            {
                var properties = viewModel.GetType().GetProperties();
                for(int i=0; i<properties.Count(); i++)
                {
                    String name = properties[i].Name;
                    String value = properties[i].GetValue(viewModel) as String;

                    writer.WriteLine(name + "=" + value);
                }
            }
        }

        public ViewModels.SettingsViewModel GetSettings()
        {
            var viewModel = new ViewModels.SettingsViewModel();
            var properties = viewModel.GetType().GetProperties();

            using(var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/config.cfg")))
            {
                while(!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    var words = line.Split('=');

                    properties.First(p => p.Name == words[0]).SetValue(viewModel, words[1]);
                }
            }

            return viewModel;
        }
    }
}