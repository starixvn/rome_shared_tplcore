using System.Threading;

namespace TplCore.Element
{
    public abstract class BaseElement : IElement
    {
        public bool Visible = true;

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        /// <summary>
        /// Absolute url without version.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        protected static string AbsoluteUrl(string relativeUrl)
        {
            return AbsoluteUrl(relativeUrl, true);
        }

        /// <summary>
        /// Absolute url without version.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="autoLanguage"></param>
        /// <returns></returns>
        protected static string AbsoluteUrl(string relativeUrl, bool autoLanguage)
        {
            relativeUrl = FindUrl(relativeUrl);

            relativeUrl = AddThemeName(relativeUrl);

            if (autoLanguage)
            {
                string currentLanguageSuffix = "_" + Thread.CurrentThread.CurrentUICulture.Name + ".";
                relativeUrl = relativeUrl.Replace(TplSettings.LanguageSuffix, currentLanguageSuffix);
            }

            if (relativeUrl.Contains(TplSettings.DOTJS) || !TplSettings.UseCookieless)
            {
                return TplSettings.GetBaseUrl() + relativeUrl;
            }

            return TplSettings.GetCookielessDomain(relativeUrl) + relativeUrl;
        }

        /// <summary>
        /// Absolute url with version.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="nextMetaChar"></param>
        /// <returns></returns>
        protected static string AbsoluteUrl(string relativeUrl, char nextMetaChar)
        {
            relativeUrl = FindUrl(relativeUrl);

            string version = string.IsNullOrEmpty(TplSettings.UrlVersion) ? string.Empty : (nextMetaChar + TplSettings.UrlVersion);
            return AbsoluteUrl(relativeUrl) + version;
        }

        /// <summary>
        /// Find absolute url from relative url
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private static string FindUrl(string relativeUrl)
        {
            return TplSettings.FindUrl(relativeUrl);
        }

        /// <summary>
        /// Absolute cookieless url without version.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        protected static string AbsoluteUrl2(string relativeUrl)
        {
            relativeUrl = FindUrl(relativeUrl);

            return TplSettings.GetBaseUrl2() + relativeUrl;
        }

        /// <summary>
        /// Absolute cookieless url with version.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="nextMetaChar"></param>
        /// <returns></returns>
        protected static string AbsoluteUrl2(string relativeUrl, char nextMetaChar)
        {
            relativeUrl = FindUrl(relativeUrl);

            string version = string.IsNullOrEmpty(TplSettings.UrlVersion) ? string.Empty : (nextMetaChar + TplSettings.UrlVersion);
            return TplSettings.GetBaseUrl2() + relativeUrl + version;
        }

        /// <summary>
        /// Add current theme name as suffix of css url. Ex: style.css-->style.[ThemeName].css
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private static string AddThemeName(string relativeUrl)
        {
            return relativeUrl.Replace(TplSettings.DOTCSS,
                (string.IsNullOrEmpty(TplSettings.CurrentTheme) ? null : (TplSettings.Dot + TplSettings.CurrentTheme))
                + TplSettings.DOTCSS);
        }

        #region IElement Members

        public virtual bool RegisterComponent(string name, IComponent component)
        {
            return false;
        }

        public virtual bool RegisterComponent(IComponent component)
        {
            return RegisterComponent(null, component);
        }

        #endregion
    }
}
