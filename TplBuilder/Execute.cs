using System.IO;
using EnvDTE;
using TplCore.Compiler;

namespace TplBuilder
{
    public class Execute
    {
        private bool _isWebsite = false;
        private Project _currentProject = null;
        private string _projectPath;
        private string _outputPath;

        public string OutputPath
        {
            get { return _outputPath; }
        }

        public Project CurrentProject
        {
            get
            {
                return _currentProject;
            }
        }

        public bool IsWebsite
        {
            get { return _isWebsite; }
        }

        public string ProjectPath
        {
            get { return _projectPath; }
        }

        public Execute(Project project)
        {
            _currentProject = project;
            _isWebsite = Utils.IsWebsite(_currentProject);
            _projectPath = Utils.GetDirPath(_currentProject.FileName);
            if (_isWebsite)
            {
                _outputPath = Config.TplPath;
            }
            else
            {
                _outputPath = "";
            }
        }

        /// <summary>
        /// delete a folder in project
        /// </summary>
        /// <param name="dirPath"></param>
        public void DeleteProjectFolder(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath, "*" + Config.CsExt);
            if (files.Length == 0)
                return;

            string file = files[0];
            ProjectItem projectItem = VisualStudioRef.ApplicationObject.Solution.FindProjectItem(file);
            if (projectItem != null)
            {
                ProjectItem parentItem = (ProjectItem)(projectItem.Collection.Parent);
                if (parentItem != null)
                {
                    parentItem.Delete();
                }
                else
                {
                    projectItem.Delete();
                }
            }
        }

        /// <summary>
        /// delete a find in project
        /// </summary>
        /// <param name="filePath"></param>
        public void DeleteProjectFile(string filePath)
        {
            var projectItem = VisualStudioRef.ApplicationObject.Solution.FindProjectItem(filePath);
            if (projectItem != null)
            {
                projectItem.Delete();
            }
        }

        /// <summary>
        /// Build a specified template file.
        /// </summary>
        /// <param name="filePath">Full path of the template file.</param>
        public void BuildTemplate(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            string fileName = UppercaseFirstLetter(Utils.GetFileName(filePath));
            string dirPath = Utils.GetDirPath(filePath);
            string relativePath = dirPath.Remove(0, ProjectPath.Length);
            string dir = "";
            dir = ProjectPath + OutputPath + relativePath;

            string siteName = null;

            if (Config.DefaultNamespace != null && IsWebsite)
            {
                siteName = Config.DefaultNamespace;
            }
            else
            {
                siteName = CurrentProject.Properties.Item("DefaultNamespace").Value.ToString();
                if (IsWebsite)
                {
                    siteName = siteName.Remove(siteName.Length - 1, 1);
                }
                siteName = siteName.Substring(siteName.LastIndexOf('\\') + 1);
            }

            // Render
            string tplFilename = relativePath.Replace('\\', '.') + fileName.Replace(Config.TplExt, string.Empty).Replace('.', '_');
            var g = new Generator(tplFilename, siteName, IsWebsite);

            // Load data
            string data = File.ReadAllText(filePath);
            string content = g.RenderTemplate(data);

            if (string.IsNullOrEmpty(content))
                return;

            if (!Directory.Exists(dir)) // Add dir to current project
            {
                Directory.CreateDirectory(dir);
            }

            string outputFile = dir + fileName + Config.CsExt;
            if (!File.Exists(outputFile))
            {
                // Save to file
                File.WriteAllText(outputFile, content);
                CurrentProject.ProjectItems.AddFromFile(outputFile);
            }
            else
            {
                // Check out item
                try
                {
                    bool isCheckedOut = VisualStudioRef.ApplicationObject.SourceControl.IsItemCheckedOut(outputFile);
                    bool isUnderSCC = VisualStudioRef.ApplicationObject.SourceControl.IsItemUnderSCC(outputFile);
                    if (!isCheckedOut && isUnderSCC)
                    {
                        VisualStudioRef.ApplicationObject.SourceControl.CheckOutItem(outputFile);
                    }
                }
                catch
                {
                }

                // Save to file
                File.WriteAllText(outputFile, content);
            }
        }

        /// <summary>
        /// Build all template file in specified folder.
        /// </summary>
        /// <param name="folder">Full path of a folder.</param>
        public void BuildTemplateFolder(string folder)
        {
            if (!Directory.Exists(folder))
                return;

            string[] files = Directory.GetFiles(folder, "*" + Config.TplExt, SearchOption.TopDirectoryOnly);
            int c = files.Length;

            for (int i = 0; i < c; i++)
            {
                BuildTemplate(files[i]);
            }
        }

        public void ShowFileMapping()
        {
            var frm = new frmFileMapping(this);
            frm.Show();
        }

        public string GetCsFilePath(ProjectItem htmItem)
        {
            string htmFilePath = htmItem.get_FileNames(0);
            string htmFileName = htmItem.Name;
            string htmFileDir = Utils.GetDirPath(htmFilePath);
            string htmRelativeDir = htmFileDir.Replace(ProjectPath, "");
            string csFileDir = ProjectPath;
            if (!string.IsNullOrEmpty(OutputPath))
            {
                csFileDir = csFileDir + OutputPath;
            }
            csFileDir += htmRelativeDir;
            return csFileDir + htmFileName + Config.CsExt;
        }

        public string GetHtmFilePath(ProjectItem csItem)
        {
            string csFilePath = csItem.get_FileNames(0);
            string csFileName = csItem.Name;
            string csFileDir = Utils.GetDirPath(csFilePath);
            string htmFileDir = csFileDir;
            if (!string.IsNullOrEmpty(OutputPath))
            {
                htmFileDir = csFileDir.Replace(OutputPath, "");
            }
            return htmFileDir + Utils.GetHtmFileName(csFileName);
        }

        /// <summary>
        /// http://www.dotnetperls.com/uppercase-first-letter
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string UppercaseFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            char[] arrayLetter = text.ToCharArray();
            arrayLetter[0] = char.ToUpper(arrayLetter[0]);
            return new string(arrayLetter);
        }
    }
}