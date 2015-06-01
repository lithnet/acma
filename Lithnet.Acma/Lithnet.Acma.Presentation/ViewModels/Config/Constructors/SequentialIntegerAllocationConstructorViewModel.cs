using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class SequentialIntegerAllocationConstructorViewModel : AttributeConstructorViewModel
    {
        SequentialIntegerAllocationConstructor typedModel;

        public SequentialIntegerAllocationConstructorViewModel(SequentialIntegerAllocationConstructor model)
            : base(model)
        {
            this.typedModel = model;
        }

        public AcmaSequence Sequence
        {
            get
            {
                return this.typedModel.Sequence;
            }
            set
            {
                this.typedModel.Sequence = value;
            }
        }

        public IEnumerable<AcmaSequence> Sequences
        {
            get
            {
                return ActiveConfig.DB.Sequences;
            }
        }

        public override IEnumerable<AcmaSchemaAttribute> AllowedAttributes
        {
            get
            {
                return this.Model.ObjectClass.Attributes.Where(t => !t.IsReadOnlyInClass(this.Model.ObjectClass) && t.Type == ExtendedAttributeType.Integer).OrderBy(t => t.Name);
            }
        }
    }
}
