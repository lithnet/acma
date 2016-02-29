using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lithnet.Acma.ServiceModel
{
    [Serializable]
    [KnownType(typeof(List<string>))]
    public class AcmaResource : ISerializable
    {
        public string ObjectType { get; set; }

        public Guid ObjectID { get; set; }

        public Dictionary<string, IList<string>> Attributes { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, IList<string>> kvp in this.Attributes)
            {
                if (kvp.Value.Count > 1)
                {
                    info.AddValue(kvp.Key, kvp.Value, typeof(List<string>));
                }
                else if (kvp.Value.Count == 1)
                {
                    info.AddValue(kvp.Key, kvp.Value.First(), typeof(object));
                }
                else
                {
                    continue;
                }
            }
        }
    }
}