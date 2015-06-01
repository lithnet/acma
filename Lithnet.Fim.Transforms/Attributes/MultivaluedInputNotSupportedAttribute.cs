namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines an attribute that indicates that a class derived from Transform does not allow multi-valued inputs
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MultivaluedInputNotSupportedAttribute : Attribute
    {
    }
}