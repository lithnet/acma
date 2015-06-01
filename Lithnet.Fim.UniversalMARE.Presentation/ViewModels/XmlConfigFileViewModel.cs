using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Lithnet.Common.Presentation;
using Microsoft.Win32;
using System.ComponentModel;
using System.Linq;
using Lithnet.Fim.Transforms;
using Lithnet.Fim.UniversalMARE;
using Lithnet.Fim.Transforms.Presentation;
using System.Collections.ObjectModel;
using System.Collections;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.UniversalMARE.Presentation
{
    public class XmlConfigFileViewModel : ViewModelBase<Lithnet.Fim.UniversalMARE.XmlConfigFile>
    {
        private TransformCollectionViewModel transformCollectionViewModel;

        private FlowRuleAliasCollectionViewModel flowRulesAliasCollectionViewModel;

        public XmlConfigFileViewModel()
            : base()
        {
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            this.Model = new Lithnet.Fim.UniversalMARE.XmlConfigFile();

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            string openFile = null;

            if (commandLineArgs != null)
            {
                if (commandLineArgs.Length >= 2)
                {
                    openFile = commandLineArgs[1];
                }
            }

            if (openFile != null)
            {
                if (System.IO.File.Exists(openFile))
                {
                    this.Open(openFile);
                }
            }

            this.AddDependentPropertyNotification("FileName", "DisplayName");
            this.AddDependentPropertyNotification("HasAnyChanges", "DisplayName");
            this.IgnorePropertyHasChanged.Add("DisplayName");

            this.Commands.AddItem("New", x => this.New());
            this.Commands.AddItem("Open", x => this.Open());
            this.Commands.AddItem("Save", x => this.Save(), x => this.CanSave());
            this.Commands.AddItem("SaveAs", x => this.SaveAs(), x => this.CanSave());
            this.Commands.AddItem("Close", x => this.Close());

            this.Commands.AddItem("AddTransform", x => this.TransformCollectionViewModel.AddTransform());
            this.Commands.AddItem("DeleteTransform", x => this.DeleteTransform(), x => this.CanDeleteTransform());

            this.Commands.AddItem("AddAlias", x => this.FlowRulesAliasCollectionViewModel.AddAlias());
            this.Commands.AddItem("DeleteAlias", x => this.DeleteAlias(), x => this.CanDeleteAlias());

            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            this.TransformCollectionViewModel = new TransformCollectionViewModel(this.Model.Transforms);
            this.FlowRulesAliasCollectionViewModel = new FlowRuleAliasCollectionViewModel(this.Model.FlowRuleAliases);

            UINotifyPropertyChanges.EndIgnoreAllChanges();

            this.ResetChangeState();
        }

        public TransformCollectionViewModel TransformCollectionViewModel
        {
            get
            {
                return this.transformCollectionViewModel;
            }
            set
            {
                if (this.transformCollectionViewModel != null)
                {
                    this.UnregisterChildViewModel(this.transformCollectionViewModel);
                }

                this.transformCollectionViewModel = value;
                this.RegisterChildViewModel(this.transformCollectionViewModel);
            }
        }

        public FlowRuleAliasCollectionViewModel FlowRulesAliasCollectionViewModel
        {
            get
            {
                return this.flowRulesAliasCollectionViewModel;
            }
            set
            {
                if (this.flowRulesAliasCollectionViewModel != null)
                {
                    this.UnregisterChildViewModel(this.flowRulesAliasCollectionViewModel);
                }

                this.flowRulesAliasCollectionViewModel = value;
                this.RegisterChildViewModel(this.flowRulesAliasCollectionViewModel);
            }
        }

        private void DeleteTransform()
        {
            TransformViewModel vm = this.TransformCollectionViewModel.FirstOrDefault(t => t.IsSelected) as TransformViewModel;

            if (vm != null)
            {
                vm.ParentCollection.Remove(vm.Model);
            }
        }

        private bool CanDeleteTransform()
        {
            return this.TransformCollectionViewModel.Any(t => t.IsSelected);
        }

        private bool CanDeleteAlias()
        {
            return this.FlowRulesAliasCollectionViewModel.Any(t => t.IsSelected);
        }

        private void DeleteAlias()
        {
            FlowRuleAliasViewModel vm = this.FlowRulesAliasCollectionViewModel.FirstOrDefault(t => t.IsSelected) as FlowRuleAliasViewModel;

            if (vm != null)
            {
                vm.ParentCollection.Remove(vm.Model);
            }
        }
       
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FileName))
                {
                    return string.Format("Universal MA Rules Extension Editor - (new file){0}", this.HasAnyChanges ? "*" : string.Empty);
                }
                else
                {
                    return string.Format("Universal MA Rules Extension Editor - {0}{1}", Path.GetFileName(this.FileName), this.HasAnyChanges ? "*" : string.Empty);
                }
            }
        }

        public string FileName
        {
            get
            {
                return this.Model.FileName;
            }
        }

        [PropertyChanged.DependsOn("TransformCollectionViewModel", "FlowRuleAliasCollectionViewModel")]
        public IEnumerable<UINotifyPropertyChanges> ConfigItems
        {
            get
            {
                yield return this.TransformCollectionViewModel as UINotifyPropertyChanges;
                yield return this.FlowRulesAliasCollectionViewModel as UINotifyPropertyChanges;
            }
        }

        private void Open()
        {
            if (this.HasAnyChanges)
            {
                if (MessageBox.Show("There are unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".umarex";
            dialog.Filter = "Configuration Files (.umarex)|*.umarex";
            dialog.CheckFileExists = true;

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                this.Open(dialog.FileName);
            }
        }

        private void Open(string fileName)
        {
            try
            {
                UINotifyPropertyChanges.BeginIgnoreAllChanges();
                this.Model = ConfigManager.LoadXml(fileName);
                this.TransformCollectionViewModel = new Transforms.Presentation.TransformCollectionViewModel(this.Model.Transforms);
                this.FlowRulesAliasCollectionViewModel = new FlowRuleAliasCollectionViewModel(this.Model.FlowRuleAliases);
                UINotifyPropertyChanges.EndIgnoreAllChanges();

                this.RaisePropertyChanged("DisplayName");
                this.RaisePropertyChanged("ConfigItems");
                this.TransformCollectionViewModel.IsExpanded = true;
                this.FlowRulesAliasCollectionViewModel.IsExpanded = true;
                this.ResetChangeState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Could not open the file\n\n{0}", ex.Message), "File Open", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave()
        {
            return this.Model.Transforms.Count > 0;
        }

        private void Save()
        {
            if (this.HasErrors)
            {
                MessageBox.Show("There are one or more errors that must be fixed before the file can be saved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                this.SaveAs();
            }
            else
            {
                try
                {
                    ConfigManager.Save(this.FileName, this.Model);
                    this.ResetChangeState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Could not save the file\n\n{0}", ex.Message), "File Save", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void New()
        {
            if (this.HasAnyChanges)
            {
                if (MessageBox.Show("There are unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            this.Model = new XmlConfigFile();

            this.TransformCollectionViewModel = new Transforms.Presentation.TransformCollectionViewModel(this.Model.Transforms);
            this.FlowRulesAliasCollectionViewModel = new FlowRuleAliasCollectionViewModel(this.Model.FlowRuleAliases);
            UINotifyPropertyChanges.EndIgnoreAllChanges();

            this.RaisePropertyChanged("DisplayName");
            this.RaisePropertyChanged("ConfigItems");
            this.TransformCollectionViewModel.IsExpanded = true;
            this.ResetChangeState();
        }

        private void SaveAs()
        {
            if (this.HasErrors)
            {
                MessageBox.Show("There are one or more errors that must be fixed before the file can be saved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".umarex";
            dialog.Filter = "Configuration Files (.umarex)|*.umarex";
            dialog.FileName = this.FileName;

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    ConfigManager.Save(dialog.FileName, this.Model);

                    this.ResetChangeState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Could not save the file\n\n{0}", ex.Message), "File Save", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close()
        {
            if (this.HasAnyChanges)
            {
                if (MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            Application.Current.Shutdown();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.HasAnyChanges)
            {
                if (MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}