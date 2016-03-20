using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.Acma;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "unit-test-step", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [KnownType(typeof(UnitTestStepObjectModification))]
    [KnownType(typeof(UnitTestStepObjectEvaluation))]
    [KnownType(typeof(UnitTestStepObjectCreation))]
    public abstract class UnitTestStep : UINotifyPropertyChanges, IExtensibleDataObject
    {
        public abstract void Execute();
        public abstract void Cleanup();

        [DataMember(Name = "test-name")]
        public abstract string Name { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }

        public UnitTest ParentTest { get; set; }
    }
}
