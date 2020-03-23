/*  Author: Lee, SMS
 *  @20091127
 *  All rights reserved.
 */

using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Windows.Forms;

namespace TplBuilder
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private static DocumentEvents _documentEvents;
        private static ProjectItemsEvents _websiteItemsEvents;
        private static ProjectItemsEvents _projectItemsEvents;

        private static CommandBarPopup _websiteCommandBar;
        private static CommandBarPopup _projectCommandBar;

        /// <summary>
        /// 
        /// </summary>
        public static AddIn AddInInstance { get; private set; }

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            try
            {
                VisualStudioRef.ApplicationObject = (DTE2)application;
                AddInInstance = (AddIn)addInInst;

                InfoLog.Log("OnConnection is called");

                switch (connectMode)
                {
                    case ext_ConnectMode.ext_cm_UISetup:
                        // Do nothing for this add-in with temporary user interface
                        break;

                    case ext_ConnectMode.ext_cm_Startup:
                        // The add-in was marked to load on startup
                        // Do nothing at this point because the IDE may not be fully initialized
                        // Visual Studio will call OnStartupComplete when fully initialized
                        break;

                    case ext_ConnectMode.ext_cm_AfterStartup:
                        // The add-in was loaded by hand after startup using the Add-In Manager
                        // Initialize it in the same way that when is loaded on startup
                        InitializeAddIn();
                        break;
                }
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }

        private void InitializeAddIn()
        {
            AddTemporaryUI();
            RegisterEvents();
        }

        private void AddTemporaryUI()
        {
            object[] contextUIGuids = new object[] { };

            // The only command that will be created. We will create several buttons from it
            Command myCommand = null;

            // Try to retrieve the command, just in case it was already created, ignoring the 
            // exception that would happen if the command was not created yet.
            try
            {
                myCommand = VisualStudioRef.ApplicationObject.Commands.Item(AddInInstance.ProgID + "." + Config.MAIN_COMMAND_NAME, -1);
            }
            catch
            {
            }

            // Add the command if it does not exist
            if (myCommand == null)
            {
                // reference FaceId: http://www.kebabshopblues.co.uk/2007/01/04/visual-studio-2005-tools-for-office-commandbarbutton-faceid-property/
                myCommand = VisualStudioRef.ApplicationObject.Commands.AddNamedCommand(
                    AddInInstance,
                   Config.MAIN_COMMAND_NAME,
                   Config.MAIN_COMMAND_CAPTION,
                   Config.MAIN_COMMAND_TOOLTIP,
                   true,
                   Config.MY_IMAGE_ID,
                   ref contextUIGuids,
                   (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
            }

            // Retrieve the collection of commandbars
            // Note:
            // - In VS.NET 2002/2003 (which uses the Office.dll reference) 
            //   DTE.CommandBars returns directly a CommandBars type, so a cast 
            //   to CommandBars is redundant
            // - In VS 2005 or higher (which uses the new Microsoft.VisualStudio.CommandBars.dll reference) 
            //   DTE.CommandBars returns an Object type, so we do need a cast to CommandBars
            var commandBars = (CommandBars)VisualStudioRef.ApplicationObject.CommandBars;

            // Retrieve some built-in commandbars
            CommandBar menuBarCommandBar = commandBars[VSIDEConstants.VS_MENUBAR_COMMANDBAR_NAME];
            CommandBar projectCommandBar = commandBars[VSIDEConstants.VS_PROJECT_COMMANDBAR_NAME];
            CommandBar websiteCommandBar = commandBars[VSIDEConstants.VS_WEBSITE_COMMANDBAR_NAME];

            //Find the Tools command bar on the MenuBar command bar
            var toolsCommandBar = Utils.GetCommandBarPopup(menuBarCommandBar, VSIDEConstants.VS_TOOLS_COMMANDBAR_NAME);

            #region --- Add commands on Website project ---
            _websiteCommandBar = (CommandBarPopup)websiteCommandBar.Controls.Add(
                    MsoControlType.msoControlPopup,
                    Type.Missing,
                    Type.Missing,
                    1,
                    true);
            _websiteCommandBar.CommandBar.Name = Config.MAIN_COMMAND_NAME;
            _websiteCommandBar.Caption = Config.MAIN_COMMAND_CAPTION;
            _websiteCommandBar.TooltipText = Config.MAIN_COMMAND_TOOLTIP;

            var fileMappingCommandBarButton = (CommandBarButton)myCommand.AddControl(
               _websiteCommandBar.CommandBar, 1);
            fileMappingCommandBarButton.Caption = "HTM <-> CS";
            fileMappingCommandBarButton.TooltipText = "HTM <-> CS";
            fileMappingCommandBarButton.Click += fileMappingCommandBarButton_Click;
            #endregion

            #region --- Add commands on project ---
            _projectCommandBar = (CommandBarPopup)projectCommandBar.Controls.Add(
                    MsoControlType.msoControlPopup,
                    Type.Missing,
                    Type.Missing,
                    1,
                    true);
            _projectCommandBar.CommandBar.Name = Config.MAIN_COMMAND_NAME;
            _projectCommandBar.Caption = Config.MAIN_COMMAND_CAPTION;
            _projectCommandBar.TooltipText = Config.MAIN_COMMAND_TOOLTIP;

            fileMappingCommandBarButton = (CommandBarButton)myCommand.AddControl(
               _projectCommandBar.CommandBar, 1);
            fileMappingCommandBarButton.Caption = "HTM <-> CS";
            fileMappingCommandBarButton.TooltipText = "HTM <-> CS";
            fileMappingCommandBarButton.Click += fileMappingCommandBarButton_Click;
            #endregion
        }

        void fileMappingCommandBarButton_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            try
            {
                Project selectedProject = GetSelectedProject();
                if (selectedProject == null)
                {
                    MessageBox.Show("Please select a project", Config.MsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    Execute execute = new Execute(selectedProject);
                    execute.ShowFileMapping();
                }
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }

        private void RegisterEvents()
        {
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
            try
            {
                InfoLog.Log("ItemRenamed is fired");

                if (!AddInInstance.Connected)
                    return;

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
            try
            {
                InfoLog.Log("ItemRemoved is fired");

                if (!AddInInstance.Connected)
                    return;

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
            try
            {
                InfoLog.Log("DocumentSaved is fired");

                if (!AddInInstance.Connected)
                    return;

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

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            try
            {
                InfoLog.Log("OnDisconnection is called");

                switch (disconnectMode)
                {
                    case ext_DisconnectMode.ext_dm_HostShutdown:
                    case ext_DisconnectMode.ext_dm_UserClosed:
                        RemoveTemporaryUI();
                        UnregisterEvents();
                        break;
                }
            }
            catch (Exception ex)
            {
                InfoLog.Log(ex);
            }
        }

        private void RemoveTemporaryUI()
        {
            if (_websiteCommandBar != null)
            {
                _websiteCommandBar.Delete();
            }

            if (_projectCommandBar != null)
            {
                _projectCommandBar.Delete();
            }
        }

        private void UnregisterEvents()
        {
            if (_documentEvents != null)
            {
                _documentEvents.DocumentSaved -= _documentEvents_DocumentSaved;
                InfoLog.Log("DocumentSaved event is unregistered succesfully");
            }

            // unregister for website project
            if (_websiteItemsEvents != null)
            {
                _websiteItemsEvents.ItemRemoved -= _projectItemsEvents_ItemRemoved;
                _websiteItemsEvents.ItemRenamed -= _projectItemsEvents_ItemRenamed;
                InfoLog.Log("WebSiteItemsEvents' events are registered succesfully");
            }

            // unregister for other projects
            if (_projectItemsEvents != null)
            {
                _projectItemsEvents.ItemRemoved -= _projectItemsEvents_ItemRemoved;
                _projectItemsEvents.ItemRenamed -= _projectItemsEvents_ItemRenamed;
                InfoLog.Log("ProjectItemsEvents' events are unregistered succesfully");
            }
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
            InitializeAddIn();
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {

        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == AddInInstance.ProgID + "." + Config.MAIN_COMMAND_NAME)
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                }
                else
                {
                    status = vsCommandStatus.vsCommandStatusUnsupported;
                }
            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if (commandName == AddInInstance.ProgID + "." + Config.MAIN_COMMAND_NAME)
                {
                    InfoLog.Log(string.Format("Command {0} is called", commandName));
                    handled = true;
                }
            }
        }

        private bool IsSolutionLoaded()
        {
            bool result = false;
            if (VisualStudioRef.ApplicationObject.Solution.IsOpen)
            {
                result = true;
            }
            return result;
        }

        private Project GetSelectedProject()
        {
            Project result = null;

            UIHierarchy uiHierarchy = VisualStudioRef.ApplicationObject.ToolWindows.SolutionExplorer;
            if (uiHierarchy != null)
            {
                object[] items = uiHierarchy.SelectedItems as object[];
                if (items != null && items.Length > 0)
                {
                    UIHierarchyItem item0 = items[0] as UIHierarchyItem;
                    if (item0 != null)
                    {
                        var project = item0.Object as Project;
                        if (project != null)
                        {
                            result = project;
                        }
                        else
                        {
                            var projectItem = item0.Object as ProjectItem;
                            if (projectItem != null)
                            {
                                result = projectItem.ContainingProject;
                            }
                        }
                    }
                }
            }

            if (result == null)
            {
                if (VisualStudioRef.ApplicationObject.ActiveDocument != null)
                {
                    result = VisualStudioRef.ApplicationObject.ActiveDocument.ProjectItem.ContainingProject;
                }
            }

            return result;
        }
    }
}