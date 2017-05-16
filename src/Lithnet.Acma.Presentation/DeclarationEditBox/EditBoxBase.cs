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
    public abstract class EditBoxBase : TextEditor, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected CompletionWindow completionWindow;

        protected TextMarkerService textMarkerService;

        private ToolTip toolTip;

        public EditBoxBase()
        {
            this.textMarkerService = new TextMarkerService(this);
            TextView textView = this.TextArea.TextView;
            textView.BackgroundRenderers.Add(textMarkerService);
            textView.LineTransformers.Add(textMarkerService);
            textView.Services.AddService(typeof(TextMarkerService), textMarkerService);
            textView.MouseHover += textView_MouseHover;
            textView.MouseHoverStopped += textView_TextEditorMouseHoverStopped;
            textView.VisualLinesChanged += textView_VisualLinesChanged;
            
            this.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Hidden;
            this.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
            this.GotKeyboardFocus += EditBoxBase_GotKeyboardFocus;
            
        }

        public bool IsMultiLine { get; set; }

        protected void DisplayValidationError(string message, int column, int lineNumber)
        {
            if (lineNumber >= 1 && lineNumber <= this.Document.LineCount)
            {
                int offset = this.Document.GetOffset(new TextLocation(lineNumber, column));
                int endOffset = TextUtilities.GetNextCaretPosition(this.Document, offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
                if (endOffset < 0)
                {
                    endOffset = this.Document.TextLength;
                }
                int length = endOffset - offset;

                if (length < 2)
                {
                    length = Math.Min(2, this.Document.TextLength - offset);
                }

                this.textMarkerService.Create(offset, length, message);
            }
        }

        protected void TransferCompletionData(IList<ICompletionData> source, IList<ICompletionData> target)
        {
            target.Clear();

            foreach (ICompletionData item in source.OrderBy(t => t.Text))
            {
                target.Add(item);
            }
        }
        public new string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        internal string baseText { get { return base.Text; } set { base.Text = value; } }

        public static DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(EditBoxBase),
            new PropertyMetadata((obj, args) =>
            {
                EditBoxBase target = (EditBoxBase)obj;
                if (target.baseText != (string)args.NewValue)    //avoid undo stack overflow
                    target.baseText = (string)args.NewValue;
            })
        );

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.Text != this.baseText && !(string.IsNullOrEmpty(this.Text) && string.IsNullOrEmpty(this.baseText)))
            {
                SetCurrentValue(TextProperty, baseText);
                RaisePropertyChanged("Text");
            }

            base.OnTextChanged(e);
        }

        /// <summary>
        /// Raises a property changed event
        /// </summary>
        /// <param name="property">The name of the property that updates</param>
        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void textView_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = this.TextArea.TextView.GetPositionFloor(e.GetPosition(this.TextArea.TextView) + this.TextArea.TextView.ScrollOffset);
            bool inDocument = pos.HasValue;
            if (inDocument)
            {
                TextLocation logicalPosition = pos.Value.Location;
                int offset = this.Document.GetOffset(logicalPosition);

                var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
                TextMarkerService.TextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (toolTip == null)
                    {
                        toolTip = new ToolTip();
                        toolTip.Closed += ToolTipClosed;
                        toolTip.PlacementTarget = this;
                        toolTip.Content = new TextBlock
                        {
                            Text = markerWithToolTip.ToolTip,
                            TextWrapping = TextWrapping.Wrap
                        };
                        toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        private void ToolTipClosed(object sender, RoutedEventArgs e)
        {
            toolTip = null;
        }

        private void textView_TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void textView_VisualLinesChanged(object sender, EventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
        }

        private void EditBoxBase_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DataGrid dg = this.FindParent<DataGrid>(this);

            if (dg != null)
            {
                try
                {
                    dg.BeginEdit();
                }
                catch
                {
                }
            }
        }
    }
}
