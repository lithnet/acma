using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Lithnet.Common.Presentation;
using Lithnet.Fim.Core;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class DelimitedTextFileLookupTransformViewModel : TransformViewModel
    {
        private DelimitedTextFileLookupTransform model;

        private List<EnumExtension.EnumMember> allowedReturnTypes;

        public DelimitedTextFileLookupTransformViewModel(DelimitedTextFileLookupTransform model)
            : base(model)
        {
            this.model = model;
            this.Commands.AddItem("SelectFile", x => this.SelectFile());

            if (this.model.UserDefinedReturnType == ExtendedAttributeType.Undefined)
            {
                this.model.UserDefinedReturnType = ExtendedAttributeType.String;
            }
        }

        private void SelectFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".csv";
            dialog.Filter = "CSV files|*.csv|TSV files|*.tsv|All files|*.*";
            dialog.CheckFileExists = true;

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                this.FileName = dialog.FileName;
            }
        }

        public OnMissingMatch OnMissingMatch
        {
            get
            {
                return model.OnMissingMatch;
            }
            set
            {
                model.OnMissingMatch = value;
            }
        }

        public bool HasHeaderRow
        {
            get
            {
                return model.HasHeaderRow;
            }
            set
            {
                model.HasHeaderRow = value;
            }
        }

        public int FindColumn
        {
            get
            {
                return model.FindColumn;
            }
            set
            {
                model.FindColumn = value;
            }
        }

        public int ReplaceColumn
        {
            get
            {
                return model.ReplaceColumn;
            }
            set
            {
                model.ReplaceColumn = value;
            }
        }

        public string FileName
        {
            get
            {
                return model.FileName;
            }
            set
            {
                model.FileName = value;
            }
        }

        public string CommentChar
        {
            get
            {
                return model.CommentCharacter;
            }
            set 
            { 
                model.CommentCharacter = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return model.DefaultValue;
            }
            set
            {
                model.DefaultValue = value;
            }
        }

        public DelimiterType DelimiterType
        {
            get
            {
                return this.model.DelimiterType;
            }
            set
            {
                this.model.DelimiterType = value;
            }
        }

        public string CustomDelimiterRegex
        {
            get
            {
                return this.model.CustomDelimiterRegex;
            }
            set
            {
                this.model.CustomDelimiterRegex = value;
            }
        }


        public string CustomEscapeSequence
        {
            get
            {
                return this.model.CustomEscapeSequence;
            }
            set
            {
                this.model.CustomEscapeSequence = value;
            }
        }

        public bool DefaultValueIsEnabled
        {
            get
            {
                return this.OnMissingMatch == Transforms.OnMissingMatch.UseDefault;
            }
        }

        public bool CustomDelimiterRegexIsEnabled
        {
            get
            {
                return this.DelimiterType == Transforms.DelimiterType.Custom; 
            }
        }

        public ExtendedAttributeType UserDefinedReturnType
        {
            get
            {
                return this.model.UserDefinedReturnType;
            }
            set
            {
                this.model.UserDefinedReturnType = value;
            }
        }

        public IList<EnumExtension.EnumMember> AllowedReturnTypes
        {
            get
            {
                if (this.allowedReturnTypes == null)
                {
                    this.allowedReturnTypes = new List<EnumExtension.EnumMember>();

                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.String.GetEnumDescription(), Value = ExtendedAttributeType.String });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Integer.GetEnumDescription(), Value = ExtendedAttributeType.Integer });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Boolean.GetEnumDescription(), Value = ExtendedAttributeType.Boolean });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Binary.GetEnumDescription(), Value = ExtendedAttributeType.Binary });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Reference.GetEnumDescription(), Value = ExtendedAttributeType.Reference });

                    if (TransformGlobal.HostProcessSupportsNativeDateTime)
                    {
                        this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.DateTime.GetEnumDescription(), Value = ExtendedAttributeType.DateTime });
                    }
                }

                return this.allowedReturnTypes;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.DelimitedTextFileLookupTransformDescription;
            }
        }
    }
}
