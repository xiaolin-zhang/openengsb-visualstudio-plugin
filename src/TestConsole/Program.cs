using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Assistants;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI;
using System.Reflection;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;
using Moq;
using EnvDTE;
using EnvDTE80;
using VSLangProj;
namespace TestConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var env = new Mock<DTE2>();
            var vsproject = new Mock<VSProject>();
            var project = new Mock<Project>();
            project.Setup(proj => proj.UniqueName).Returns("OpenEngSbMockProject");
            vsproject.Setup(proj => proj.Project).Returns(project.Object);
            OpenEngSbWizard wizard = new OpenEngSbWizard(env.Object, vsproject.Object);
            wizard.DoConfiguration();
            //wizard.DoSteps();
            //wizard.DownloadOpenEngSb();
            //wizard.DoBus();
            //wizard.DownloadBridge(true);
            //wizard.IncludeBridge();
        }
    }
}
