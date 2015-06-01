using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
namespace Lithnet.Acma.DataModel
{
    [DataContract(Name = "sequence", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Serializable]
    public partial class AcmaSequence: IObjectReference, IExtensibleDataObject
    {
        private string serializationName;

        public long GetNextValue()
        {
            return ActiveConfig.DB.GetNextSequenceValue(this.Name);
        }
        
        public ExtensionDataObject ExtensionData { get; set; }

        [DataMember(Name = "name")]
        private string SerializationName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.serializationName = value;
            }
        }

        public object GetRealObject(StreamingContext context)
        {
            return ActiveConfig.DB.GetSequence(this.serializationName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is AcmaSequence)
            {
                AcmaSequence otherAttribute = (AcmaSequence)obj;

                if (this.Name == otherAttribute.Name)
                {
                    return true;
                }
            }

            return base.Equals(obj);
        }

        public static bool operator ==(AcmaSequence a, AcmaSequence b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Name == b.Name;
        }

        public static bool operator !=(AcmaSequence a, AcmaSequence b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
