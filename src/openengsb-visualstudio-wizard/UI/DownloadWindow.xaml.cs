using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.UI
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window, IWizardStep
    {
        private Wizard _wizard;
        private IWizardStep _nextStep;
        private bool _canceled;

        public DownloadWindow(Wizard wizard)
        {
            InitializeComponent();
            Commons.CenterWindow(this);

            _canceled = false;
            _wizard = wizard;
            _wizard.ProgressChanged += new Wizard.ProgressHandler(UpdateProgress);
            _nextStep = null;
            progressBar.Maximum = 1;
            progressBar.Minimum = 0;
        }

        public void UpdateProgress(double progress)
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                progressBar.Value = progress;
                
                if (progress >= 1.0)
                {
                    Close();

                    if (this._nextStep == null)
                        return;

                    _nextStep.DoStep();
                }
            });
        }

        public bool DoStep()
        {
            _wizard.DownloadItems();
            _canceled = false;
            ShowDialog();
            return !_canceled;
        }

        public void SetNextStep(IWizardStep step)
        {
            _nextStep = step;
        }

        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            _wizard.CancelDownloads();
            Close();
            _canceled = true;
        }
    }
}