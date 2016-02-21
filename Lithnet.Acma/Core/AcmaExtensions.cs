using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma
{
    public static class AcmaExtensions
    {
        
        //public static object Execute(this Transform transform, AttributeValue attributeValue)
        //{
        //    return transform.Execute(attributeValue.Attribute.Type, attributeValue.Value);
        //}

        //public static object Execute(this Transform transform, IEnumerable<AttributeValue> attributeValues)
        //{
        //    if (attributeValues.Count() == 0)
        //    {
        //        return new List<object>();
        //    }

        //    IEnumerable<object> values = attributeValues.Select(t => t.Value);
        //    AttributeType type = attributeValues.First().Attribute.Type;

        //    return transform.Execute(type, values);
        //}
    }
}
