using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TplCore.Compiler
{
    /// <summary>
    /// Filter language, css, js, img, swf, action (form) and hidden block.
    /// </summary>
    class Filtering
    {
        #region Declaration

        private static string OpenBlockNamePrefix = "<!--";
        private static string BlockNameSuffix = "-->";
        private static string CloseBlockNamePrefix = "<!--/";

        private static string TagPrefix = "{";
        private static string TagSuffix = "}";
        private static string StaticStringPrefix = "static:";

        // Auto-located form action, external css, img and javascript
        private static string LocatingFunction = "AbsoluteUrl";
        private static string LocatingFunction2 = "AbsoluteUrl2";
        private static string OpenFunctionString = "(";
        private static string CloseFunctionString = ")";

        private static string CssPrefix = "<link href=\"";
        private static string CssSuffix = "\"";

        private static string JsPrefix = "<script src=\"";
        private static string JsSuffix = "\"";

        // Ex:  <img src="img.ashx"...
        private static string ImgPrefix = " src=\"";
        private static string ImgSuffix = "\"";

        private static string ActionPrefix = " action=\"";
        private static string ActionSuffix = "\"";

        private static string ImgHandler = ".ashx";

        private static char DoubleQuote = '"';

        #endregion

        private static string TemplateUrl;


        public static void FilterTemplate(ref string text, string templateUrl)
        {
            TemplateUrl = templateUrl;

            RemoveHiddenBlock(ref text);
            PredicateRelativeUrl(ref text);
        }


        #region Filter Hidden Block

        private static string HiddenBlock = "HiddenBlock";
        private static void RemoveHiddenBlock(ref string text)
        {
            string name = HiddenBlock;

            int i = text.IndexOf(OpenBlockNamePrefix + name + BlockNameSuffix, 0);
            if (-1 == i)
            {
                return;
            }

            // Close Block
            int j = text.IndexOf(CloseBlockNamePrefix + name + BlockNameSuffix, i);
            if (-1 == j)
            {
                return;
            }


            text = text.Remove(i, j - i + (CloseBlockNamePrefix + name + BlockNameSuffix).Length);

            // Recursive all hidden block
            RemoveHiddenBlock(ref text);
        }

        #endregion

        #region Filter Url


        private static void PredicateRelativeUrl(ref string text)
        {
            PredicateCssUrl(ref text, 0);
            PredicateJsUrl(ref text, 0);
            PredicateImgSrc(ref text, 0);
            PredicateFrmAction(ref text, 0);
        }

        /// <summary>
        /// Predicate all relative css urls.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static void PredicateCssUrl(ref string text, int index)
        {
            if (String.IsNullOrEmpty(text)) return;

            int i1 = text.IndexOf(CssPrefix, index);
            if (-1 == i1) return;

            int i2 = text.IndexOf(CssSuffix, i1 + CssPrefix.Length);
            if (-1 == i2) return;

            int start = i1 + CssPrefix.Length;
            int length = i2 - start;

            string cssUrl = text.Substring(start, length);

            if (IsRelativeUrl(cssUrl))
            {
                // Replace with predicated static tag
                // ../../abc.css --> {static:AbsoluteUrl(relativeUrl, nextChar)}

                string cssFileName;
                int numberOfJump = UrlLocater.CountJumpOut(cssUrl, out cssFileName);

                string relativeCssUrl = UrlLocater.JumpOut(TemplateUrl, numberOfJump) + cssFileName;

                string replacingString = TagPrefix + StaticStringPrefix
                    + LocatingFunction
                    + OpenFunctionString
                    + DoubleQuote + relativeCssUrl + DoubleQuote
                    + TplSettings.Comma
                    + TplSettings.Space
                    + UrlLocater.NextMetaChar(relativeCssUrl)
                    + CloseFunctionString
                    + TagSuffix;

                text = text.Replace(CssPrefix + cssUrl + CssSuffix, CssPrefix + replacingString + CssSuffix);
                PredicateCssUrl(ref text, i1 + 1);
            }
            else
            {
                PredicateCssUrl(ref text, i2 + 1);
            }
        }

        /// <summary>
        /// Predicate all relative js urls.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static void PredicateJsUrl(ref string text, int index)
        {
            if (String.IsNullOrEmpty(text)) return;

            int i1 = text.IndexOf(JsPrefix, index);
            if (-1 == i1) return;

            int i2 = text.IndexOf(JsSuffix, i1 + JsPrefix.Length);
            if (-1 == i2) return;

            int start = i1 + JsPrefix.Length;
            int length = i2 - start;

            string jsUrl = text.Substring(start, length);

            if (IsRelativeUrl(jsUrl))
            {
                // Replace with predicated static tag
                // ../../abc.js --> {static:AbsoluteUrl(relativeUrl, nextChar)}

                string jsFileName;
                int numberOfJump = UrlLocater.CountJumpOut(jsUrl, out jsFileName);

                string relativeJsUrl = UrlLocater.JumpOut(TemplateUrl, numberOfJump) + jsFileName;
                string replacingString = TagPrefix + StaticStringPrefix
                    + LocatingFunction
                    + OpenFunctionString
                    + DoubleQuote + relativeJsUrl + DoubleQuote
                    + TplSettings.Comma
                    + TplSettings.Space
                    + UrlLocater.NextMetaChar(relativeJsUrl)
                    + CloseFunctionString
                    + TagSuffix;

                text = text.Replace(JsPrefix + jsUrl + JsSuffix, JsPrefix + replacingString + JsSuffix);
                PredicateJsUrl(ref text, i1 + 1);
            }
            else
            {
                PredicateJsUrl(ref text, i2 + 1);
            }
        }

        /// <summary>
        /// Predicate all relative img src.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static void PredicateImgSrc(ref string text, int index)
        {
            if (String.IsNullOrEmpty(text)) return;

            int i1 = text.IndexOf(ImgPrefix, index);
            if (-1 == i1) return;

            int i2 = text.IndexOf(ImgSuffix, i1 + ImgPrefix.Length);
            if (-1 == i2) return;

            int start = i1 + ImgPrefix.Length;
            int length = i2 - start;

            string imgSrc = text.Substring(start, length);

            if (IsRelativeUrl(imgSrc))
            {
                // Replace with predicated static tag
                // ../../abc.ashx --> {static:AbsoluteUrl2(relativeUrl)}

                // Replace with predicated static tag
                // ../../abc.jpg --> {static:AbsoluteUrl(relativeUrl)}

                string imgFileName;
                int numberOfJump = UrlLocater.CountJumpOut(imgSrc, out imgFileName);

                string locatingFunction = (imgSrc.IndexOf(ImgHandler) == -1) ? LocatingFunction : LocatingFunction2;

                string relativeImgSrc = UrlLocater.JumpOut(TemplateUrl, numberOfJump) + imgFileName;
                string replacingString = TagPrefix + StaticStringPrefix
                    + locatingFunction
                    + OpenFunctionString
                    + DoubleQuote + relativeImgSrc + DoubleQuote
                    + CloseFunctionString
                    + TagSuffix;

                text = text.Replace(ImgPrefix + imgSrc + ImgSuffix, ImgPrefix + replacingString + ImgSuffix);
                PredicateImgSrc(ref text, i1 + 1);
            }
            else
            {
                PredicateImgSrc(ref text, i2 + 1);
            }
        }

        /// <summary>
        /// Predicate all relative form action.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static void PredicateFrmAction(ref string text, int index)
        {
            if (String.IsNullOrEmpty(text)) return;

            int i1 = text.IndexOf(ActionPrefix, index);
            if (-1 == i1) return;

            int i2 = text.IndexOf(ActionSuffix, i1 + ActionPrefix.Length);
            if (-1 == i2) return;

            int start = i1 + ActionPrefix.Length;
            int length = i2 - start;

            string action = text.Substring(start, length);

            if (IsRelativeUrl(action))
            {
                // Replace with predicated static tag
                // ../../abc.aspx --> {static:AbsoluteUrl2(relativeUrl)}

                string actionFileName;
                int numberOfJump = UrlLocater.CountJumpOut(action, out actionFileName);

                string relativeAction = UrlLocater.JumpOut(TemplateUrl, numberOfJump) + actionFileName;
                string replacingString = TagPrefix + StaticStringPrefix
                    + LocatingFunction2
                    + OpenFunctionString
                    + DoubleQuote + relativeAction + DoubleQuote
                    + CloseFunctionString
                    + TagSuffix;

                text = text.Replace(ActionPrefix + action + ActionSuffix, ActionPrefix + replacingString + ActionSuffix);
                PredicateFrmAction(ref text, i1 + 1);
            }
            else
            {
                PredicateFrmAction(ref text, i2 + 1);
            }
        }

        private static bool IsRelativeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            if (url.Contains(TagPrefix + StaticStringPrefix)) return false;

            // Not absolulte url
            return !url.StartsWith(TplSettings.HttpSlashSlash);
        }

        #endregion
    }
}
