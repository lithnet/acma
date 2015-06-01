using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.Presentation
{
    public class DBQueryByValueViewModel : DBQueryObjectViewModel
    {
        private DBQueryByValue typedModel;

        private ValueDeclarationViewModel declaration;

        public DBQueryByValueViewModel(DBQueryByValue model)
            : base(model)
        {
            this.Commands.AddItem("DeleteQuery", t => this.Delete());
            this.typedModel = model;
            this.IgnorePropertyHasChanged.Add("DisplayName");

            if (this.typedModel.ValueDeclarations == null)
            {
                this.typedModel.ValueDeclarations = new List<ValueDeclaration>();
            }

            if (this.typedModel.ValueDeclarations.Count == 0)
            {
                this.typedModel.ValueDeclarations.Add(new ValueDeclaration());
            }

            this.Declaration = new ValueDeclarationViewModel(this.typedModel.ValueDeclarations.First(), null);

            this.EnableCutCopy();
        }

        public string DisplayName
        {
            get
            {
                if (this.SearchAttribute == null)
                {
                    return "Undefined value query";
                }
                else
                {
                    if (this.Operator == ValueOperator.IsPresent || this.Operator == ValueOperator.NotPresent)
                    {
                        return string.Format("{0} {1}", this.SearchAttribute.Name, this.Operator.GetEnumDescription().ToLower());
                    }
                    else if (this.Declaration == null || string.IsNullOrWhiteSpace(this.Declaration.Declaration ))
                    {
                        return "Undefined value query";

                    }
                    else 
                    {
                        return string.Format("{0} {1} {2}", this.SearchAttribute.Name, this.Operator.GetEnumDescription().ToLower(), this.Declaration.Declaration);
                    }
                }
            }
        }

        public ValueDeclarationViewModel Declaration
        {
            get
            {
                return this.declaration;
            }

            set
            {
                if (this.declaration != null)
                {
                    this.UnregisterChildViewModel(this.declaration);
                    this.declaration.PropertyChanged -= valueDeclaration_PropertyChanged;
                }

                this.declaration = value;

                if (this.declaration != null)
                {
                    this.RegisterChildViewModel(this.declaration);
                    this.declaration.PropertyChanged += valueDeclaration_PropertyChanged;
                }
            }
        }

        private void valueDeclaration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Declaration")
            {
                this.RaisePropertyChanged("DisplayName");
            }
            else if (e.PropertyName == "Operator")
            {
                this.RaisePropertyChanged("IsValueModificationAllowed");
            }
        }

        public AcmaSchemaAttribute SearchAttribute
        {
            get
            {
                return this.typedModel.SearchAttribute;
            }
            set
            {
                this.typedModel.SearchAttribute = value;
            }
        }

        public IEnumerable AllowedSearchAttributes
        {
            get
            {
                return ActiveConfig.DB.AttributesBindingList;
            }
        }

        public ValueOperator Operator
        {
            get
            {
                return this.typedModel.Operator;
            }
            set
            {
                this.typedModel.Operator = value;

                if (this.typedModel.Operator == ValueOperator.IsPresent || this.typedModel.Operator == ValueOperator.NotPresent)
                {
                    this.Declaration.Declaration = null;
                    this.Declaration.TransformsString = null;
                }
            }
        }

        public IEnumerable<EnumExtension.EnumMember> AllowedValueOperators
        {
            get
            {
                if (this.SearchAttribute == null)
                {
                    return null;
                }

                List<EnumExtension.EnumMember> list = new List<EnumExtension.EnumMember>();
                list.AddRange(ComparisonEngine.GetAllowedValueOperators(this.SearchAttribute.Type).Select(t => new EnumExtension.EnumMember() { Description = t.GetEnumDescription(), Value = t }));
                list.AddRange(ComparisonEngine.AllowedPresenceOperators.Select(t => new EnumExtension.EnumMember() { Description = t.GetEnumDescription(), Value = t }));
                return list;
            }
        }

        public bool IsValueModificationAllowed
        {
            get
            {
                return this.Operator != ValueOperator.NotPresent && this.Operator != ValueOperator.IsPresent;
            }
        }

        private void Delete()
        {
            try
            {
                this.ParentCollection.Remove(this.Model);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete the rule\n\n" + ex.Message);
            }
        }

        public bool IsMissingIndex
        {
            get
            {
                if (this.SearchAttribute == null)
                {
                    return false;
                }
                else
                {
                    return !this.SearchAttribute.IsIndexed;
                }
            }
        }

    }
}
