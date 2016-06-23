using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class DBQueryGroupViewModel : DBQueryObjectViewModel
    {
        private DBQueryGroup typedModel;

        private DBQueryObjectsViewModel dbqueryObjects;

        private string displayName;

        public DBQueryGroupViewModel(DBQueryGroup model)
            : base(model)
        {
            this.Commands.AddItem("DeleteQueryGroup", t => this.DeleteQueryGroup());
            this.Commands.AddItem("AddQueryByValue", t => this.AddDBQueryByValue());
            this.Commands.AddItem("AddQueryGroup", t => this.AddDBQueryGroup());
            this.typedModel = model;
            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.DBQueryObjects = new DBQueryObjectsViewModel(this.typedModel.DBQueries);
            this.Commands.AddItem("Paste", t => this.DBQueryObjects.Paste(), t => this.DBQueryObjects.CanPaste());

            this.EnableCutCopy();
        }

        public DBQueryGroupViewModel(DBQueryGroup model, string displayName)
            : this(model)
        {
            this.displayName = displayName;
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.displayName))
                {
                    return string.Format("Query group ({0})", this.Operator.ToString().ToLower());
                }
                else
                {
                    return string.Format("{0} ({1})", this.displayName, this.Operator.ToString().ToLower());
                }
            }
        }

        public string Description
        {
            get
            {
                return this.typedModel.Description;
            }
            set
            {
                this.typedModel.Description = value;
            }
        }

        public GroupOperator Operator
        {
            get
            {
                return this.typedModel.Operator;
            }
            set
            {
                this.typedModel.Operator = value;
            }
        }


        public DBQueryObjectsViewModel DBQueryObjects
        {
            get
            {
                return this.dbqueryObjects;
            }
            set
            {
                if (this.dbqueryObjects != null)
                {
                    this.UnregisterChildViewModel(this.dbqueryObjects);
                }

                this.dbqueryObjects = value;
                this.RegisterChildViewModel(this.dbqueryObjects);
            }
        }

        private void DeleteQueryGroup()
        {
            try
            {
                if (this.DBQueryObjects.Count > 0)
                {
                    if (MessageBox.Show("Are you are you want to delete this group?", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                if (this.ParentCollection != null)
                {
                    this.ParentCollection.Remove(this.Model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the group\n\n" + ex.Message);
            }
        }

        private void AddDBQueryGroup()
        {
            this.DBQueryObjects.AddDBQueryGroup();
        }

        private void AddDBQueryByValue()
        {
            this.DBQueryObjects.AddDBQueryByValue();
        }

    }
}
