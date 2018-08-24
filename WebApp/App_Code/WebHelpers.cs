using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace WebApi
{
    public static class WebHelpers
    {
        public const string EMAIL_MESSAGE_HEADER_KEY = "{{MESSAGE_HEADING}}";
        public const string EMAIL_MESSAGE_BODY_KEY = "{{MESSAGE_BODY}}";

        public static string GenerateEmailTemplate(string content, string heading)
        {
            var path = HostingEnvironment.MapPath("~/EmailTemplates/default-template.html");
            var emailTemplate = File.ReadAllText(path, Encoding.UTF8);

            emailTemplate = emailTemplate.Replace(EMAIL_MESSAGE_HEADER_KEY, heading);
            emailTemplate = emailTemplate.Replace(EMAIL_MESSAGE_BODY_KEY, content);

            return emailTemplate;
        }

        public static string GetRootIndexPath()
        {
            var rootIndexPath = ConfigurationManager.AppSettings["RootIndexPath"];

            if (!string.IsNullOrEmpty(rootIndexPath))
                return rootIndexPath;

            return "wwwroot/index.html";
        }

        public static string SanitizeFileName(string filename)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(filename, invalidRegStr, "_");
        }

    }
}