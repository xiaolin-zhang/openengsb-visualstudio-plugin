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
using System.Collections.Generic;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Addin
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        const string VS_PROJECT_CMDBAR_NAME = "Project";
        const string POPUP_OPENENGSB_NAME = "OpenEngSb";
        const string POPUP_OPENENGSB_CAPTION = "OpenEngSb";

        const string CMD_BRIDGE_DOWNLOAD_NAME = "openengsbbridgedownload";
        const string CMD_BRIDGE_DOWNLOAD_CAPTION = "Download .Net Bridge";
        const string CMD_BRIDGE_DOWNLOAD_TOOLTIP = "Download the OpenEngSb .Net Bridge";

        const string CMD_BRIDGE_INCLUDE_NAME = "openengsbbridgeinclude";
        const string CMD_BRIDGE_INCLUDE_CAPTION = "Include .Net Bridge";
        const string CMD_BRIDGE_INCLUDE_TOOLTIP = "Include the OpenEngSb .Net Bridge";

        const string CMD_WIZARD_NAME = "openengsbwizard";
        const string CMD_WIZARD_CAPTION = "Start Wizard";
        const string CMD_WIZARD_TOOLTIP = "Start Artifacts Wizard";

        const string CMD_BUS_DOWNLOAD_NAME = "openengsbbusdownload";
        const string CMD_BUS_DOWNLOAD_CAPTION = "Download OpenEngSb";
        const string CMD_BUS_DOWNLOAD_TOOLTIP = "Download OpenEng Service Bus";

        const string CMD_BUS_OPEN_NAME = "openengsbbusopen";
        const string CMD_BUS_OPEN_CAPTION = "Open OpenEngSb";
        const string CMD_BUS_OPEN_TOOLTIP = "Open OpenEng Service Bus";

        const string CMD_CONFIGURE_NAME = "openengsbbusconfigure";
        const string CMD_CONFIGURE_CAPTION = "Configure Wizard";
        const string CMD_CONFIGURE_TOOLTIP = "Configure Wizard";

        private DTE2 _applicationObject;
        private AddIn _addInInstance;

        private IList<CommandBarPopup> _popups;
        private IList<CommandBarButton> _buttons;
        private IList<Command> _commmands;

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
            _popups = new List<CommandBarPopup>();
            _buttons = new List<CommandBarButton>();
            _commmands = new List<Command>();
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            if (connectMode == ext_ConnectMode.ext_cm_Startup)
            {
                _applicationObject = (DTE2)application;
                _addInInstance = (AddIn)addInInst;

                // Getting context menu of projects
                CommandBars bars = (CommandBars)_applicationObject.CommandBars;
                CommandBar vsBarProject = bars[VS_PROJECT_CMDBAR_NAME];

                // ------------------------------------------------------------------------------------                

                // Adding a menu item
                CommandBarPopup openengsbPopup = (CommandBarPopup)vsBarProject.Controls.Add(
                    MsoControlType.msoControlPopup,
                    System.Type.Missing,
                    System.Type.Missing,
                    1,
                    true);
                openengsbPopup.Caption = POPUP_OPENENGSB_CAPTION;
                openengsbPopup.CommandBar.Name = POPUP_OPENENGSB_NAME;
                _popups.Add(openengsbPopup);


                Command command = addTemporaryCommand(
                    CMD_BRIDGE_DOWNLOAD_NAME,
                    CMD_BRIDGE_DOWNLOAD_CAPTION,
                    CMD_BRIDGE_DOWNLOAD_TOOLTIP);
                addTemporaryButtonTo(command, openengsbPopup, CMD_BRIDGE_DOWNLOAD_CAPTION);

                command = addTemporaryCommand(
                    CMD_BRIDGE_INCLUDE_NAME,
                    CMD_BRIDGE_INCLUDE_CAPTION,
                    CMD_BRIDGE_INCLUDE_TOOLTIP);
                addTemporaryButtonTo(command, openengsbPopup, CMD_BRIDGE_INCLUDE_CAPTION);

                command = addTemporaryCommand(
                    CMD_WIZARD_NAME,
                    CMD_WIZARD_CAPTION,
                    CMD_WIZARD_TOOLTIP);
                addTemporaryButtonTo(command, openengsbPopup, CMD_WIZARD_CAPTION);

                command = addTemporaryCommand(
                    CMD_BUS_DOWNLOAD_NAME,
                    CMD_BUS_DOWNLOAD_CAPTION,
                    CMD_BUS_DOWNLOAD_TOOLTIP);
                addTemporaryButtonTo(command, openengsbPopup, CMD_BUS_DOWNLOAD_CAPTION);

                command = addTemporaryCommand(
                    CMD_BUS_OPEN_NAME,
                    CMD_BUS_OPEN_CAPTION,
                    CMD_BUS_OPEN_TOOLTIP);
                addTemporaryButtonTo(command, openengsbPopup, CMD_BUS_OPEN_CAPTION);

                command = addTemporaryCommand(
                    CMD_CONFIGURE_NAME,
                    CMD_CONFIGURE_CAPTION,
                    CMD_CONFIGURE_TOOLTIP);
                addTemporaryButtonTo(command, openengsbPopup, CMD_CONFIGURE_CAPTION);
            }
        }

        private Command addTemporaryCommand(string name, string caption, string tooltip)
        {
            Command command = null;
            // Try to retrieve the command, just in case it was already created, ignoring the 
            // exception that would happen if the command was not created yet.
            try
            {
                command = _applicationObject.Commands.Item(_addInInstance.ProgID + "." + name);
            }
            catch
            {
            }

            // Add the command if it does not exist
            if (command == null)
            {
                command = _applicationObject.Commands.AddNamedCommand(_addInInstance,
                   name,
                   caption,
                   tooltip,
                   true);
            }
            _commmands.Add(command);
            return command;
        }

        private void addTemporaryButtonTo(Command command, CommandBarPopup popup, string caption)
        {
            CommandBarButton button = (CommandBarButton)command.AddControl(popup.CommandBar, popup.Controls.Count + 1);

            button.Caption = caption;
            button.Style = MsoButtonStyle.msoButtonCaption;
            button.BeginGroup = false;
            button.Visible = true;

            _buttons.Add(button);
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            try
            {
                switch (disconnectMode)
                {
                    case ext_DisconnectMode.ext_dm_HostShutdown:
                    case ext_DisconnectMode.ext_dm_UserClosed:
                        clearUserTemporayUI();
                        break;
                }
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        private void clearUserTemporayUI()
        {
            foreach (CommandBarButton b in _buttons)
            {
                b.Delete();
            }

            foreach (CommandBarPopup p in _popups)
            {
                p.Delete(true);
            }

            foreach (Command c in _commmands)
            {
                c.Delete();
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
                if (commandName.StartsWith(_addInInstance.ProgID + "."))
                {
                    status = (vsCommandStatus)(vsCommandStatus.vsCommandStatusEnabled |
                       vsCommandStatus.vsCommandStatusSupported);
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

            if ((executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault))
            {
                if (commandName == buildCommandName(CMD_BRIDGE_DOWNLOAD_NAME))
                {
                    handled = true;
                    downloadBridge();
                }

                if (commandName == buildCommandName(CMD_BRIDGE_INCLUDE_NAME))
                {
                    handled = true;
                    includeBridge();
                }

                if (commandName == buildCommandName(CMD_WIZARD_NAME))
                {
                    handled = true;
                    startWizard();
                }

                if (commandName == buildCommandName(CMD_BUS_OPEN_NAME))
                {
                    handled = true;
                    openBus();
                }

                if (commandName == buildCommandName(CMD_BUS_DOWNLOAD_NAME))
                {
                    handled = true;
                    downloadBus();
                }

                if (commandName == buildCommandName(CMD_CONFIGURE_NAME))
                {
                    handled = true;
                    configureWizard();
                }
            }
        }

        private string buildCommandName(string name)
        {
            return _addInInstance.ProgID + "." + name;
        }

        private void startWizard()
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new OpenEngSbWizard(_applicationObject, (VSProject)project.Object).DoSteps();
        }

        private void downloadBridge()
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new OpenEngSbWizard(_applicationObject, (VSProject)project.Object).DownloadBridge(true);
        }

        private void includeBridge()
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new OpenEngSbWizard(_applicationObject, (VSProject)project.Object).IncludeBridge();
        }

        private void downloadBus()
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            OpenEngSbWizard wizard = new OpenEngSbWizard(_applicationObject, (VSProject)project.Object);
            wizard.DownloadOpenEngSb();
        }

        private void openBus()
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            OpenEngSbWizard wizard = new OpenEngSbWizard(_applicationObject, (VSProject)project.Object);
            wizard.DoBus();
        }

        private void configureWizard()
        {
            Array activeProjects = _applicationObject.ActiveSolutionProjects as Array;

            if (activeProjects == null)
                return;

            if (activeProjects.Length < 0)
                return;

            Project project = (Project)activeProjects.GetValue(0);

            new OpenEngSbWizard(_applicationObject, (VSProject)project.Object).DoConfiguration();
        }
    }
}