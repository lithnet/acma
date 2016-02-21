using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.Acma;

namespace Lithnet.Acma.TestEngine
{
    public class UnitTestOutcome
    {
        public IList<string> MissingConstructors { get; set; }

        public UnitTest Test { get; set; }

        public UnitTestResult Result { get; set; }

        public Exception Exception { get; set; }

        public string AdditionalFailureInformation { get; set; }

        public int FailureStepNumber { get; set; }

        public string FailureStepName { get; set; }

        public string Description { 
            get
            {
                switch (this.Result)
                {
                    case UnitTestResult.Passed:
                        return string.Empty;

                    case UnitTestResult.Inconclusive:
                         return string.Format("The test was inconclusive\n{0}", this.AdditionalFailureInformation);

                    case UnitTestResult.Failed:
                         return string.Format("Evaluation failed on step {0}: {1}\n{2}", this.FailureStepNumber, this.FailureStepName, this.AdditionalFailureInformation);
                    
                    case UnitTestResult.Error:
                         return string.Format("Unexpected exception during step {0}: {1}. {2}\n{3}", this.FailureStepNumber, this.FailureStepName, this.Exception == null ? string.Empty : this.Exception.Message, this.AdditionalFailureInformation);
             
                    default:
                        return string.Empty;
                }
            }
        }

    }
}
