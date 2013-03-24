using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service;
using EnvDTE80;
using EnvDTE100;
using VSLangProj;
using System.Xml.Serialization;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common
{
    public class Wizard
    {
        private const double PROGRESS_COMPLETE = 1.0;

        public WizardConfiguration Configuration { get; set; }

        private FileService _fileService;
        private DTE2 _visualStudio;

        private int _progress;

        public IList<string> ProjectReferences { get; set; }

        public IList<Item> Items { get; set; }

        private VSProject _activeProject;

        public delegate void ProgressHandler(double progress);
        public event ProgressHandler ProgressChanged;

        public IList<IWizardStep> WizardSteps { get; set; }

        private bool _singleFileDownload;
        private bool _canceled;
        public Wizard(DTE2 visualStudio, VSProject project)
        {
            WizardSteps = new List<IWizardStep>();
            _fileService = new FileService();
            _fileService.FileLoadedEvent += new FileService.FileLoadedHandler(OnFileLoaded);
            _fileService.ProgressEvent += new FileService.ProgressHandler(OnProgress);
            _progress = 0;
            _visualStudio = visualStudio;
            _activeProject = project;
            Configuration = loadConfiguration(project.Project.UniqueName);
            _singleFileDownload = false;
            _canceled = false;
        }

        private string getConfigFilePath()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "OpenEngSb");
            Directory.CreateDirectory(path);
            return Path.Combine(path, _activeProject.Project.UniqueName + ".wizard.conf");
        }

        public void DoWizard()
        {
            _canceled = false;
            foreach (IWizardStep step in WizardSteps)
            {
                if (!step.DoStep())
                {
                    _canceled = true;
                    break;
                }
            }
        }

        private void OnFileLoaded(int i)
        {
            if (_singleFileDownload)
            {
                ProgressChanged(PROGRESS_COMPLETE);
            }
            else
            {
                _progress += i;
                ProgressChanged(GetProgress());
            }
        }

        private void OnProgress(int i)
        {
            if (_singleFileDownload)
            {
                ProgressChanged(((double)i) / 100);
            }
        }

        public void DownloadItems()
        {
            if (Items.Count <= 0)
            {
                ProgressChanged(PROGRESS_COMPLETE);
                return;
            }
            foreach (Item i in Items)
            {
                i.Path = _fileService.CreatePath(Configuration.ArtifactFolder, i.Name);
            }

            _singleFileDownload = false;
            _fileService.LoadFilesFrom(Items);
        }

        public bool DonwloadsComplete()
        {
            return _progress >= Items.Count;
        }

        public double GetProgress()
        {
            if (Items.Count == 0)
                return 1;

            return ((double)_progress) / Items.Count;
        }

        public void CancelDownloads()
        {
            _fileService.CancelDownloads();
        }

        public void CreateSolution()
        {
            generateDlls();
            createSolutionTemplate();
        }

        public void CreateReferences()
        {
            if (_canceled)
                return;

            generateDlls();

            foreach (Item i in Items)
            {
                _activeProject.References.Add(i.DllPath);
            }
        }

        private void generateDlls()
        {
            ProcessStartInfo srcStartInfo = null;
            if (Configuration.UseSvcutils)
                srcStartInfo = new ProcessStartInfo(Configuration.SvcutilsExePath);
            else
                srcStartInfo = new ProcessStartInfo(Configuration.WsdlExePath);

            ProcessStartInfo dllStartInfo = new ProcessStartInfo(Configuration.CscExePath);
            srcStartInfo.CreateNoWindow = true;
            srcStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            dllStartInfo.CreateNoWindow = true;
            dllStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            foreach (Item i in Items)
            {
                string parent = Path.GetDirectoryName(i.Path);
                string baseFileName = Path.GetFileNameWithoutExtension(i.Path);
                string srcFile = baseFileName + ".cs";
                string dllFile = baseFileName + ".dll";
                string srcFilePath = Path.Combine(parent, srcFile);
                string dllFilePath = Path.Combine(parent, dllFile);
                string args = "";
                Process proc = null;

                if (Configuration.UseSvcutils)
                    args = "/noConfig /o:" + srcFilePath + " " + i.Path;
                else
                    args = "/serverInterface /out:" + srcFilePath + " " + i.Path;

                srcStartInfo.Arguments = args;
                proc = Process.Start(srcStartInfo);
                proc.WaitForExit();

                args = "/target:library /out:" + dllFilePath + " " + srcFilePath;
                dllStartInfo.Arguments = args;
                proc = Process.Start(dllStartInfo);
                proc.WaitForExit();

                i.DllPath = dllFilePath;
            }
        }

        private void createSolutionTemplate()
        {
            if (_visualStudio == null)
                return;

            Solution4 solution = (Solution4)_visualStudio.Solution;
            string csTemplatePath = solution.GetProjectTemplate("ConsoleApplication.zip", "CSharp");
            string prjPath = Path.Combine(Configuration.SolutionDirectory, Configuration.ProjectName);

            solution.Create(Configuration.SolutionDirectory, Configuration.SolutionName);
            prjPath = Path.GetFullPath(prjPath);
            solution.AddFromTemplate(csTemplatePath, prjPath, Configuration.ProjectName, false);

            VSProject project = solution.Projects.Item(1).Object;
            foreach (Item i in Items)
            {
                project.References.Add(i.DllPath);
            }
        }


        public void SaveConfiguration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WizardConfiguration));
            string path = getConfigFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = File.Open(path, FileMode.Create);
            serializer.Serialize(fs, Configuration);
            fs.Close();
        }

        private WizardConfiguration loadConfiguration(string projectName)
        {
            string configFile = getConfigFilePath();
            if (File.Exists(configFile))
            {
                XmlSerializer serizalier = new XmlSerializer(typeof(WizardConfiguration));
                FileStream fs = File.OpenRead(configFile);
                return (WizardConfiguration)serizalier.Deserialize(fs);
            }
            else
            {
                WizardConfiguration conf = new WizardConfiguration();
                conf.Repositories.Add(new Repository("https://search.maven.org", "", "", Repository.Type.Maven, null));
                return conf;
            }
        }

        public void DownloadBridge(bool overwrite)
        {
            if (Configuration.BridgePath == string.Empty)
            {
                ProgressChanged(PROGRESS_COMPLETE);
                throw new ApplicationException("Bridge destination path not set!");
            }

            if (Configuration.BridgeUrl == string.Empty)
            {
                ProgressChanged(PROGRESS_COMPLETE);
                throw new ApplicationException("Bridge source URL not set!");
            }
            string unzipPath = Configuration.ArtifactFolder;

            if (File.Exists(Configuration.BridgePath))
            {
                if (!overwrite)
                {
                    ProgressChanged(PROGRESS_COMPLETE);
                    return;
                }
                File.Delete(Configuration.BridgePath);
            }
            _singleFileDownload = true;
            _fileService.LoadFileFrom(Configuration.BridgeUrl, Configuration.BridgePath,
                Configuration.BridgeRepo.User,
                Configuration.BridgeRepo.Password);
        }

        public void IncludeBridge()
        {
            if (!File.Exists(Configuration.BridgePath))
            {
                throw new ApplicationException("Bridge archive file not found!");
            }

            ArchiveService ser = new ArchiveService();
            IList<string> files = ser.Unzip(Configuration.BridgePath, Path.GetDirectoryName(Configuration.BridgePath));

            if (files.Count < 1)
                return;

            foreach (string file in files)
            {
                if (file.ToLower().EndsWith(".dll"))
                {
                    _activeProject.References.Add(file);
                }
            }
        }



        public void DownloadOpenEngSb(bool overwrite)
        {
            if (Configuration.BusPath == string.Empty)
            {
                ProgressChanged(PROGRESS_COMPLETE);
                throw new ApplicationException("OpenEngSb destination path not set!");
            }

            if (Configuration.BusUrl == string.Empty)
            {
                ProgressChanged(PROGRESS_COMPLETE);
                throw new ApplicationException("OpenEngSb source URL not set!");
            }

            if (File.Exists(Configuration.BusPath) && !overwrite)
            {
                ProgressChanged(PROGRESS_COMPLETE);
                return;
            }

            if (File.Exists(Configuration.BusPath))
            {
                File.Delete(Configuration.BusPath);
            }

            _singleFileDownload = true;
            Directory.CreateDirectory(Path.GetDirectoryName(Configuration.BusPath));
            _fileService.LoadFileFrom(Configuration.BusUrl, Configuration.BusPath,
                Configuration.BusRepo.User,
                Configuration.BusRepo.Password);
        }

        public void ExecuteBus()
        {
            if (!File.Exists(Configuration.BusPath))
            {
                throw new ApplicationException("OpenEngSb file not found!");
            }
            ArchiveService ser = new ArchiveService();
            IList<string> files = ser.Unzip(Configuration.BusPath, Path.GetDirectoryName(Configuration.BusPath));
            foreach (string file in files)
                Console.WriteLine(file);
            if (files.Count < 1)
                return;

            Process.Start(Path.GetDirectoryName(files.First()));
        }
    }
}
