using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using TplBuilder;
using Task = System.Threading.Tasks.Task;

namespace Fanex.TplBuilder
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class TplCoreCommand
    {
        private static DocumentEvents _documentEvents;
        private static ProjectItemsEvents _websiteItemsEvents;
        private static ProjectItemsEvents _projectItemsEvents;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("92ea285f-be2a-4672-9002-33a81ec5e376");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="TplCoreCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private TplCoreCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TplCoreCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in TplCoreCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new TplCoreCommand(package, commandService);
            VisualStudioRef.ApplicationObject = (DTE2)(await package.GetServiceAsync(typeof(DTE)));
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            UnregisterEvents();
            RegisterEvents();

            string message = string.Format(CultureInfo.CurrentCulture, "Extension loaded successfully", this.GetType().FullName);
            string title = "Fanex.TplBuilder";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private void UnregisterEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _documentEvents = VisualStudioRef.ApplicationObject.Events.DocumentEvents;
            _documentEvents.DocumentSaved -= _documentEvents_DocumentSaved;
            if (_websiteItemsEvents != null)
            {
                _websiteItemsEvents.ItemRemoved -= _projectItemsEvents_ItemRemoved;
                _websiteItemsEvents.ItemRenamed -= _projectItemsEvents_ItemRenamed;
            }

            if (_projectItemsEvents != null)
            {
                _projectItemsEvents.ItemRemoved += _projectItemsEvents_ItemRemoved;
                _projectItemsEvents.ItemRenamed += _projectItemsEvents_ItemRenamed;
            }
        }

        private void RegisterEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _documentEvents = VisualStudioRef.ApplicationObject.Events.DocumentEvents;
            _documentEvents.DocumentSaved += _documentEvents_DocumentSaved;
            InfoLog.Log("DocumentSaved event is registered succesfully");

            // register for website project
            _websiteItemsEvents = VisualStudioRef.ApplicationObject.Events.GetObject("WebSiteItemsEvents") as ProjectItemsEvents;
            if (_websiteItemsEvents != null)
            {
                _websiteItemsEvents.ItemRemoved += _projectItemsEvents_ItemRemoved;
                _websiteItemsEvents.ItemRenamed += _projectItemsEvents_ItemRenamed;
                InfoLog.Log("WebSiteItemsEvents' events are registered succesfully");
            }

            // register for other projects
            Events2 events2 = VisualStudioRef.ApplicationObject.Events as Events2;
            if (events2 != null)
            {
                _projectItemsEvents = events2.ProjectItemsEvents;
                if (_projectItemsEvents != null)
                {
                    _projectItemsEvents.ItemRemoved += _projectItemsEvents_ItemRemoved;
                    _projectItemsEvents.ItemRenamed += _projectItemsEvents_ItemRenamed;
                    InfoLog.Log("ProjectItemsEvents' events are registered succesfully");
                }
            }
        }

        private void _projectItemsEvents_ItemRenamed(ProjectItem ProjectItem, string OldName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                InfoLog.Log("ItemRenamed is fired");

                // item can a file or a folder
                string itemName = ProjectItem.Name;
                if (itemName == OldName)
                    return;

                var execute = new Execute(ProjectItem.ContainingProject);
                string itemPath = ProjectItem.get_FileNames(0);

                if (itemPath[itemPath.Length - 1] == '\\') // Rename a folder
                {
                    string relativePath = itemPath.Remove(0, execute.ProjectPath.Length);
                    relativePath = relativePath.Remove(relativePath.Length - 1);
                    relativePath = relativePath.Substring(0, relativePath.LastIndexOf('\\') + 1);
                    string tplDir = execute.ProjectPath + execute.OutputPath + relativePath + OldName + '\\';
                    execute.DeleteProjectFolder(tplDir);
                    execute.BuildTemplateFolder(itemPath);
                }
                else
                {
                    if (!OldName.EndsWith(Config.TplExt) || !itemPath.EndsWith(Config.TplExt))
                        return;

                    // Rename a file
                    string dirPath = Utils.GetDirPath(itemPath);
                    string relativePath = dirPath.Remove(0, execute.ProjectPath.Length);
                    string tplDir = execute.ProjectPath + execute.OutputPath + relativePath;

                    // Delete old template class
                    string oldFilePath = tplDir + OldName + Config.CsExt;
                    execute.DeleteProjectFile(oldFilePath);

                    // Build new template class
                    execute.BuildTemplate(itemPath);
                }
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }

        private void _projectItemsEvents_ItemRemoved(ProjectItem ProjectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                InfoLog.Log("ItemRemoved is fired");

                var execute = new Execute(ProjectItem.ContainingProject);
                string filePath = ProjectItem.get_FileNames(0);

                string relativePath;
                string tplDir;
                if (filePath[filePath.Length - 1] == '\\') // Del a folder
                {
                    relativePath = filePath.Remove(0, execute.ProjectPath.Length);
                    if (relativePath.StartsWith(execute.OutputPath))
                        return; // Del child folder in App_Code\\TplCore
                    tplDir = execute.ProjectPath + execute.OutputPath + relativePath;
                    execute.DeleteProjectFolder(tplDir);
                    return;
                }

                if (!filePath.EndsWith(Config.TplExt))
                    return;

                // Del old template class
                string fileName = Utils.GetFileName(filePath);
                string dirPath = Utils.GetDirPath(filePath);

                relativePath = dirPath.Remove(0, execute.ProjectPath.Length);
                tplDir = execute.ProjectPath + execute.OutputPath + relativePath;

                // Check folder is empty
                if (Utils.IsEmptyFolder(dirPath, Config.CsExt))
                {
                    execute.DeleteProjectFolder(tplDir);
                    return;
                }

                // Delete old template class
                string oldFileName = tplDir + fileName + Config.CsExt;
                execute.DeleteProjectFile(oldFileName);
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }

        private void _documentEvents_DocumentSaved(Document Document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                InfoLog.Log("DocumentSaved is fired");

                string filePath = Document.FullName;
                if (!filePath.EndsWith(Config.TplExt))
                    return;

                var execute = new Execute(Document.ProjectItem.ContainingProject);
                execute.BuildTemplate(filePath);
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }
    }
}
