using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class MissingAttributeViewModel : ViewModelBase
    {
        public MissingAttributeViewModel(Window window)
            : base()
        {
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                return "Missing attribute from schema";
            }
        }

        public string MissingAttributeName { get; set; }

        public AcmaSchemaAttribute Attribute { get; set; }

        public IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                return ActiveConfig.DB.Attributes;
            }
        }

        private bool CanCreate()
        {
            return this.Attribute != null;
        }

        private void Create(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }
    }
}
