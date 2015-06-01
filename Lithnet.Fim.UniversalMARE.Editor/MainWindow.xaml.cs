using System.Windows;
using Lithnet.Fim.Transforms;

namespace Lithnet.Fim.UniversalMARE.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new Lithnet.Fim.UniversalMARE.Presentation.XmlConfigFileViewModel();
            TransformGlobal.HostProcessSupportsLoopbackTransforms = true;
        }
    }
}
