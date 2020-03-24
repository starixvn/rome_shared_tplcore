using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using System.IO;

namespace TplBuilder
{
    public static class Utils
    {
        public static string GetFileExt(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                int dotIndex = fileName.LastIndexOf('.');
                if (dotIndex != -1)
                {
                    string fileExt = fileName.Substring(dotIndex).ToLower();
                    return fileExt;
                }
            }
            return null;
        }

        public static bool IsCsExt(string fileName)
        {
            string fileExt = GetFileExt(fileName);
            if (fileExt == Config.CsExt)
            {
                return true;
            }
            return false;
        }

        public static bool IsHtmExt(string fileName)
        {
            string fileExt = GetFileExt(fileName);
            if (fileExt == Config.TplExt)
            {
                return true;
            }
            return false;
        }

        public static bool IsTplCsExt(string fileName)
        {
            string pattern = Config.TplCsExt;
            int length = pattern.Length;
            if (fileName.Length > length)
            {
                string substring = fileName.Substring(fileName.Length - length).ToLower();
                if (substring == pattern)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTplCsContent(string filePath)
        {
            bool result = false;
            using (StreamReader sr = new StreamReader(filePath))
            {
                char[] buffer = new char[Config.TplCoreCSTemplateHeader.Length];
                sr.Read(buffer, 0, Config.TplCoreCSTemplateHeader.Length);
                string s = new string(buffer);
                if (s == Config.TplCoreCSTemplateHeader)
                {
                    result = true;
                }
            }
            return result;
        }

        public static string GetHtmFileName(string tplCsFileName)
        {
            string result = "";
            if (tplCsFileName.Length > Config.CsExt.Length)
            {
                result = tplCsFileName.Substring(0, tplCsFileName.Length - Config.CsExt.Length);
            }
            return result;
        }

        // Check current project is a website or not
        public static bool IsWebsite(Project project)
        {
            if (project != null)
            {
                foreach (Property p in project.Properties)
                {
                    if (p.Name == "CurrentWebsiteLanguage")
                    {
                        return true;
                    }
                }
                //object[] extenderNames = (object[])project.ExtenderNames;
                //if (extenderNames.Length > 0)
                //{
                //    if (extenderNames[0].ToString() == "WebApplication")
                //    {
                //        return true;
                //    }
                //}
                //else
                //{
                //    foreach (Property p in project.Properties)
                //    {
                //        if (p.Name == "CurrentWebsiteLanguage")
                //        {
                //            return true;
                //        }
                //    }
                //}
            }
            return false;
        }

        /// <summary>
        /// Get directory path from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDirPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            int i = filePath.LastIndexOf('\\');
            if (i == -1)
                return null;

            return filePath.Substring(0, i + 1);
        }

        /// <summary>
        /// Get file name from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            int i = filePath.LastIndexOf('\\');
            if (i == -1)
                return null;

            return filePath.Substring(i + 1);
        }

        /// <summary>
        /// check folder is empty
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool IsEmptyFolder(string dirPath, string extension)
        {
            if (!Directory.Exists(dirPath))
                return false;

            string[] files = Directory.GetFiles(dirPath, "*" + extension);
            if (files.Length == 0) // Empty
                return true;

            return false;
        }

        public static CommandBar GetCommandBarPopup(CommandBar parentCommandBar, string commandBarPopupName)
        {
            CommandBar commandBar = null;
            CommandBarPopup commandBarPopup;

            foreach (CommandBarControl commandBarControl in parentCommandBar.Controls)
            {
                if (commandBarControl.Type == MsoControlType.msoControlPopup)
                {
                    commandBarPopup = (CommandBarPopup)commandBarControl;

                    if (commandBarPopup.CommandBar.Name == commandBarPopupName)
                    {
                        commandBar = commandBarPopup.CommandBar;
                        break;
                    }
                }
            }
            return commandBar;
        }

        public static bool IsFile(ProjectItem item)
        {
            if (item.Kind == VSProjectConstants.PhysicalFile)
            {
                return true;
            }

            return false;
        }

        public static bool IsFolder(ProjectItem item)
        {
            if (item.Kind == VSProjectConstants.PhysicalFolder)
            {
                return true;
            }

            return false;
        }
    }
}
