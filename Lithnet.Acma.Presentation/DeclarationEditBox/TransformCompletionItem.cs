using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Lithnet.Acma.DataModel;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using Lithnet.Transforms;
using Lithnet.Common.ObjectModel;
using System.Windows.Media.Imaging;

namespace Lithnet.Acma.Presentation
{
    public class TransformCompletionItem : ICompletionData
    {
        private Transform transform;

        public TransformCompletionItem(Transform transform)
        {
            this.transform = transform;
        }

        public System.Windows.Media.ImageSource Image
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Transforms.Presentation;component/Resources/transform.png", UriKind.Absolute)); ;
            }
        }

        public string Text
        {
            get
            {
                return this.transform.ID;
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
                    return this.transform.GetType().GetTypeDescription();
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
