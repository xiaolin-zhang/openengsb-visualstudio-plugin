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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();

            Assembly asm = Assembly.GetExecutingAssembly();
            String path = System.IO.Path.GetDirectoryName(asm.Location);
            path = System.IO.Path.Combine(path, "openengsb_medium.png");
            Uri uri = new Uri(path, UriKind.Relative);

            BitmapImage imgSource = new BitmapImage(uri);
            Canvas canv = new Canvas();
            canv.Background = new ImageBrush(imgSource);
            AddChild(canv);

            Width = imgSource.PixelWidth;
            Height = imgSource.PixelHeight;

            Commons.MakeTransparentBackground(this);
            Commons.CenterWindow(this);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1.0;
            animation.To = 0.5;
            animation.Duration = new Duration(TimeSpan.FromSeconds(2));
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            this.BeginAnimation(Window.OpacityProperty, animation);
        }
    }
}
