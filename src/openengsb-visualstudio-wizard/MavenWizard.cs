using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI;
using EnvDTE80;
using VSLangProj;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Assistants
{
    public class MavenWizard
    {
        private Wizard _wizard;

        public MavenWizard(DTE2 visualStudio, VSProject project)
        {
            _wizard = new Wizard(visualStudio, project);
        }

        private void resetSteps()
        {
            _wizard.WizardSteps.Clear();

            Splash sp = new Splash();
            try
            {
                sp.Show();
                _wizard.WizardSteps.Add(new BrowserWindow(_wizard, null));
                _wizard.WizardSteps.Add(new DownloadWindow(_wizard));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sp.Close();
            }
        }

        public void DoSteps()
        {
            try
            {
                resetSteps();
                _wizard.DoWizard();
                _wizard.CreateReferences();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void DownloadBridge(bool overwrite)
        {
            Splash sp = new Splash();
            try
            {

                sp.Show();
                DownloadWindow win = new DownloadWindow(_wizard);
                _wizard.DownloadBridge(overwrite);
                sp.Close();
                win.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sp.Close();
            }
        }

        public void IncludeBridge()
        {
            try
            {
                DownloadBridge(false);
                _wizard.IncludeBridge();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void DownloadOpenEngSb()
        {
            Splash sp = new Splash();
            try
            {
                sp.Show();
                DownloadWindow win = new DownloadWindow(_wizard);
                _wizard.DownloadOpenEngSb(true);
                sp.Close();
                win.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sp.Close();
            }
        }

        public void DoBus()
        {
            Splash sp = new Splash();
            try
            {
                sp.Show();
                DownloadWindow win = new DownloadWindow(_wizard);
                _wizard.DownloadOpenEngSb(false);
                sp.Close();
                win.ShowDialog();
                _wizard.ExecuteBus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sp.Close();
            }
        }

        public void DoConfiguration()
        {
            try
            {
                new ConfigurationWindow(_wizard).ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
