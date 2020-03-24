/*  Author: Lee, SMS
 *  @20091225
 *  All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using TplCore;

namespace TplCore.Compiler
{
    class UrlLocater
    {
        /// <summary>
        /// Check an url having ? or &
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsPlainUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return true;

            bool hasQuestion = (-1 == url.IndexOf(TplSettings.Question));
            bool hasAnd = (-1 == url.IndexOf(TplSettings.And));

            return hasQuestion && hasAnd;
        }

        /// <summary>
        /// Return ? or & depend on url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string NextMetaChar(string url)
        {
            string meta = "TplSettings.Question";
            if (!IsPlainUrl(url))
            {
                meta = "TplSettings.And";
            }

            return meta;
        }

        /// <summary>
        /// Count number of ../ in url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static int CountJumpOut(string url, out string fileName)
        {
            int result = 0;
            if (string.IsNullOrEmpty(url))
            {
                fileName = url;
                return result;
            }

            string match = TplSettings.UrlJumpStep;
            int step = match.Length;

            int index = url.IndexOf(match, 0);
            if (-1 == index)
            {
                fileName = url;
                return result;
            }

            int oldIndex = index;
            while (-1 != index)
            {
                result++;
                oldIndex = index;
                index = url.IndexOf(match, index + step);
            }

            fileName = url.Substring(oldIndex + step);

            return result;
        }

        /// <summary>
        /// Jump out numberOfJump in url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="numberOfJump"></param>
        /// <returns></returns>
        public static string JumpOut(string url, int numberOfJump)
        {
            if (string.IsNullOrEmpty(url) || (0 == numberOfJump)) return url;
            string[] folders = url.Split(new char[] { TplSettings.UrlSlash }, StringSplitOptions.RemoveEmptyEntries);
            int c = folders.Length;
            if (c == 0) return url;
            if (c <= numberOfJump) return string.Empty;

            c -= numberOfJump;

            string result = string.Empty;
            for (int i = 0; i < c; i++)
            {
                result += folders[i] + TplSettings.UrlSlash;
            }

            return result;
        }
    }
}
