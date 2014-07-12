using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Services
{
    public interface ICaptchaService
    {
        bool ValidateCode(String response, String challange);
    }
}
