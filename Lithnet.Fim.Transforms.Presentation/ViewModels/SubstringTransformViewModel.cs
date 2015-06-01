using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class SubstringTransformViewModel : TransformViewModel
    {
        private SubstringTransform model;

        public SubstringTransformViewModel(SubstringTransform model)
            : base(model)
        {
            this.model = model;
            this.model.PropertyChanged += model_PropertyChanged;
            this.ToolTips.AddItem("Length", "Specifies the number of characters to obtain starting from the left-hand side of the string");
            this.ToolTips.AddItem("PadCharacter", "When 'Padding Type' is set to 'SpecifiedValue', and the string is shorter than the specified length, this character is repeated at the end of the string to meet the length requirements specified");
            this.ToolTips.AddItem("PaddingType", "Specifies the action to take when the string is shorter than the specified number of characters. The first, last or character you specify can be appended to the string to make it the required length");
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PaddingType")
            {
                this.RaisePropertyChanged("PadCharacterFieldEnabled");
            }
        }

        public int Length
        {
            get
            {
                return this.model.Length;
            }
            set
            {
                this.model.Length = value;
            }
        }

        public Direction Direction
        {
            get
            {
                return this.model.Direction;
            }
            set
            {
                this.model.Direction = value;
            }
        }

        public string PadCharacter
        {
            get
            {
                return this.model.PadCharacter;
            }
            set
            {
                this.model.PadCharacter = value;
            }
        }

        public int StartIndex
        {
            get
            {
                return this.model.StartIndex;
            }
            set
            {
                this.model.StartIndex = value;
            }
        }

        public bool PadCharacterFieldEnabled
        {
            get
            {
                return this.model.PaddingType == PadType.SpecifiedValue;
            }
        }

        public PadType PaddingType
        {
            get
            {
                return this.model.PaddingType;
            }
            set
            {
                this.model.PaddingType = value;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.SubstringTransformDescription;
            }
        }
    }
}
