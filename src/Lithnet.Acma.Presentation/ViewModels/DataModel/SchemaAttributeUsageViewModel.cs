using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class SchemaAttributeUsageViewModel : ViewModelBase<SchemaAttributeUsage>
    {
        private AcmaSchemaMappingViewModel parentVM;

        internal static event EventHandler RequestSelectionEvent;

        public SchemaAttributeUsageViewModel(SchemaAttributeUsage model, AcmaSchemaMappingViewModel parentVM)
            : base(model)
        {
            this.Commands.AddItem("Goto", t => this.Goto());
            this.parentVM = parentVM;
            this.IgnorePropertyHasChanged.Add("ObjectID");
            this.IgnorePropertyHasChanged.Add("Path");
            this.IgnorePropertyHasChanged.Add("ObjectType");
            this.IgnorePropertyHasChanged.Add("Context");
        }

        public object UsedBy
        {
            get
            {
                return this.Model.UsedBy;
            }
        }

        public string ObjectID
        {
            get
            {
                return this.Model.ObjectID;
            }
        }

        public string Path
        {
            get
            {
                return this.Model.Path;
            }

        }

        public string ObjectType
        {
            get
            {
                return this.Model.ObjectType;
            }
        }

        public string Context
        {
            get
            {
                return this.Model.Context;
            }
        }

        private void Goto()
        {
            if (SchemaAttributeUsageViewModel.RequestSelectionEvent != null)
           {
               SchemaAttributeUsageViewModel.RequestSelectionEvent(this.UsedBy, new EventArgs());
           }
        }
    }
}