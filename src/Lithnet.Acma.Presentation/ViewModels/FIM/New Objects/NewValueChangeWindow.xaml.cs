using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Lithnet.Acma.Presentation
{
    public partial class NewValueChangeWindow : Window
    {
        public NewValueChangeWindow()
        {
            InitializeComponent();
        }

        private void Value_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    e.Handled = true;
                    TextBox textEdit = (TextBox)sender;
                    int carretPos = textEdit.CaretIndex;
                    textEdit.Text = textEdit.Text.Insert(carretPos, Environment.NewLine);
                    textEdit.CaretIndex = carretPos + 1;
                }
                else
                {
                    e.Handled = true;

                    if (this.ButtonOK.Command.CanExecute(null))
                    {
                        this.ButtonOK.Command.Execute(null);
                    }
                }
            }
        }
    }
}
