using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;
using ICSharpCode.AvalonEdit.Document;

namespace Lithnet.Acma.Presentation
{
    public class ValueDeclarationViewModel : ViewModelBase<ValueDeclaration>
    {
        public ValueDeclarationViewModel()
            : base(new ValueDeclaration())
        {
            this.IgnorePropertyHasChanged.Add("SyntaxErrors");
            this.IgnorePropertyHasChanged.Add("ObjectClass");
        }

        public ValueDeclarationViewModel(ValueDeclaration model, AcmaSchemaObjectClass objectClass)
            : base(model)
        {
            this.ObjectClass = objectClass;
        }

        public AcmaSchemaObjectClass ObjectClass { get; set; }

        public IList<TokenError> SyntaxErrors { get; set; }

        public string Declaration
        {
            get
            {
                return this.Model.Declaration;
            }
            set
            {
                try
                {
                    this.Model.Declaration = value;
                    this.RemoveError("Declaration");
                }
                catch (Exception ex)
                {
                    this.AddError("Declaration", ex.Message);
                }
            }
        }

        public string TransformsString
        {
            get
            {
                return this.Model.TransformsString;
            }
            set
            {
                try
                {
                    this.Model.TransformsString = value;
                    this.RemoveError("TransformsString");
                }
                catch (Exception ex)
                {
                    this.AddError("TransformsString", ex.Message);
                }
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "SyntaxErrors")
            {
                if (this.SyntaxErrors == null || this.SyntaxErrors.Count == 0)
                {
                    this.RemoveError("Declaration");
                }
                else
                {
                    this.AddError("Declaration", this.SyntaxErrors.Select(t => t.ToString()).ToNewLineSeparatedString());
                }
            }
        }
    }
}
