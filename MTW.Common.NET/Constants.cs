using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTW.Common
{
    public static class Constants
    {
        public const string UserAgentString = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36 Edg/139.0.0.0"; //"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

        public static readonly Regex UrlRegex = new Regex("<a [^<>]*href\\s*=\\s*\"(?<1>https://[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
