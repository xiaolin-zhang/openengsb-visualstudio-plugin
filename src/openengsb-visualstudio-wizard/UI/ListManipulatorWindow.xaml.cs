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

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.UI
{
    /// <summary>
    /// Interaction logic for ListManipulatorWindow.xaml
    /// </summary>
    public partial class ListManipulatorWindow : Window
    {
        private ObservableCollection<string> _collection;
        private bool _changed;
        
        public ListManipulatorWindow(IList<string> list)
        {
            InitializeComponent();
            Commons.CenterWindow(this);

            _collection = new ObservableCollection<string>(list);
            listBox.ItemsSource = _collection;
            _changed = false;
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
                return;

            _collection.Remove((string)listBox.SelectedItem);
            _changed = true;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == string.Empty)
                return;

            _collection.Add(textBox.Text);
            textBox.Text = "";
            _changed = true;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = _changed;
        }

        public string[] GetList()
        {
            return _collection.ToArray();
        }
    }
}
