using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Assistants;
using System.Windows.Forms;
using VSLangProj;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Addin
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
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
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;

            if (connectMode == ext_ConnectMode.ext_cm_Startup)
            {
                // Getting context menu of projects
                CommandBars bars = (CommandBars)_applicationObject.CommandBars;
                CommandBar vsBarProject = bars["Project"];

                // Adding a menu item
                CommandBarPopup openengsbPopup = (CommandBarPopup)vsBarProject.Controls.Add(
                    MsoControlType.msoControlPopup,
                    System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, 1, true);
                openengsbPopup.Caption = "OpenEngSb";

                // Adding submenu
                CommandBarButton button = (CommandBarButton)openengsbPopup.Controls.Add(MsoControlType.msoControlButton);
                button.Caption = "Start Wizard";
                button.Click += new _CommandBarButtonEvents_ClickEventHandler(OnDownloadArtifactsClick);

                button = (CommandBarButton)openengsbPopup.Controls.Add(MsoControlType.msoControlButton);
                button.Caption = "Download .NET Bridge";
                button.Click += new _CommandBarButtonEvents_ClickEventHandler(OnDownloadBridgeClick);

                button = (CommandBarButton)openengsbPopup.Controls.Add(MsoControlType.msoControlButton);
                button.Caption = "Include .NET Bridge";
                button.Click += new _CommandBarButtonEvents_ClickEventHandler(OnIncludeBridgeClick);

                button = (CommandBarButton)openengsbPopup.Controls.Add(MsoControlType.msoControlButton);
                button.Caption = "Download OpenEngSb";
                button.Click += new _CommandBarButtonEvents_ClickEventHandler(OnBusClick);

                button = (CommandBarButton)openengsbPopup.Controls.Add(MsoControlType.msoControlButton);
                button.Caption = "Open OpenEngSb";
                button.Click += new _CommandBarButtonEvents_ClickEventHandler(OnOpenBusClick);

                button = (CommandBarButton)openengsbPopup.Controls.Add(MsoControlType.msoControlButton);
                button.Caption = "Configure";
                button.Click += new _CommandBarButtonEvents_ClickEventHandler(OnConfigureClick);
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
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
        }

        /// <summary>
        /// The event handler associated with the sub menu item.
        /// event handler for Click event.
        /// </summary>
        public void OnDownloadArtifactsClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new MavenWizard(_applicationObject, (VSProject)project.Object).DoSteps();
        }

        /// <summary>
        /// The event handler associated with the sub menu item.
        /// event handler for Click event.
        /// </summary>
        protected void OnDownloadBridgeClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new MavenWizard(_applicationObject, (VSProject)project.Object).DownloadBridge(true);
        }

        /// <summary>
        /// The event handler associated with the sub menu item.
        /// event handler for Click event.
        /// </summary>
        protected void OnIncludeBridgeClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new MavenWizard(_applicationObject, (VSProject)project.Object).IncludeBridge();
        }

        /// <summary>
        /// The event handler associated with the sub menu item.
        /// event handler for Click event.
        /// </summary>
        protected void OnBusClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            MavenWizard wizard = new MavenWizard(_applicationObject, (VSProject)project.Object);
            wizard.DownloadOpenEngSb();
        }

        /// <summary>
        /// The event handler associated with the sub menu item.
        /// event handler for Click event.
        /// </summary>
        protected void OnOpenBusClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            MavenWizard wizard = new MavenWizard(_applicationObject, (VSProject)project.Object);
            wizard.DoBus();
        }

        /// <summary>
        /// The event handler associated with the sub menu item.
        /// event handler for Click event.
        /// </summary>
        protected void OnConfigureClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new MavenWizard(_applicationObject, (VSProject)project.Object).DoConfiguration();
        }

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
    }
}