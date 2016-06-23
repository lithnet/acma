using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using Lithnet.Acma.DataModel;
using Lithnet.Transforms;
using System.ComponentModel;
using System.Windows.Media;
using Lithnet.MetadirectoryServices;
using Lithnet.Common.Presentation;

namespace Lithnet.Acma.Presentation
{
    public class TransformEditBox : EditBoxBase
    {
        private IList<ICompletionData> transformCompletionItems;

        public TransformEditBox()
        {
            Uri uri = new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/AcmaDLTransform.xshd", UriKind.Absolute);

            using (Stream s = Application.GetResourceStream(uri).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    this.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            this.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            this.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            ActiveConfig.XmlConfig.Transforms.CollectionChanged += Transforms_CollectionChanged;
            this.CreateTransformCompletionItems();
        }

        private IList<ICompletionData> TransformCompletionItems
        {
            get
            {
                if (this.transformCompletionItems == null)
                {
                    this.CreateTransformCompletionItems();
                }

                return this.transformCompletionItems;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.ValidateDeclaration();
        }

        private void CreateTransformCompletionItems()
        {
            this.transformCompletionItems = new List<ICompletionData>();

            foreach (Lithnet.Transforms.Transform transform in ActiveConfig.XmlConfig.Transforms.OrderBy(t => t.ID))
            {
                this.transformCompletionItems.Add(new TransformCompletionItem(transform));
            }
        }

        private void TransferCompletionItems(IList<ICompletionData> list)
        {
            foreach (ICompletionData item in this.TransformCompletionItems)
            {
                list.Add(item);
            }
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (this.completionWindow != null)
            {
                return;
            }

            if (e.Text == ">")
            {
                int previousCharacter = this.TextArea.Caret.Offset - 2;

                if (previousCharacter < 0)
                {
                    return;
                }

                if (this.baseText[previousCharacter] == '>')
                {
                    this.ShowTransformCompletionWindow();
                }
            }
        }

        private void ShowTransformCompletionWindow()
        {
            this.completionWindow = new CompletionWindow(this.TextArea);
            this.TransferCompletionItems(this.completionWindow.CompletionList.CompletionData);
            this.completionWindow.Show();
            this.completionWindow.Closed += delegate
            {
                this.completionWindow = null;
            };
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            else if (completionWindow == null)
            {
                if (!this.IsMultiLine && e.Text[0] == '\n')
                {
                    e.Handled = true;
                }

                if (e.Text != ">")
                {
                    this.ShowTransformCompletionWindow();
                }
            }

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }
      
        private void ValidateDeclaration()
        {
            if (this.textMarkerService == null)
            {
                return;
            }

            this.textMarkerService.Clear();

            TransformParser t = new TransformParser(this.baseText);

            foreach (TokenError error in t.Errors)
            {
                this.DisplayValidationError(error.Description, error.ColumnNumber, error.LineNumber);
            }
        }

        private void Transforms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.CreateTransformCompletionItems();
        }
    }
}
