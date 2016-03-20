using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lithnet.Acma;
using Lithnet.Logging;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Lithnet.Common.ObjectModel;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "unit-test-group", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class UnitTestGroup : UnitTestObject
    {
        public delegate void UnitTestGroupStartEventHandler(object sender);

        public delegate void UnitTestGroupCompleteEventHandler(object sender, IList<UnitTestOutcome> outcome);

        public static event UnitTestGroupStartEventHandler UnitTestGroupStartingEvent;

        public static event UnitTestGroupCompleteEventHandler UnitTestGroupCompletedEvent;

        private List<UnitTestOutcome> UnitTestResults { get; set; }

        [DataMember(Name = "unit-test-objects")]
        public List<UnitTestObject> UnitTestObjects { get; set; }

        public UnitTestGroup()
        {
            this.Initialize();
        }

        public IList<UnitTestOutcome> Execute()
        {
            this.RaiseStartEvent();

            foreach (UnitTestObject item in this.UnitTestObjects)
            {
                if (item is UnitTest)
                {
                    this.UnitTestResults.Add(((UnitTest)item).Execute());
                }
                else if (item is UnitTestGroup)
                {
                    this.UnitTestResults.AddRange(((UnitTestGroup)item).Execute());
                }
                else
                {
                    throw new InvalidOperationException("The unit test object type was unknown");
                }
            }

            this.RaiseCompleteEvent(this.UnitTestResults);
            return this.UnitTestResults;
        }

        private void RaiseStartEvent()
        {
            if (UnitTestGroupStartingEvent != null)
            {
                UnitTestGroupStartingEvent(this);
            }
        }

        private void RaiseCompleteEvent(IList<UnitTestOutcome> outcomes)
        {
            if (UnitTestGroupCompletedEvent != null)
            {
                UnitTestGroupCompletedEvent(this, outcomes);
            }
        }

        private void Initialize()
        {
            this.UnitTestResults = new List<UnitTestOutcome>();
            this.UnitTestObjects = new List<UnitTestObject>();
        }
      
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
