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

namespace Lithnet.Acma.Presentation
{
    public class AcmaSchemaShadowObjectLinksViewModel : ListViewModel<AcmaSchemaShadowObjectLinkViewModel, AcmaSchemaShadowObjectLink>
    {
        private AcmaSchemaObjectClass objectClass;

        private IBindingList model;

        public AcmaSchemaShadowObjectLinksViewModel(IBindingList model, AcmaSchemaObjectClass objectClass)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel(model, t => this.ViewModelResolver(t));
            this.Commands.AddItem("AddLink", t => this.CreateLink());
            this.objectClass = objectClass;
            this.IgnorePropertyHasChanged.Add("DisplayName");
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Shadow object links");
            }
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.objectClass;
            }
        }

        public void CreateLink()
        {
            NewShadowLinkWindow window = new NewShadowLinkWindow();
            NewAcmaSchemaShadowObjectLinkViewModel vm = new NewAcmaSchemaShadowObjectLinkViewModel(window, this);
            window.DataContext = vm;
            window.ShowDialog();
        }

        public void AddLink(AcmaSchemaAttribute provisioningAttribute, AcmaSchemaAttribute referenceAttribute, string name)
        {
            this.Add(ActiveConfig.DB.CreateShadowLink(this.objectClass, provisioningAttribute, referenceAttribute, name, this.model));
        }

        public void DeleteLink(AcmaSchemaShadowObjectLink link)
        {
            {
                try
                {
                    if (MessageBox.Show(
                        "Are you are you want to delete this shadow link?\nThis will permanently delete all shadow objects that were created using this link",
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
                            ActiveConfig.DB.DeleteShadowLink(link, this.model);

                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                this.Remove(link);
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
        }

        private AcmaSchemaShadowObjectLinkViewModel ViewModelResolver(AcmaSchemaShadowObjectLink model)
        {
            return new AcmaSchemaShadowObjectLinkViewModel(model, this);
        }
    }
}