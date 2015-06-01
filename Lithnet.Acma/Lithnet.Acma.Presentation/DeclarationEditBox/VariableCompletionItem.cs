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
    public class VariableCompletionItem : ICompletionData
    {
        private VariableDeclaration variable;

        public VariableCompletionItem(string variableName)
        {
            this.variable = new VariableDeclarationParser("%" + variableName + "%").GetVariableDeclaration();
        }

        public System.Windows.Media.ImageSource Image
        {
            get
            {
                return null;
            }
        }

        public string Text
        {
            get
            {
                return this.variable.VariableName;
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
                try
                {
                    return this.variable.Expand().FirstOrDefault();
                }
                catch
                {
                    return null;
                }
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        public double Priority
        {
            get { return 0; }
        }
    }

}
