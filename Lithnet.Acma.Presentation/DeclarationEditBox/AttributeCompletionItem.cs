using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Lithnet.Acma.DataModel;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;

namespace Lithnet.Acma.Presentation
{
    public class AttributeCompletionItem : ICompletionData
    {
        private AcmaSchemaAttribute attribute;

        public AttributeCompletionItem(AcmaSchemaAttribute attribute)
        {
            this.attribute = attribute;
        }

        public System.Windows.Media.ImageSource Image
        {
            get
            {
                return new IconProvider().GetImageForItem(this.attribute);
            }
        }

        public string Text
        {
            get
            {
                return this.attribute.Name;
            }
        }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get
            {
                return this.Text;
            }
        }

        public object Description
        {
            get
            {
                return this.attribute.Type.ToString() + (this.attribute.IsMultivalued ? " (multivalued)" : " (single-valued)");
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            if (textArea.Caret.Offset < textArea.Document.TextLength && textArea.Document.Text[textArea.Caret.Offset] == '}')
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
            else
            {
                textArea.Document.Replace(completionSegment, this.Text + "}");
            }
        }

        public double Priority
        {
            get { return 0; }
        }
    }

}
