using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.UI;
using System.Reflection;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;
using Moq;
using EnvDTE;
using EnvDTE80;
using VSLangProj;
using Org.OpenEngSB.
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
            MavenWizard wizard = new MavenWizard(env.Object, vsproject.Object);
            IAuditingDomainSoapBinding 
            wizard.DoConfiguration();
            //wizard.DoSteps();
            wizard.DownloadOpenEngSb();
            //wizard.DoBus();
            //wizard.DownloadBridge(true);
            //wizard.IncludeBridge();
        }
    }
}
