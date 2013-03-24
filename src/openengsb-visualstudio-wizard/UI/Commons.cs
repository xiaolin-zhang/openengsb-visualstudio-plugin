using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    class Commons
    {
        public static string ShowFolderBrowser(string folder)
        {
            FolderBrowserDialog dia = new FolderBrowserDialog();
            DialogResult result = dia.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folder = dia.SelectedPath;
            }

            return folder;
        }

        public static string ShowFileBrowser(string file)
        {
            FileDialog dia = new OpenFileDialog();
            DialogResult result = dia.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                file = dia.FileName;
            }

            return file;
        }

        public static void CenterWindow(Window window)
        {
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        public static void MakeTransparentBackground(Window window)
        {
            window.AllowsTransparency = true;
            window.WindowStyle = System.Windows.WindowStyle.None;
            SolidColorBrush brush = new SolidColorBrush(Colors.LightGray);
            brush.Opacity = 0.1;
            window.Background = brush;
            window.ShowInTaskbar = false;
        }
    }
}
