using System.Windows.Navigation;

namespace Lithnet.Fim.Transforms.Presentation
{
    public partial class FormatStringTransformView
    {
        private void HyperLink_Navigate(object sender, RequestNavigateEventArgs e)
        {
            HyperLinkLauncher.RequestNavigate(e);
        }
    }
}
