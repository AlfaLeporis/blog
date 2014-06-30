using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using Blog.ViewModels;

namespace Blog.Services
{
    public class SettingsService : ISettingsService
    {
        private static SettingsViewModel _cachedSettings = null;
        private static DateTime _lastSettingsChange = DateTime.MinValue;
        private const String _fileName = "~/config.cfg";

        public SettingsService()
        {
            _lastSettingsChange = File.GetLastWriteTime(HttpContext.Current.Server.MapPath(_fileName));
        }

        public void Save(ViewModels.SettingsViewModel viewModel)
        {
            using (var writer = new StreamWriter(HttpContext.Current.Server.MapPath(_fileName)))
            {
                var properties = viewModel.GetType().GetProperties();
                for(int i=0; i<properties.Count(); i++)
                {
                    String name = properties[i].Name;
                    String value = properties[i].GetValue(viewModel).ToString();

                    writer.WriteLine(name + "=" + value);
                }
            }
        }

        public ViewModels.SettingsViewModel GetSettings()
        {
            bool needReload = IsSettingsFileChanged();

            if (_cachedSettings == null || needReload)
            {
                var viewModel = new ViewModels.SettingsViewModel();
                var properties = viewModel.GetType().GetProperties();

                using (var reader = new StreamReader(HttpContext.Current.Server.MapPath(_fileName)))
                {
                    while (!reader.EndOfStream)
                    {
                        String line = reader.ReadLine();
                        var words = line.Split('=');

                        var property = properties.First(p => p.Name == words[0]);
                        var convertedValue = Convert.ChangeType(words[1], property.PropertyType);
                        property.SetValue(viewModel, convertedValue);
                    }
                }

                _cachedSettings = viewModel;
                return viewModel;
            }

            return _cachedSettings;
        }

        private bool IsSettingsFileChanged()
        {
            var time = File.GetLastWriteTime(HttpContext.Current.Server.MapPath(_fileName));
            if (time == _lastSettingsChange)
                return false;
            else
            {
                _lastSettingsChange = time;
                return true;
            }
        }
    }
}