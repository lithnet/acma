using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lithnet.Acma.Presentation
{
    public class DBQueryObjectsViewModel : ListViewModel<DBQueryObjectViewModel, DBQueryObject>
    {
        private string displayName;

        private IList<DBQueryObject> model;

        public DBQueryObjectsViewModel(IList<DBQueryObject> model)
            : base()
        {
            this.model = model;
            this.Commands.AddItem("AddQueryByValue", t => this.AddDBQueryByValue());
            this.Commands.AddItem("AddQueryGroup", t => this.AddDBQueryGroup());
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.SetCollectionViewModel(this.model, t => this.ViewModelResolver(t));
            this.PasteableTypes.Add(typeof(DBQueryByValue));
            this.PasteableTypes.Add(typeof(DBQueryGroup));
        }

        public DBQueryObjectsViewModel(IList<DBQueryObject> model, string displayName)
            : this(model)
        {
            this.displayName = displayName;
        }

        public string DisplayName
        {
            get
            {
                if (this.displayName == null)
                {
                    return string.Format("Queries");
                }
                else
                {
                    return this.displayName;
                }
            }
        }

        public void AddDBQueryGroup()
        {
            this.Add(new DBQueryGroup(), true);
        }

        public void AddDBQueryByValue()
        {
            this.Add(new DBQueryByValue(), true);
        }

        private DBQueryObjectViewModel ViewModelResolver(DBQueryObject model)
        {
            if (model is DBQueryGroup)
            {
                return new DBQueryGroupViewModel(model as DBQueryGroup);
            }
            else if (model is DBQueryByValue)
            {
                return new DBQueryByValueViewModel(model as DBQueryByValue);
            }
            else
            {
                throw new ArgumentException("The object type is unknown", "model");
            }
        }
    }
}
