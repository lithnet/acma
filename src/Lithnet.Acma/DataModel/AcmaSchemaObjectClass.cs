using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.DataModel
{
    [DataContract(Name = "object-class", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Serializable]
    public partial class AcmaSchemaObjectClass : IObjectReference, IExtensibleDataObject
    {
        private string serializationName;

        private string columnListforSelectQuery;

        private List<AcmaSchemaMapping> inheritedAttributes;

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

        private IBindingList mappingsBindingList;

        private IBindingList shadowLinksBindingList;

        private IBindingList referenceLinksBindingList;

        private List<AcmaSchemaAttribute> avpAttributes;

        public ExtensionDataObject ExtensionData { get; set; }

        public bool IsShadowObject
        {
            get
            {
                return this.ShadowFromObjectClass != null;
            }
        }

        public IEnumerable<AcmaSchemaMapping> InheritedAttributes
        {
            get
            {
                if (this.inheritedAttributes == null)
                {
                    this.inheritedAttributes = this.Mappings.Where(t => t.InheritanceSourceAttribute != null).ToList();
                }

                return this.inheritedAttributes;
            }
        }

        public IEnumerable<AcmaSchemaAttribute> AvpAttributes
        {
            get
            {
                if (this.avpAttributes == null)
                {
                    this.avpAttributes = this.Attributes.Where(t => t.IsInAVPTable).ToList();
                }

                return this.avpAttributes;
            }
        }

        public IEnumerable<AcmaSchemaAttribute> Attributes
        {
            get
            {
                return this.Mappings.Select(t => t.Attribute);
            }
        }

        public IEnumerable<AcmaSchemaAttribute> BindableAttributes
        {
            get
            {
                return ActiveConfig.DB.Attributes.Except(this.Attributes).Where(t => !t.IsBuiltIn);
            }
        }

        public IBindingList MappingsBindingList
        {
            get
            {

                if (this.mappingsBindingList == null)
                {
                    this.mappingsBindingList = this.Mappings.GetNewBindingList();
                }

                return this.mappingsBindingList;
            }
        }

        public IBindingList ShadowLinksBindingList
        {
            get
            {

                if (this.shadowLinksBindingList == null)
                {
                    this.shadowLinksBindingList = this.ShadowLinks.GetNewBindingList();
                }

                return this.shadowLinksBindingList;
            }
        }

        public IBindingList ReferenceLinksBindingList
        {
            get
            {
                if (this.referenceLinksBindingList == null)
                {
                    this.referenceLinksBindingList = this.ForwardLinks.GetNewBindingList();
                }

                return this.referenceLinksBindingList;
            }
        }

        public bool IsShadowParent
        {
            get
            {
                return this.ShadowChildren.Any();
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public ValueDeclaration ObjectDisplayDeclaration { get; private set; }

        public void ThrowOnNoSuchAttribute(AcmaSchemaAttribute attribute)
        {
            if (!this.Attributes.Contains(attribute))
            {
                throw new NoSuchAttributeInObjectTypeException(attribute.Name);
            }
        }

        public bool HasAttribute(string name)
        {
            AcmaSchemaAttribute attribute = this.Attributes.FirstOrDefault(t => t.Name == name);

            if (attribute == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public AcmaSchemaAttribute GetAttribute(string name)
        {
            AcmaSchemaAttribute attribute = this.Attributes.FirstOrDefault(t => t.Name == name);

            if (attribute == null)
            {
                throw new NoSuchAttributeInObjectTypeException(attribute.Name);
            }

            return attribute;
        }

        public string ColumnListForSelectQuery
        {
            get
            {
                if (this.columnListforSelectQuery == null)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine("ID");
                    builder.AppendLine(",inheritedUpdate");
                    builder.AppendLine(",rowversion");

                    foreach (AcmaSchemaAttribute attribute in this.Attributes.Where(t => !t.IsInAVPTable && !t.IsInheritedInClass(this)))
                    {
                        builder.AppendLine("," + attribute.ColumnName);
                    }

                    this.columnListforSelectQuery = builder.ToString();

                }

                return this.columnListforSelectQuery;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is AcmaSchemaObjectClass)
            {
                AcmaSchemaObjectClass other = (AcmaSchemaObjectClass)obj;

                if (this.ID == other.ID)
                {
                    return true;
                }
            }

            return base.Equals(obj);
        }

        public static bool operator ==(AcmaSchemaObjectClass a, AcmaSchemaObjectClass b)
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

            return a.ID == b.ID;
        }

        public static bool operator !=(AcmaSchemaObjectClass a, AcmaSchemaObjectClass b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public object GetRealObject(StreamingContext context)
        {
            return ActiveConfig.DB.GetObjectClass(this.serializationName);
        }
    }
}
