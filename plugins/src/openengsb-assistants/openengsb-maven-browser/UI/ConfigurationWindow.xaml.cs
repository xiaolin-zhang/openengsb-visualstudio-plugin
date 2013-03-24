using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.UI
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private Wizard _wizard;

        private ObservableCollection<Repository> _repositories;

        public ConfigurationWindow(Wizard wizard)
        {
            _wizard = wizard;
            _repositories = new ObservableCollection<Repository>(_wizard.Configuration.Repositories);
            
            InitializeComponent();
            Commons.CenterWindow(this);

            listboxRepositories.ItemsSource = _repositories;
            listboxRepositories.DisplayMemberPath = "Location";
            loadFormData();
        }

        private void loadFormData()
        {
            labelCscPath.Content = _wizard.Configuration.CscExePath;
            labelSvcutils.Content = _wizard.Configuration.SvcutilsExePath;
            labelWsdlPath.Content = _wizard.Configuration.WsdlExePath;
            labelBridgePath.Content = _wizard.Configuration.BridgePath;
            labelBridge.Content = _wizard.Configuration.BridgeUrl;
            labelBusPath.Content = _wizard.Configuration.BusPath;
            labelBus.Content = _wizard.Configuration.BusUrl;
            labelBridgeRepo.Content = bridgeRepoString();
            labelBusRepo.Content = busRepoString();
            if (_wizard.Configuration.UseSvcutils)
                radioSvcutils.IsChecked = true;
            else
                radioWsdl.IsChecked = true;
        }

        private string bridgeRepoString()
        {
            if (_wizard.Configuration.BridgeRepo == null)
                return "Set Bridge Repository";
            return "Bridge Repo: " + _wizard.Configuration.BridgeRepo.Location;
        }

        private string busRepoString()
        {
            if (_wizard.Configuration.BusRepo == null)
                return "Set Bus Repository";
            return "Bus Repo: " + _wizard.Configuration.BusRepo.Location;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRespositoryWindow dia = new AddRespositoryWindow();
            if (!dia.ShowDialog().Value)
                return;

            _repositories.Add(dia.Repo);
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listboxRepositories.SelectedItem == null)
                return;

            Repository repo = (Repository)listboxRepositories.SelectedItem;
            ListManipulatorWindow win = new ListManipulatorWindow(repo.GroupIds);
            if (!win.ShowDialog().Value)
                return;

            repo.GroupIds = win.GetList();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listboxRepositories.SelectedItem == null)
                return;

            _repositories.Remove((Repository)listboxRepositories.SelectedItem);
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            _wizard.Configuration.Repositories = new List<Repository>(_repositories);
            _wizard.Configuration.SvcutilsExePath = (string)labelSvcutils.Content;
            _wizard.Configuration.WsdlExePath = (string)labelWsdlPath.Content;
            _wizard.Configuration.CscExePath = (string)labelCscPath.Content;
            _wizard.Configuration.UseSvcutils = radioSvcutils.IsChecked.Value;
            _wizard.SaveConfiguration();
            Close();
        }

        private void buttonWsdl_Click(object sender, RoutedEventArgs e)
        {
            labelWsdlPath.Content = Commons.ShowFileBrowser((string)labelWsdlPath.Content);
        }

        private void buttonSvcutils_Click(object sender, RoutedEventArgs e)
        {
            labelSvcutils.Content = Commons.ShowFileBrowser((string)labelSvcutils.Content);
        }

        private void buttonCsc_Click(object sender, RoutedEventArgs e)
        {
            labelCscPath.Content = Commons.ShowFileBrowser((string)labelCscPath.Content);
        }

        private void buttonBridgeRepo_Click(object sender, RoutedEventArgs e)
        {
            AddRespositoryWindow addWin = new AddRespositoryWindow();
            if (!addWin.ShowDialog().Value)
                return;

            _wizard.Configuration.BridgeRepo = addWin.Repo;
            labelBridgeRepo.Content = bridgeRepoString();
        }

        private void buttonBusRepo_Click(object sender, RoutedEventArgs e)
        {
            AddRespositoryWindow addWin = new AddRespositoryWindow();
            if (!addWin.ShowDialog().Value)
                return;

            _wizard.Configuration.BusRepo = addWin.Repo;
            labelBusRepo.Content = busRepoString();
        }

        private void buttonBridge_Click(object sender, RoutedEventArgs e)
        {
            if (_wizard.Configuration.BridgeRepo == null)
            {
                MessageBox.Show("Configure repository first!");
                return;
            }

            WizardConfiguration originalConf = _wizard.Configuration;
            _wizard.Configuration = new WizardConfiguration();
            _wizard.Configuration.Repositories = new List<Repository>();
            _wizard.Configuration.BridgeRepo = originalConf.BridgeRepo;
            _wizard.Configuration.BridgeRepo.GroupIds = new string[] { textboxBridgeId.Text };
            _wizard.Configuration.Repositories.Add(_wizard.Configuration.BridgeRepo);
            
            Splash splash = new Splash();
            splash.Show();
            BrowserWindow browser = new BrowserWindow(_wizard, (string s) => s.ToLower().EndsWith(".zip"));
            splash.Close();
            if (!browser.ShowDialog().Value)
                return;

            IList<Item> items = _wizard.Items;
            if (items.First() == null)
                return;

            Item item = items.First();
            item.Path = System.IO.Path.Combine(_wizard.Configuration.ArtifactFolder, item.Name);

            _wizard.Configuration = originalConf;
            _wizard.Configuration.BridgePath = item.Path;
            _wizard.Configuration.BridgeUrl = item.Url;
            labelBridge.Content = _wizard.Configuration.BridgeUrl;
            labelBridgePath.Content = _wizard.Configuration.BridgePath;
        }

        private void buttonBus_Click(object sender, RoutedEventArgs e)
        {
            if (_wizard.Configuration.BusRepo == null)
            {
                MessageBox.Show("Configure repository first!");
                return;
            }
            
            WizardConfiguration originalConf = _wizard.Configuration;
            _wizard.Configuration = new WizardConfiguration();
            _wizard.Configuration.Repositories = new List<Repository>();
            _wizard.Configuration.BusRepo = originalConf.BusRepo;
            _wizard.Configuration.BusRepo.GroupIds = new string[] { textboxBusId.Text };
            _wizard.Configuration.Repositories.Add(_wizard.Configuration.BusRepo);
            
            Splash splash = new Splash();
            splash.Show();
            BrowserWindow browser = new BrowserWindow(_wizard, (string s) => s.ToLower().EndsWith(".zip"));
            splash.Close();
            if (!browser.ShowDialog().Value)
                return;

            IList<Item> items = _wizard.Items;
            if (items.First() == null)
                return;

            Item item = items.First();
            item.Path = System.IO.Path.Combine(_wizard.Configuration.ArtifactFolder, item.Name);

            _wizard.Configuration = originalConf;
            _wizard.Configuration.BusPath = item.Path;
            _wizard.Configuration.BusUrl = item.Url;
            labelBus.Content = _wizard.Configuration.BusUrl;
            labelBusPath.Content = _wizard.Configuration.BusPath;
        }
    }
}
