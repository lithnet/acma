namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines an attribute that indicates that a class derived from Transform handles its own multivalued input processing
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class HandlesOwnMultivaluedInputAttribute : Attribute
    {
    }
}