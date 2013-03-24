/***
* Licensed to the Austrian Association for Software Tool Integration (AASTI)
* under one or more contributor license agreements. See the NOTICE file
* distributed with this work for additional information regarding copyright
* ownership. The AASTI licenses this file to you under the Apache License,
* Version 2.0 (the "License"); you may not use this file except in compliance
* with the License. You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
***/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using HtmlAgilityPack;
using Ionic.Zip;
using forms = System.Windows.Forms;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Console
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        public const string OpenEngSBBaseUri = "http://uk1.maven.org/maven2/org/openengsb/openengsb/";
        private static readonly string PluginConfig = string.Format(@"{0}\openengsb\plugin.config", Environment.GetEnvironmentVariable("APPDATA"));

        public static readonly RoutedUICommand StartOpenEngSB;
        public static readonly RoutedUICommand StopOpenEngSB;
        public static readonly RoutedUICommand DownloadOpenEngSB = new RoutedUICommand("Download", "Download", typeof(ConsoleControl));

        ObservableCollection<OpenEngSBVersion> openEngSBVersions;

        static ConsoleControl()
        {
            InputGestureCollection startGestures = new InputGestureCollection();
            startGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control, "Ctrl + R"));
            StartOpenEngSB = new RoutedUICommand("Start OpenEngSB", "Start", typeof(ConsoleControl), startGestures);

            InputGestureCollection stopGestures = new InputGestureCollection();
            stopGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl + C"));
            StopOpenEngSB = new RoutedUICommand("Stop OpenEngSB", "Stop", typeof(ConsoleControl), stopGestures);
        }

        private Process openEngSBProcess;
        private Paragraph outputParagraph;

        public ConsoleControl()
        {
            try
            {
                if (File.Exists(PluginConfig))
                {
                    var serializer = new XmlSerializer(typeof(OpenEngSBVersion[]));

                    using (var sr = new StreamReader(PluginConfig))
                    {
                        this.openEngSBVersions = new ObservableCollection<OpenEngSBVersion>((OpenEngSBVersion[])serializer.Deserialize(sr));
                    }
                }
                else
                    this.openEngSBVersions = new ObservableCollection<OpenEngSBVersion>();

                this.openEngSBVersions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(openEngSBVersions_CollectionChanged);
            }
            catch (Exception)
            {
            }

            InitializeComponent();

            List<string> commands = new List<string>();

            using (var sr = new StreamReader(GetType().Assembly.GetManifestResourceStream("Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Console.Resources.commands.txt")))
            {
                while (!sr.EndOfStream)
                {
                    commands.Add(sr.ReadLine());
                }
            }

            tbConsoleInput.AutoCompleteSource = commands;

            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(Path.GetTempPath(), "*.openengsb-*"))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                });

            bStart.ContextMenu.ItemsSource = this.openEngSBVersions;
        }

        void openEngSBVersions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OpenEngSBVersion[]));

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PluginConfig));

                using (var sw = new StreamWriter(PluginConfig))
                {
                    serializer.Serialize(sw, this.openEngSBVersions.ToArray());
                }
            }
            catch (Exception)
            {
            }
        }

        void stopBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.openEngSBProcess.Close();
            this.openEngSBProcess = null;
            CommandManager.InvalidateRequerySuggested();
        }

        void stopBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.openEngSBProcess != null);
        }

        void startBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.openEngSBProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            var param = e.Parameter as OpenEngSBVersion;

            if (param == null)
                startInfo.FileName = @"c:\Windows\System32\cmd.exe";
            else
                startInfo.FileName = param.GetDefaultBinPath();
            //startInfo.FileName = @"C:\Temp\Openengsb\openengsb-2.0.1\bin\openengsb.bat ";
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.ErrorDialog = false;
            startInfo.CreateNoWindow = true;

            this.outputParagraph = new Paragraph();
            this.outputParagraph.FontFamily = new FontFamily("Courier New");

            Hyperlink link = new Hyperlink(new Run("http://localhost:8090/openengsb/index"));
            link.NavigateUri = new Uri("http://localhost:8090/openengsb/index");
            link.TargetName = "_blank";
            link.IsEnabled = true;
            link.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(link_RequestNavigate);
            link.Cursor = Cursors.Hand;
            link.Click += new RoutedEventHandler(link_Click);

            Bold header = new Bold();

            if (param != null)
            {
                header.Inlines.Add(new Run(string.Format("OpenEngSB {0} has been started an can be administrated at ", param.Version)));
                header.Inlines.Add(link);
            }
            else
                header.Inlines.Add(new Run(string.Format("{0} has been started", startInfo.FileName)));

            tbConsoleOutput.Document.Blocks.Clear();
            tbConsoleOutput.Document.Blocks.Add(new Paragraph(header));
            tbConsoleOutput.Document.Blocks.Add(this.outputParagraph);

            this.openEngSBProcess.StartInfo = startInfo;
            this.openEngSBProcess.Exited += new System.EventHandler(openEngSBProcess_Exited);
            this.openEngSBProcess.OutputDataReceived += new DataReceivedEventHandler(openEngSBProcess_OutputDataReceived);
            this.openEngSBProcess.ErrorDataReceived += new DataReceivedEventHandler(openEngSBProcess_ErrorDataReceived);
            this.openEngSBProcess.Start();
            this.openEngSBProcess.BeginErrorReadLine();
            this.openEngSBProcess.BeginOutputReadLine();

            CommandManager.InvalidateRequerySuggested();
        }

        void link_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;

            if (link != null)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(link.NavigateUri.ToString());
                Process p = new Process();

                p.StartInfo = startInfo;
                p.Start();
            }
        }

        void link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(e.Uri.ToString());
            Process p = new Process();

            p.StartInfo = startInfo;
            p.Start();
        }

        void openEngSBProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string line = e.Data;

            if (line != null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    this.outputParagraph.Inlines.Add(new Run(line) { Foreground = Brushes.Red });
                    this.outputParagraph.Inlines.Add(new LineBreak());
                    tbConsoleOutput.ScrollToEnd();
                }), DispatcherPriority.Send);
            }
        }

        void openEngSBProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string line = e.Data;

            if (line != null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    this.outputParagraph.Inlines.Add(new Run(line));
                    this.outputParagraph.Inlines.Add(new LineBreak());
                    tbConsoleOutput.ScrollToEnd();
                }), DispatcherPriority.Send);
            }
        }

        void openEngSBProcess_Exited(object sender, System.EventArgs e)
        {
            this.openEngSBProcess = null;
        }

        void startBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.openEngSBProcess == null);
        }

        private void bExecute_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbConsoleInput.Text) && this.openEngSBProcess != null && this.openEngSBProcess.StandardInput != null)
                this.openEngSBProcess.StandardInput.WriteLine(tbConsoleInput.Text.Trim());

            tbConsoleInput.Text = string.Empty;
        }

        private void bDownload_Click(object sender, RoutedEventArgs e)
        {
            HtmlDocument html = new HtmlDocument();
            WebClient wClient = new WebClient();

            string rawVersions = wClient.DownloadString(OpenEngSBBaseUri);

            html.LoadHtml(rawVersions);
            var pre = html.DocumentNode.SelectSingleNode("//pre");
            List<OpenEngSBVersion> versions = new List<OpenEngSBVersion>();
            string tmpVersion = null;

            foreach (var node in pre.ChildNodes)
            {
                if (tmpVersion == null)
                    tmpVersion = node.InnerText;
                else
                {
                    if (tmpVersion.EndsWith("/") && !tmpVersion.StartsWith("."))
                    {
                        versions.Add(new OpenEngSBVersion { Version = tmpVersion.TrimEnd('/'), Date = node.InnerText.Trim(' ', '-', '\n', '\r') });
                    }
                    tmpVersion = null;
                }
            }

            versions.Reverse();

            lvVersions.ItemsSource = versions;
            lvVersions.SelectedIndex = 0;
            pDownload.IsOpen = true;
        }

        private void bCancelDownload_Click(object sender, RoutedEventArgs e)
        {
            pDownload.IsOpen = false;
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            var version = (OpenEngSBVersion)e.UserState;

            tbStatusText.Text = string.Format("{0}/{1} OpenEngSB {2} is being downloaded.", BytesToString(e.BytesReceived), BytesToString(e.TotalBytesToReceive), version.Version);
        }

        private string BytesToString(long bytes)
        {
            double[] steps = new double[] { 1, 1024, (1024 * 1024), (1024 * 1024 * 1024) };
            string[] stepDescs = new string[] { "Bytes", "KB", "MB", "GB" };
            int step;

            for (step = 0; step < steps.Length && steps[step] < bytes; step++) ;

            step--;

            if (step < 0)
                step = 0;

            return (bytes / steps[step]).ToString("F") + stepDescs[step];
        }

        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var paths = (OpenEngSBVersion)e.UserState;
            ZipFile zip = new ZipFile(paths.TempPath);

            zip.ExtractAll(paths.BaseFolder, ExtractExistingFileAction.OverwriteSilently);
            spStatusBar.Visibility = System.Windows.Visibility.Collapsed;

            paths.BinPath = paths.GetDefaultBinPath();

            this.openEngSBVersions.Add(paths);
        }

        private void DownloadBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (e.Parameter != null && e.Parameter is OpenEngSBVersion);
        }

        private void DownloadBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new forms::FolderBrowserDialog() { Description = "Choose the folder where OpenEngSB should be installed." };
            var dlgResult = dlg.ShowDialog();

            if (dlgResult == forms::DialogResult.OK)
            {
                string tmpFile = Path.GetTempFileName();

                File.Delete(tmpFile);

                WebClient client = new WebClient();
                var version = (OpenEngSBVersion)e.Parameter;

                tmpFile = string.Format("{0}.openengsb-{1}", tmpFile, version.Version);

                version.TempPath = tmpFile;
                version.BaseFolder = dlg.SelectedPath;

                spStatusBar.Visibility = System.Windows.Visibility.Visible;

                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileAsync(version.GetFullUri(), tmpFile, version);
            }
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            foreach (var item in this.openEngSBVersions)
            {
                if (item != mi.Tag)
                    item.IsChecked = false;
                else
                    bStart.CommandParameter = item;
            }
        }
    }

    public class OpenEngSBVersion : INotifyPropertyChanged
    {
        public string Version { get; set; }
        public string Date { get; set; }

        [XmlIgnore]
        public string BaseFolder { get; set; }
        [XmlIgnore]
        public string Path { get; set; }

        public string BinPath { get; set; }

        [XmlIgnore]
        public string TempPath { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;

                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Uri GetFullUri()
        {
            return new Uri(string.Format("{0}{1}/openengsb-{1}.zip", ConsoleControl.OpenEngSBBaseUri, Version));
        }

        public string GetDefaultBinPath()
        {
            if (!string.IsNullOrEmpty(BinPath))
                return BinPath;
            if (!string.IsNullOrEmpty(Path))
                return string.Format(@"{0}\bin\openengsb.bat", Path.TrimEnd('\\'));
            if (!string.IsNullOrEmpty(BaseFolder))
                return string.Format(@"{0}\openengsb-{1}\bin\openengsb.bat", BaseFolder.TrimEnd('\\'), Version);
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}