using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Services
{
    public interface ILogService
    {
        String GetContent();
        void Clear();
    }
}