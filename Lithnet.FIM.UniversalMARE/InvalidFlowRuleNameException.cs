using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Fim.UniversalMARE
{
    public class InvalidFlowRuleNameException : Exception
    {
        public InvalidFlowRuleNameException()
            : base(@"The flow rule name was invalid. 
Flow rule names must be in one of the following formats
Import:
    CSAttribute1>>TransformName>>MVAttribute
    CSAttribute1+CSAttribute2>>TransformName>>MVAttribute
    CSAttribute1>>TransformName1>>TransformName2>>MVAttribute
    CSAttribute1+CSAttribute2>>TransformName1>>TransformName2>>MVAttribute

Export:
    CSAttribute<<TransformName<<MVAttribute1
    CSAttribute<<TransformName<<MVAttribute1+MVAttribute2
    CSAttribute<<TransformName2<<TransformName1<<MVAttribute1
    CSAttribute<<TransformName2<<TransformName1<<MVAttribute1+MVAttribute2")
        {
        }

        public InvalidFlowRuleNameException(string message)
            : base(message)
        {
        }
    }
}
