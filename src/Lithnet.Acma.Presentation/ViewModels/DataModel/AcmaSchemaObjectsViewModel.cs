using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaObjectsViewModel : ListViewModel<AcmaSchemaObjectViewModel, AcmaSchemaObjectClass>
    {
        private IBindingList model;

        public AcmaSchemaObjectsViewModel(IBindingList model)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(this.model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddObjectClass", t => this.CreateObjectClass());

            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Object Classes");
            }
        }

        public void CreateObjectClass()
        {
            NewObjectClassWindow window = new NewObjectClassWindow();
            NewAcmaSchemaObjectViewModel vm = new NewAcmaSchemaObjectViewModel(window, this);
            window.DataContext = vm;
            window.ShowDialog();
        }

        public void CreateObjectClass(string name, AcmaSchemaObjectClass shadowParent)
        {
            this.Add(ActiveConfig.DB.CreateObjectClass(name, shadowParent));
        }

        public void DeleteObjectClass(AcmaSchemaObjectClass objectClass)
        {
            try
            {
                if (MessageBox.Show(
                    string.Format(
                        "Are you are you want to delete this object class?\nThis will delete all objects of this class",
                        objectClass.Name),
                    "Confirm delete",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    ) != MessageBoxResult.OK)
                {
                    return;
                }

                ProgressWindow p = new ProgressWindow();
                p.DataContext = ActiveConfig.DB.ProgressInfo;
                p.Show();

                Task t = new Task(() =>
                {
                    try
                    {
                        ActiveConfig.DB.DeleteObjectClass(objectClass);

                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            this.Remove(objectClass);
                        }));
                    }
                    catch (OperationCanceledException)
                    {
                        p.InvokeClose();
                    }
                    catch (Exception ex)
                    {
                        p.InvokeClose();
                        MessageBox.Show("The operation could not be completed\n\n" + ex.Message);
                    }
                });

                t.ContinueWith((z) =>
                {
                    p.InvokeClose();
                });

                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The operation could not be completed\n\n" + ex.Message);
            }

        }

        private AcmaSchemaObjectViewModel ViewModelResolver(AcmaSchemaObjectClass model)
        {
            return new AcmaSchemaObjectViewModel(model, this);
        }
    }
}