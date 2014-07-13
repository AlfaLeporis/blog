using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Blog.Services
{
    public class CaptchaService : ICaptchaService
    {
        private ISettingsService _settingsService = null;

        public CaptchaService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public bool ValidateCode(string response, string challange)
        {
            var privateKey = _settingsService.GetSettings().CaptchaPrivateKey;
            var userIP = HttpContext.Current.Request.Params["REMOTE_ADDR"];

            var webClient = new WebClient();
            var isValid = webClient.UploadValues("http://www.google.com/recaptcha/api/verify", "POST",
                new System.Collections.Specialized.NameValueCollection()
                {
                   { "privatekey", privateKey },
                   { "remoteip", userIP },
                   { "challenge", challange },
                   { "response", response }
                });
            var convertedResponse = System.Text.ASCIIEncoding.ASCII.GetString(isValid);

            if (convertedResponse.StartsWith("true"))
                return true;
            else
                return false;
        }
    }
}