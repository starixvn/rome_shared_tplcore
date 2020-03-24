namespace TplCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Caching;
    using System.Web.SessionState;

    public class TplSettings
    {
        public const string CompanyName = "Fanex";
        public const string PresentationLayer = "UI";
        public const string TemplateSuffix = "_Template";
        public const string BlocSuffix = "_Block";

        public const string LanguageSuffix = "_en-US.";

        private static object locker = new object();

        private static Dictionary<string, string> mappingUrls = new Dictionary<string, string>();
        /// <summary>
        /// Key is extension, value is all matching files.
        /// </summary>
        private static Dictionary<string, List<string>> externalUrls = new Dictionary<string, List<string>>();

        #region HttpContext Members

        internal static Cache Cache
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }

        internal static HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        internal static HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        #endregion

        internal static string Protocol
        {
            get
            {
                return (Request.IsSecureConnection ? HttpSlashSlashSSL : HttpSlashSlash);
            }
        }

        #region Base Url Without SessionId in url

        internal static bool IsRequireSessionState
        {
            get
            {
                return HttpContext.Current.Handler is IRequiresSessionState;
            }
        }

        internal static bool UseCookieless = false;

        internal const string DOMAIN_LIST = "TPLCORE.DOMAIN_LIST";
        internal static List<string> Domains
        {
            get
            {
                return Session[DOMAIN_LIST] as List<string>;
            }
            set
            {
                Session[DOMAIN_LIST] = value;
            }
        }

        internal static int LENGTH = 3;

        /// <summary>
        /// Set a cookiesless domains if you want loading external resources with parralel mode.
        /// </summary>
        /// <param name="domains"></param>
        public static void SetCookielessDomains(string[] domains)
        {
            if (domains == null || domains.Length == 0)
            {
                UseCookieless = false;
                Domains = null;
                return;
            }


            UseCookieless = true;

            LENGTH = domains.Length;
            Domains = new List<string>(LENGTH);
            Domains.AddRange(domains);
        }

        /// <summary>
        ///  Hash a url to a number in number range.
        /// </summary>
        /// <param name="length">Number range.</param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static int GetHashNumber(int length, string url)
        {
            if (length <= 1) return 0;

            /* 
             * Get last "seed" characters, convert to int, sum and modulo to "length"           
             */

            int seed = 10;
            int sum = 0;
            int count = url.Length;
            if (seed > count) seed = count;
            for (int i = 1; i <= seed; i++)
            {
                sum += (int)(url[count - i]);
            }

            return sum % length;
        }

        /// <summary>
        /// Get a cookieless domain from a url (use hash function).
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static string GetCookielessDomain(string url)
        {
            if (Domains == null) return GetBaseUrl();
            return Domains[GetHashNumber(LENGTH, url)];
        }

        /// <summary>
        /// Base url for a website.
        /// Ex: http://www.abc.com:1080/site1/
        /// </summary>
        public static string GetBaseUrl()
        {
            string hostName = Request.Headers["Host"];
            string appPath = Request.ApplicationPath;

            string url = /*Protocol + hostName +*/ appPath;

            if (appPath.Length > 1)
            {
                url += UrlSlash;
            }

            return url;
        }

        #endregion

        #region Base Url With SessionId in url

        internal static string GetBaseUrl2()
        {
            string hostName = Request.Headers["Host"];
            string appPath = HttpContext.Current.Response.ApplyAppPathModifier(Request.ApplicationPath);
            if (appPath[appPath.Length - 1] != UrlSlash)
            {
                appPath += UrlSlash;
            }

            return /*Protocol + hostName +*/ appPath;
        }

        #endregion

        internal const string CURRENT_THEME = "TPLCORE.CURRENT_THEME";
        /// <summary>
        /// Get or set current theme for current session. Ex. Link to style.css will be suffix as style.[CurrentTheme].css
        /// </summary>
        public static string CurrentTheme
        {
            get
            {
                return Session[CURRENT_THEME] as string;
            }
            set
            {
                Session[CURRENT_THEME] = value;
            }
        }

        // Fields
        public const char And = '&';
        internal const char Comma = ',';
        internal const char Dot = '.';
        internal const char DoubleQuote = '"';
        internal const string HttpSlashSlash = "http://";
        internal const string HttpSlashSlashSSL = "https://";
        public const char Question = '?';
        internal const string Seperator = "_";
        internal const char Space = ' ';
        internal const char TwoDot = ':';
        internal const string UrlJumpStep = "../";
        internal const char UrlSlash = '/';

        internal const string DOTJS = ".js";
        internal const string DOTCSS = ".css";

        public static string UrlVersion = string.Empty;

        /// <summary>
        /// Find and correct relative url
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        internal static string FindUrl(string relativeUrl)
        {
            // Correct relative Url and save to memory cache
            if (!mappingUrls.ContainsKey(relativeUrl))
            {
                lock (locker)
                {
                    string matching = '*' + Path.GetExtension(relativeUrl);
                    if (!externalUrls.ContainsKey(matching))
                    {
                        // Find correct url
                        string[] files = Directory.GetFiles(HttpContext.Current.Request.PhysicalApplicationPath, matching, SearchOption.AllDirectories);
                        List<string> urls = new List<string>();
                        foreach (string file in files)
                        {
                            string url = file.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "").Replace("\\", "/");
                            urls.Add(url);
                            if (url.EndsWith(relativeUrl, StringComparison.OrdinalIgnoreCase))
                            {
                                if (mappingUrls.ContainsKey(relativeUrl)) continue;
                                mappingUrls.Add(relativeUrl, url);
                            }
                        }
                        externalUrls.Add(matching, urls);
                    }
                    else
                    {
                        List<string> urls = externalUrls[matching];
                        foreach (string url in urls)
                        {
                            if (url.EndsWith(relativeUrl, StringComparison.OrdinalIgnoreCase))
                            {
                                if (mappingUrls.ContainsKey(relativeUrl)) continue;
                                mappingUrls.Add(relativeUrl, url);
                            }
                        }
                    }
                }
            }

            relativeUrl = mappingUrls[relativeUrl];
            return relativeUrl;        
        }

        /// <summary>
        /// Return absolute url from relative url
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string AbsoluteUrl(string relativeUrl)
        {
            return GetBaseUrl() + FindUrl(relativeUrl) + Question + UrlVersion;
        }

        /// <summary>
        /// Return absolute url from relative url, including cookieless
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string AbsoluteUrl2(string relativeUrl)
        {
            return GetBaseUrl2() + FindUrl(relativeUrl) + Question + UrlVersion;
        }
    }
}
