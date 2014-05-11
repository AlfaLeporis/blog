using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Blog.Services
{
    public class LogService : ILogService
    {
        public string GetContent()
        {
            var content = String.Empty;

            var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/log.txt"));
            content = reader.ReadToEnd();
            reader.Close();

            return content;
        }

        public void Clear()
        {
            var reader = new StreamWriter(HttpContext.Current.Server.MapPath("~/log.txt"), false);
            reader.Write("");
            reader.Close();
        }
    }
}