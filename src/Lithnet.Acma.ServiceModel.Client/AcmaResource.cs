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

        public AcmaResource()
        {
        }

        internal AcmaResource(SerializationInfo info, StreamingContext context)
        {
            this.Attributes = new Dictionary<string, IList<string>>();

            foreach (SerializationEntry entry in info)
            {
                if (entry.Value == null)
                {
                    this.Attributes.Add(entry.Name, null);
                }
                else
                {
                    IEnumerable<string> entryValues = entry.Value as IEnumerable<string>;

                    if (entryValues != null)
                    {
                        this.Attributes.Add(entry.Name, entryValues.ToList());
                    }
                    else
                    {
                        this.Attributes.Add(entry.Name, new List<string>() { entry.Value.ToString() });
                    }
                }
            }

            if (this.Attributes.ContainsKey("objectClass"))
            {
                this.ObjectType = this.Attributes["objectClass"].First();
            }

            if (this.Attributes.ContainsKey("objectId"))
            {
                this.ObjectID = new Guid(this.Attributes["objectId"].First());
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, IList<string>> kvp in this.Attributes)
            {
                if (kvp.Value == null)
                {
                    info.AddValue(kvp.Key, null, typeof(object));
                }
                else if (kvp.Value.Count > 1)
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