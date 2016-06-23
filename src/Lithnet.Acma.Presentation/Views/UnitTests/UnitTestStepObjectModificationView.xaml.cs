using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Lithnet.Acma.Presentation
{
    public partial class UnitTestStepObjectModificationView
    {
        private void AttributeChangesListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AttributeChangesViewModel vm = ((ListView)sender).DataContext as AttributeChangesViewModel;

            if (vm == null)
            {
                return;
            }

            AttributeChangeViewModel attributeChange = vm.FirstOrDefault(t => t.IsSelected);

            if (attributeChange == null)
            {
                return;
            }

            ValueChangeViewModel valueChange = attributeChange.ValueChanges.FirstOrDefault(t => t.IsSelected);

            if (valueChange == null)
            {
                valueChange = attributeChange.ValueChanges.FirstOrDefault();

                if (valueChange == null)
                {
                    return;
                }
            }

            valueChange.EditValueChange();
        }

        private void ValueChangesListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ValueChangesViewModel vm = ((ListView)sender).DataContext as ValueChangesViewModel;

            if (vm == null)
            {
                return;
            }

            ValueChangeViewModel valueChange = vm.FirstOrDefault(t => t.IsSelected);

            if (valueChange == null)
            {
                return;
            }

            valueChange.EditValueChange();
        }
    }
}
