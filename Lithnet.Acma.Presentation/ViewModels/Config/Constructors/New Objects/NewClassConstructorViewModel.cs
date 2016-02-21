using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class NewClassConstructorViewModel : ViewModelBase
    {
        public NewClassConstructorViewModel(Window window)
            : base()
        {
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.ObjectClass = this.AvailableObjectClasses.FirstOrDefault();
        }

        public string DisplayName
        {
            get
            {
                return "Create New Class Constructor";
            }
        }

        public AcmaSchemaObjectClass ObjectClass { get; set; }

        public IEnumerable<AcmaSchemaObjectClass> AvailableObjectClasses
        {
            get
            {
                return ActiveConfig.DB.ObjectClasses.Except(ActiveConfig.XmlConfig.ClassConstructors.Select(t => t.ObjectClass));
            }
        }

        private bool CanCreate()
        {
            return this.ObjectClass != null;
        }

        private void Create(Window window)
        {
            try
            {
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the constructor\n\n" + ex.Message);
            }

        }
    }
}
