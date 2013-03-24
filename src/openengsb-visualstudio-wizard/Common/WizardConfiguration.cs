using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VSLangProj;
using System.Xml.Serialization;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common
{
    public class WizardConfiguration
    {
        public string CscExePath { get; set; }
        public string WsdlExePath { get; set; }
        public string SvcutilsExePath { get; set; }

        public bool UseSvcutils { get; set; }

        public string ArtifactFolder { get; set; }

        public string SolutionName { get; set; }
        public string ProjectName { get; set; }
        public string SolutionDirectory { get; set; }

        public Repository BridgeRepo { get; set; }
        public string BridgeUrl { get; set; }
        public string BridgePath { get; set; }

        public Repository BusRepo { get; set; }
        public string BusUrl { get; set; }
        public string BusPath { get; set; }

        public List<Repository> Repositories { get; set; }

        public WizardConfiguration()
        {
            ArtifactFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Repositories = new List<Repository>();
            CscExePath = "";
            WsdlExePath = "";
            UseSvcutils = false;
            SolutionDirectory = "";
            BridgeRepo = null;
            BridgeUrl = "";
            BridgePath = "";
            BusRepo = null;
            BusUrl = "";
            BusPath = "";
            locateCsharpCompiler();
            locateWsdlCompiler();
            locateSvcUtilCompiler();
        }

        private void locateCsharpCompiler()
        {
            string path = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe";
            if (File.Exists(path))
                CscExePath = path;

            path = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";
            if (File.Exists(path))
                CscExePath = path;

            if (CscExePath == string.Empty)
                throw new ApplicationException("csc.exe not found");
        }

        private void locateSvcUtilCompiler()
        {
            string path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\svcutil.exe";
            if (File.Exists(path))
                SvcutilsExePath = path;

            path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\x64\svcutil.exe";
            if (File.Exists(path))
                SvcutilsExePath = path;

            path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\svcutil.exe";
            if (File.Exists(path))
                SvcutilsExePath = path;

            path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\svcutil.exe";
            if (File.Exists(path))
                SvcutilsExePath = path;

            if (SvcutilsExePath == string.Empty)
                throw new ApplicationException("svcutil.exe not found");
        }

        private void locateWsdlCompiler()
        {
            string path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\wsdl.exe";
            if (File.Exists(path))
                WsdlExePath = path;

            path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\x64\wsdl.exe";
            if (File.Exists(path))
                WsdlExePath = path;

            path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\wsdl.exe";
            if (File.Exists(path))
                WsdlExePath = path;

            path = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\wsdl.exe";
            if (File.Exists(path))
                WsdlExePath = path;

            if (WsdlExePath == string.Empty)
                throw new ApplicationException("wsdl.exe not found");
        }
    }
}