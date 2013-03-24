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
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.UI
{
    /// <summary>
    /// Interaction logic for AddRespositoryWindow.xaml
    /// </summary>
    public partial class AddRespositoryWindow : Window
    {
        public Repository Repo { get; private set; }

        public AddRespositoryWindow()
        {
            Repo = null;
            
            InitializeComponent();
            Commons.CenterWindow(this);

            comboBoxType.ItemsSource = Enum.GetValues(typeof(Repository.Type));
            comboBoxType.SelectedIndex = 0;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLocation.Text == string.Empty)
            {
                MessageBox.Show("Locaton must not be empty!");
                return;
            }

            Repo = new Repository(
                textBoxLocation.Text,
                textboxUser.Text,
                passwordBox.Password,
                (Repository.Type) comboBoxType.SelectedValue,
                null);
            this.DialogResult = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Repo = null;
            this.DialogResult = false;
            this.Close();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            textBoxLocation.Text = Commons.ShowFolderBrowser(textboxUser.Text);
        }
    }
}
