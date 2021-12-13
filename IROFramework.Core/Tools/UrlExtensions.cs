using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using IROFramework.Core.AppEnvironment;

namespace IROFramework.Core.Tools
{
    public static class UrlExtensions
    {
        public static string UrlEncode(this string fileName)
        {
            var spaceKey = "1122SPACE2211";
            var slashKey = "1122SLASH2211";
            var backSlashKey = "1122BACKSLASH2211";
            var urlFileName = fileName
                .Replace(" ", spaceKey)
                .Replace("/", slashKey)
                .Replace("\\", backSlashKey);

            urlFileName = HttpUtility.UrlEncode(urlFileName);

            urlFileName = urlFileName
                .Replace(spaceKey, "%20")
                .Replace(slashKey, "/")
                .Replace(backSlashKey, "\\");
            return urlFileName;
        }

        public static string RemoveEndingSlash(this string url)
        {
            if (url.Last() == '/')
            {
                return url.Remove(url.Length - 1);
            }
            return url;
        }

        public static string GetOwnApiUrl()
        {
            return $"{Env.GlobalSettings.ExternalUrl.RemoveEndingSlash()}/{CommonConsts.ApiPath}";
        }

        public static string GetOwnUrl()
        {
            return Env.GlobalSettings.ExternalUrl.RemoveEndingSlash();
        }
    }
}
