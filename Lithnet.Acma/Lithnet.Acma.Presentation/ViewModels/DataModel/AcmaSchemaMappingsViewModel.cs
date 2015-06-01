using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaMappingsViewModel : ListViewModel<AcmaSchemaMappingViewModel, AcmaSchemaMapping>
    {
        private AcmaSchemaObjectClass objectClass;

        private IBindingList model;

        public AcmaSchemaMappingsViewModel(IBindingList model, AcmaSchemaObjectClass objectClass)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddMapping", t => this.CreateMapping());

            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.objectClass = objectClass;
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.objectClass;
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Attribute bindings");
            }
        }

        public void CreateMapping()
        {
            NewMappingWindow window = new NewMappingWindow();
            NewAcmaSchemaMappingViewModel vm = new NewAcmaSchemaMappingViewModel(window, this);
            window.DataContext = vm;
            window.ShowDialog();
        }

        public void AddMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute)
        {
            this.Add(ActiveConfig.DB.CreateMapping(objectClass, attribute, this.model));
        }

        public void AddMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute, AcmaSchemaAttribute inheritanceSource, AcmaSchemaObjectClass inheritanceSourceObjectClass, AcmaSchemaAttribute inheritedAttribute)
        {
            AcmaSchemaMapping mapping;

            if (inheritanceSource == null && inheritedAttribute == null)
            {
                mapping = ActiveConfig.DB.CreateMapping(objectClass, attribute, this.model);
            }
            else
            {
                mapping = ActiveConfig.DB.CreateMapping(objectClass, attribute, inheritanceSource, inheritanceSourceObjectClass, inheritedAttribute, this.model);
            }

            this.Add(mapping);
        }

        public void DeleteMapping(AcmaSchemaMapping mapping)
        {
            try
            {
                if (MessageBox.Show(
                    string.Format(
                        "Are you are you want to delete this mapping?\nThis will delete all values for {{{0}}} in objects of the class {1}",
                        mapping.Attribute.Name,
                        mapping.ObjectClass.Name),
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
                            ActiveConfig.DB.DeleteMapping(mapping, this.model);

                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                this.Remove(mapping);
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

        private AcmaSchemaMappingViewModel ViewModelResolver(AcmaSchemaMapping model)
        {
            return new AcmaSchemaMappingViewModel(model, this);
        }
    }
}