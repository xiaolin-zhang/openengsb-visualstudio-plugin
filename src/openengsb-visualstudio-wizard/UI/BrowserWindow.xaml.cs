﻿using System.Windows;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;
using System.Collections.Generic;
using System.Windows.Controls;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window, IWizardStep
    {
        private Wizard _wizard;
        private IList<TreeView> _forest;

        private Filter _filter;

        public BrowserWindow(Wizard wizard, Filter filter)
        {
            InitializeComponent();
            Commons.CenterWindow(this);

            _wizard = wizard;
            _filter = filter;
            _forest = new List<TreeView>();
            label_path.Content = _wizard.Configuration.ArtifactFolder;
            generateItemTabs(_wizard.Configuration.Repositories);
        }

        private void generateItemTabs(IList<Repository> repos)
        {
            foreach (Repository repo in repos)
            {
                TabItem tab = new TabItem();
                tab.Header = repo.Location;
                TreeView tree = new TreeView();
                tree.ItemsSource = UIArtifactService.LoadUIArtifacts(repo, _filter);
                tree.ItemTemplate = Resources["ArtifactTemplate"] as HierarchicalDataTemplate;
                tab.Content = tree;
                _forest.Add(tree);
                tabItems.Items.Add(tab);
            }
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void button_download_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            saveSelectedItems();
            Close();
        }

        private void saveSelectedItems()
        {
            _wizard.Items = new List<Item>();
            foreach (TreeView tree in _forest)
            {
                foreach (UIArtifact a in tree.Items)
                {
                    foreach (UIItemVersion v in a.Versions)
                    {
                        foreach (UIItem i in v.Items)
                        {
                            if (i.IsChecked)
                            {
                                _wizard.Items.Add(i.ItemModel);
                                UIItem domainEvents = findEventItem(v.Items, i);
                                if (domainEvents == null)
                                    continue;
                                if (domainEvents.IsChecked)
                                    continue;

                                _wizard.Items.Add(domainEvents.ItemModel);
                            }
                        }
                    }
                }
            }
        }

        private UIItem findEventItem(IList<UIItem> items, UIItem maybeDomain)
        {
            if (maybeDomain.ItemModel.Name.EndsWith("DomainEvents.wsdl"))
                return null;

            string domainEventName = maybeDomain.ItemModel.Name.Replace("Domain.wsdl", "DomainEvents.wsdl");

            foreach(UIItem i in items) 
            {
                if (i.ItemModel.Name == domainEventName)
                {
                    return i;
                }
            }
            return null;
        }

        private void button_browse_Click(object sender, RoutedEventArgs e)
        {
            _wizard.Configuration.ArtifactFolder =
                Commons.ShowFolderBrowser(_wizard.Configuration.ArtifactFolder);
            label_path.Content = _wizard.Configuration.ArtifactFolder;
        }

        public bool DoStep()
        {
            return ShowDialog().Value;
        }
    }
}
