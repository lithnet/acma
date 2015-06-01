using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Common.Presentation;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Transforms;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class TransformTestViewModel : ViewModelBase<Transform>
    {
        Transform model;

        public TransformTestViewModel(Transform model)
            : base(model)
        {
            this.Commands.AddItem("Execute", t => this.Execute(), t => this.CanExecute());
            this.model = model;
            this.AttributeType = this.model.AllowedInputTypes.FirstOrDefault();
            this.model.PropertyChanged += model_PropertyChanged;
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "AllowedInputTypes")
            {
                if (!this.AllowedInputTypes.Any(t => t == this.AttributeType))
                {
                    this.AttributeType = this.AllowedInputTypes.FirstOrDefault();
                }
            }
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MultivalueBehaviour":
                    this.RaisePropertyChanged("IsMultivalueSupported");
                    break;

                case "AllowedInputTypes":
                    this.RaisePropertyChanged("AllowedInputTypes");
                    break;

                default:
                    break;
            }
        }

        private void Execute()
        {
            try
            {
                if (this.IsLoopbackModel)
                {
                    this.ExecuteLoopbackTransform();
                }
                else if (this.IsMultivalueSupported)
                {
                    this.ExecuteMultivaluedTransform();
                }
                else
                {
                    this.ExecuteSingleValuedTransform();
                }
            }
            catch (Exception ex)
            {
                this.OutputValue = string.Format("The transform reported an error: {0}", ex.Message);
            }

            this.RaisePropertyChanged("OutputValue");
        }

        private void ExecuteSingleValuedTransform()
        {
            this.OutputValue =  this.model.TransformValue(TypeConverter.ConvertData(this.InputValue1, this.AttributeType)).Select(t => t.ToSmartStringOrNull()).ToNewLineSeparatedString();
        }

        private void ExecuteMultivaluedTransform()
        {
            List<object> inputValues = new List<object>();

            if (!string.IsNullOrEmpty(this.InputValue1))
            {
                inputValues.Add(TypeConverter.ConvertData(this.InputValue1, this.AttributeType));
            }

            if (!string.IsNullOrEmpty(this.InputValue2))
            {
                inputValues.Add(TypeConverter.ConvertData(this.InputValue2, this.AttributeType));
            }

            if (!string.IsNullOrEmpty(this.InputValue3))
            {
                inputValues.Add(TypeConverter.ConvertData(this.InputValue3, this.AttributeType));
            }

            IList<object> output = this.model.TransformValue(inputValues);

            this.OutputValue = output.Select(t => t.ToSmartStringOrNull()).ToNewLineSeparatedString();
        }

        private void ExecuteLoopbackTransform()
        {
            List<object> inputValues;
            string targetValue;

            if (this.IsMultivalueSupported)
            {
                inputValues = new List<object>() 
                    { 
                        TypeConverter.ConvertData(this.InputValue1, this.AttributeType),
                        TypeConverter.ConvertData(this.InputValue2, this.AttributeType)     
                    };

                targetValue = this.InputValue3;
            }
            else
            {
                inputValues = new List<object>() { TypeConverter.ConvertData(this.InputValue1, this.AttributeType) };
                targetValue = this.InputValue2;
            }

            ExtendedAttributeType returnType = this.model.PossibleReturnTypes.First();
            object output = this.model.TransformValuesWithLoopback(
                inputValues,
                TypeConverter.ConvertData(targetValue, returnType));

            this.OutputValue = output.ToSmartStringOrNull();
        }

        private bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(this.InputValue1) ||
                !string.IsNullOrWhiteSpace(this.InputValue2) ||
                !string.IsNullOrWhiteSpace(this.InputValue3);
        }

        public bool IsMultivalueSupported
        {
            get
            {
                return this.model.SupportsMultivaluedInput;
            }
        }

        public string InputValue1 { get; set; }

        public string InputValue2 { get; set; }

        public bool IsInputValue2Visible
        {
            get
            {
                return this.IsMultivalueSupported || this.IsLoopbackModel;
            }
        }

        public string InputValue3 { get; set; }

        public bool IsInputValue3Visible
        {
            get
            {
                if (this.IsLoopbackModel)
                {
                    if (this.IsMultivalueSupported)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return this.IsMultivalueSupported;
                }
            }
        }

        public string InputValue1Text
        {
            get
            {
                if (this.IsMultivalueSupported)
                {
                    return "Input Value 1";
                }
                else
                {
                    return "Input Value";
                }
            }
        }

        public string InputValue2Text
        {
            get
            {
                if (this.IsLoopbackModel)
                {
                    if (this.IsMultivalueSupported)
                    {
                        return "Input Value 2";
                    }
                    else
                    {
                        return "Existing Target Value";
                    }
                }
                else if (this.IsMultivalueSupported)
                {
                    return "Input Value 2";
                }
                else
                {
                    return null;
                }
            }
        }

        public string InputValue3Text
        {
            get
            {
                if (this.IsLoopbackModel)
                {
                    if (this.IsMultivalueSupported)
                    {
                        return "Existing Target Value";
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (this.IsMultivalueSupported)
                {
                    return "Input Value 3";
                }
                else
                {
                    return null;
                }
            }
        }

        public ExtendedAttributeType AttributeType { get; set; }

        public bool IsLoopbackModel
        {
            get
            {
                return this.model.ImplementsLoopbackProcessing;
            }
        }

        public IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                return this.model.AllowedInputTypes;
            }
        }

        public string OutputValue { get; set; }
    }
}
