using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Lithnet.Fim.Transforms.Presentation
{
    public static class HyperLinkLauncher
    {
        public static void RequestNavigate(RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to open link {0}\n\n{1}", e.Uri.AbsoluteUri, ex.Message));
            }
        }
    }
}
