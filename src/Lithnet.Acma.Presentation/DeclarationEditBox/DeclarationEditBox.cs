using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Lithnet.Acma.DataModel;
using Lithnet.MetadirectoryServices;
using System.Timers;
using Lithnet.Common.Presentation;
using Lithnet.Transforms;

namespace Lithnet.Acma.Presentation
{
    public class DeclarationEditBox : EditBoxBase
    {
        public static DependencyProperty ObjectClassProperty = DependencyProperty.Register(
           "ObjectClass",
           typeof(AcmaSchemaObjectClass),
           typeof(DeclarationEditBox),
           new FrameworkPropertyMetadata(
               default(AcmaSchemaObjectClass),
               new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static DependencyProperty DeclarationModeProperty = DependencyProperty.Register(
            "DeclarationMode",
            typeof(DeclarationEditBoxMode),
            typeof(DeclarationEditBox),
            new FrameworkPropertyMetadata(
                default(DeclarationEditBoxMode),
                new PropertyChangedCallback(OnDependencyPropertyChanged)));

        public static DependencyProperty SyntaxErrorsProperty = DependencyProperty.Register(
            "SyntaxErrors",
            typeof(IList<TokenError>),
            typeof(DeclarationEditBox));


        private static IList<ICompletionData> attributeCompletionData;

        private static IList<ICompletionData> variableCompletionData;

        private static IList<ICompletionData> transformCompletionData;

        private Timer syntaxHighlightingTimer;

        private IList<ICompletionData> scopedAttributeCompletionData;

        static DeclarationEditBox()
        {
            ActiveConfig.XmlConfigChanged += ActiveConfig_XmlConfigChanged;
            ActiveConfig.DatabaseConnectionChanged += ActiveConfig_DatabaseConnectionChanged;

            if (ActiveConfig.DB != null)
            {
                ActiveConfig.DB.AttributesBindingList.ListChanged += AttributesBindingList_ListChanged;
                ActiveConfig.DB.ConstantsBindingList.ListChanged += ConstantsBindingList_ListChanged;
            }

            if (ActiveConfig.XmlConfig != null)
            {
                ActiveConfig.XmlConfig.Transforms.CollectionChanged += Transforms_CollectionChanged;
            }
        }

        public DeclarationEditBox()
        {
            this.InitializeSyntaxHighlightingTimer();
            this.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            this.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            this.TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
            this.LostFocus += DeclarationEditBox_LostFocus;
            this.OnDeclarationModePropertyChanged();
            this.syntaxHighlightingTimer.Start();
            this.WordWrap = true;
        }
              
        public DeclarationEditBoxMode DeclarationMode
        {
            get
            {
                return (DeclarationEditBoxMode)this.GetValue(DeclarationModeProperty);

            }
            set
            {
                this.SetValue(DeclarationModeProperty, value);
            }
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return (AcmaSchemaObjectClass)this.GetValue(ObjectClassProperty);
            }
            set
            {
                this.SetValue(ObjectClassProperty, value);
            }
        }

        public IList<TokenError> SyntaxErrors
        {
            get
            {
                return (IList<TokenError>)this.GetValue(SyntaxErrorsProperty);
            }
            set
            {
                this.SetValue(SyntaxErrorsProperty, value);
            }
        }

        private IList<ICompletionData> ScopedAttributeCompletionData
        {
            get
            {
                if (this.scopedAttributeCompletionData == null)
                {
                    this.CreateScopedAttributeCompletionData();
                }

                return this.scopedAttributeCompletionData;
            }
        }

        private static IList<ICompletionData> AttributeCompletionData
        {
            get
            {
                if (DeclarationEditBox.attributeCompletionData == null)
                {
                    DeclarationEditBox.CreateAttributeCompletionData();
                }

                return DeclarationEditBox.attributeCompletionData;
            }
        }

        private static IList<ICompletionData> TransformCompletionData
        {
            get
            {
                if (DeclarationEditBox.transformCompletionData == null)
                {
                    DeclarationEditBox.CreateTransformCompletionData();
                }

                return DeclarationEditBox.transformCompletionData;
            }
        }

        private static IList<ICompletionData> VariableCompletionData
        {
            get
            {
                if (DeclarationEditBox.variableCompletionData == null)
                {
                    DeclarationEditBox.CreateVariableCompletionData();
                }

                return DeclarationEditBox.variableCompletionData;
            }
        }

        private bool IsInAttributeDeclaration { get; set; }

        private bool IsInVariableDeclaration { get; set; }

        private bool IsInTransformDeclaration { get; set; }

        private bool IsInReferenceChase { get; set; }

        private bool IsStartingAttributeDeclaration { get; set; }

        private bool IsStartingVariableDeclaration { get; set; }

        private bool IsStartingReferenceChase { get; set; }

        private bool IsStartingTransform { get; set; }

        private static void OnDependencyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DeclarationEditBox obj = sender as DeclarationEditBox;

            if (obj != null)
            {
                switch (e.Property.Name)
                {
                    case "DeclarationMode":
                        obj.OnDeclarationModePropertyChanged();
                        break;

                    case "ObjectClass":
                        obj.OnObjectClassPropertyChanged(e.OldValue, e.NewValue);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void ConstantsBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            DeclarationEditBox.variableCompletionData = null;
        }

        private static void Transforms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DeclarationEditBox.transformCompletionData = null;
        }

        private static void AttributesBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            DeclarationEditBox.attributeCompletionData = null;
        }

        private static void ActiveConfig_DatabaseConnectionChanged(object sender, EventArgs e)
        {
            if (ActiveConfig.DB != null)
            {
                ActiveConfig.DB.AttributesBindingList.ListChanged += AttributesBindingList_ListChanged;
                ActiveConfig.DB.ConstantsBindingList.ListChanged += ConstantsBindingList_ListChanged;
            }

            DeclarationEditBox.attributeCompletionData = null;
            DeclarationEditBox.variableCompletionData = null;
        }

        private static void ActiveConfig_XmlConfigChanged(object sender, EventArgs e)
        {
            if (ActiveConfig.XmlConfig != null)
            {
                ActiveConfig.XmlConfig.Transforms.CollectionChanged += Transforms_CollectionChanged;
            }

            DeclarationEditBox.transformCompletionData = null;
        }

        private static void CreateAttributeCompletionData()
        {
            DeclarationEditBox.attributeCompletionData = new List<ICompletionData>();

            foreach (AcmaSchemaAttribute attribute in ActiveConfig.DB.Attributes)
            {
                AttributeCompletionItem item = new AttributeCompletionItem(attribute);
                DeclarationEditBox.attributeCompletionData.Add(item);
            }
        }

        private static void CreateVariableCompletionData()
        {
            DeclarationEditBox.variableCompletionData = new List<ICompletionData>();

            foreach (string variable in VariableDeclaration.GetVariableNames())
            {
                VariableCompletionItem item = new VariableCompletionItem(variable);
                DeclarationEditBox.variableCompletionData.Add(item);
            }
        }

        private static void CreateTransformCompletionData()
        {
            DeclarationEditBox.transformCompletionData = new List<ICompletionData>();

            foreach (Transform transform in ActiveConfig.XmlConfig.Transforms)
            {
                TransformCompletionItem item = new TransformCompletionItem(transform);
                DeclarationEditBox.transformCompletionData.Add(item);
            }
        }

        private void CreateScopedAttributeCompletionData()
        {
            this.scopedAttributeCompletionData = new List<ICompletionData>();

            IEnumerable<AcmaSchemaAttribute> source;

            if (this.ObjectClass == null)
            {
                source = ActiveConfig.DB.Attributes;
            }
            else
            {
                source = this.ObjectClass.Attributes;
            }

            foreach (AcmaSchemaAttribute attribute in source)
            {
                AttributeCompletionItem item = new AttributeCompletionItem(attribute);
                this.scopedAttributeCompletionData.Add(item);
            }
        }

        private void InitializeSyntaxHighlightingTimer()
        {
            this.syntaxHighlightingTimer = new Timer();
            this.syntaxHighlightingTimer.Interval = 1200;
            this.syntaxHighlightingTimer.AutoReset = false;
            this.syntaxHighlightingTimer.Elapsed += timer_Elapsed;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invoke(() =>
            {
                this.ValidateDeclaration();
            });
        }

        private void OnObjectClassPropertyChanged(object oldValue, object newValue)
        {
            AcmaSchemaObjectClass oldClass = oldValue as AcmaSchemaObjectClass;
            AcmaSchemaObjectClass newClass = newValue as AcmaSchemaObjectClass;

            if (oldClass != null)
            {
                oldClass.Mappings.ListChanged -= Mappings_ListChanged;
            }

            if (newClass != null)
            {
                newClass.Mappings.ListChanged += Mappings_ListChanged;
            }
        }

        private void Mappings_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.scopedAttributeCompletionData = null;
        }

        private void OnDeclarationModePropertyChanged()
        {
            Uri uri;

            if (this.DeclarationMode == DeclarationEditBoxMode.NoAttributeDeclarations)
            {
                uri = new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/AcmaDLNoAttributes.xshd", UriKind.Absolute);
            }
            else
            {
                uri = new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/AcmaDL.xshd", UriKind.Absolute);
            }

            using (Stream s = Application.GetResourceStream(uri).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    this.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        private void DeclarationEditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.syntaxHighlightingTimer.Stop();
            this.ValidateDeclaration();
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            this.syntaxHighlightingTimer.Stop();
            this.syntaxHighlightingTimer.Start();
            this.ParseDeclarationOnTextChanged();

            if (this.completionWindow != null)
            {
                return;
            }

            if (this.IsStartingTransform || this.IsInTransformDeclaration)
            {
                this.ShowTransformCompletionWindow();
            }
            else if (this.IsStartingReferenceChase || this.IsInReferenceChase)
            {
                this.ShowAttributeCompletionWindow(true);
            }
            else if (this.IsInVariableDeclaration || this.IsStartingVariableDeclaration)
            {
                this.ShowVariableCompletionWindow();
            }
            else if (this.IsStartingAttributeDeclaration || this.IsInAttributeDeclaration)
            {
                this.ShowAttributeCompletionWindow(false);
            }
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
        }

        private void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    int carretPos = this.CaretOffset;
                    this.Text = this.Text.Insert(carretPos, Environment.NewLine);
                    this.CaretOffset = carretPos + 2 >= this.Text.Length ? this.Text.Length : carretPos + 2;
                }
            }
        }

        private void ShowAttributeCompletionWindow(bool showAll)
        {
            if (showAll || this.ScopedAttributeCompletionData.Count == 0)
            {
                ShowAllAttributeCompletionWindow();
            }
            else
            {
                ShowScopedAttributeCompletionWindow();
            }
        }

        private void ShowScopedAttributeCompletionWindow()
        {
            if (this.DeclarationMode != DeclarationEditBoxMode.NoAttributeDeclarations)
            {
                this.completionWindow = new CompletionWindow(this.TextArea);
                this.TransferCompletionData(this.ScopedAttributeCompletionData, this.completionWindow.CompletionList.CompletionData);

                this.completionWindow.Show();
                this.completionWindow.Closed += delegate
                {
                    this.completionWindow = null;
                };
            }
        }

        private void ShowAllAttributeCompletionWindow()
        {
            if (this.DeclarationMode != DeclarationEditBoxMode.NoAttributeDeclarations)
            {
                this.completionWindow = new CompletionWindow(this.TextArea);
                this.TransferCompletionData(DeclarationEditBox.AttributeCompletionData, this.completionWindow.CompletionList.CompletionData);

                this.completionWindow.Show();
                this.completionWindow.Closed += delegate
                {
                    this.completionWindow = null;
                };
            }
        }

        private void ShowVariableCompletionWindow()
        {
            this.completionWindow = new CompletionWindow(this.TextArea);
            this.TransferCompletionData(DeclarationEditBox.VariableCompletionData, this.completionWindow.CompletionList.CompletionData);
            this.completionWindow.Show();
            this.completionWindow.Closed += delegate
            {
                this.completionWindow = null;
            };
        }

        private void ShowTransformCompletionWindow()
        {
            this.completionWindow = new CompletionWindow(this.TextArea);
            this.TransferCompletionData(DeclarationEditBox.TransformCompletionData, this.completionWindow.CompletionList.CompletionData);
            this.completionWindow.Show();
            this.completionWindow.Closed += delegate
            {
                this.completionWindow = null;
            };
        }

        private void ValidateDeclaration()
        {
            if (this.textMarkerService == null)
            {
                return;
            }

            this.textMarkerService.Clear();

            ValueDeclarationParser p = new ValueDeclarationParser(this.baseText);

            foreach (TokenError error in p.Errors)
            {
                this.DisplayValidationError(error.Description, error.ColumnNumber, error.LineNumber);
            }

            this.SyntaxErrors = p.Errors;
        }

        private void ParseDeclarationOnTextChanged()
        {
            StringTokenizer t = new StringTokenizer(this.baseText);

            bool isInAttributeDeclaration = false;

            bool isInVariableDeclaration = false;

            bool isInTransformDeclaration = false;

            bool isInReferenceChase = false;

            int pos = this.CaretOffset;

            this.IsStartingAttributeDeclaration = false;
            this.IsStartingReferenceChase = false;
            this.IsStartingTransform = false;
            this.IsStartingVariableDeclaration = false;

            foreach (Token token in t.Tokens)
            {
                if (token.EndPosition == pos)
                {
                    if (token.Kind == TokenKind.Symbol)
                    {
                        this.IsStartingAttributeDeclaration = false;
                        this.IsStartingVariableDeclaration = false;
                        this.IsStartingTransform = false;
                        this.IsStartingReferenceChase = false;
                        isInTransformDeclaration = false;
                        isInAttributeDeclaration = false;
                        isInReferenceChase = false;
                        isInVariableDeclaration = false;
                        break;
                    }
                    else if (token.Kind == TokenKind.OpenBrace)
                    {
                        this.IsStartingAttributeDeclaration = true;
                    }
                    else if (token.Kind == TokenKind.PercentSign && !isInVariableDeclaration)
                    {
                        this.IsStartingVariableDeclaration = true;
                    }
                    else if (token.Kind == TokenKind.TransformOperator)
                    {
                        this.IsStartingTransform = true;
                    }
                    else if (token.Kind == TokenKind.ReferenceOperator)
                    {
                        this.IsStartingReferenceChase = true;
                    }
                }
                else if (token.Position > pos)
                {
                    continue;
                }

                if (token.Kind == TokenKind.OpenBrace)
                {
                    isInAttributeDeclaration = true;
                    isInVariableDeclaration = false;
                    isInTransformDeclaration = false;
                    isInReferenceChase = false;
                }
                else if (token.Kind == TokenKind.ClosedBrace)
                {
                    isInTransformDeclaration = false;
                    isInAttributeDeclaration = false;
                    isInReferenceChase = false;
                    isInVariableDeclaration = false;
                }
                else if (token.Kind == TokenKind.PercentSign && !isInVariableDeclaration)
                {
                    isInVariableDeclaration = true;
                    isInAttributeDeclaration = false;
                    isInReferenceChase = false;
                    isInTransformDeclaration = false;
                }
                else if (token.Kind == TokenKind.PercentSign && isInVariableDeclaration)
                {
                    isInVariableDeclaration = false;
                    isInTransformDeclaration = false;
                    isInAttributeDeclaration = false;
                    isInReferenceChase = false;
                }
                else if (token.Kind == TokenKind.TransformOperator)
                {
                    isInTransformDeclaration = true;
                    isInReferenceChase = false;
                    isInAttributeDeclaration = false;
                    isInVariableDeclaration = false;
                }
                else if (token.Kind == TokenKind.ReferenceOperator)
                {
                    isInAttributeDeclaration = false;
                    isInTransformDeclaration = false;
                    isInVariableDeclaration = false;
                    isInReferenceChase = true;
                }
            }

            this.IsInAttributeDeclaration = isInAttributeDeclaration;
            this.IsInVariableDeclaration = isInVariableDeclaration;
            this.IsInReferenceChase = isInReferenceChase;
            this.IsInTransformDeclaration = isInTransformDeclaration;
        }
    }
}
