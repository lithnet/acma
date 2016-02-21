using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reeb.SqlOM;
using Microsoft.MetadirectoryServices;
using System.ComponentModel;
using System.Runtime.Serialization;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.DataModel
{
    [DataContract(Name = "attribute", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Serializable]
    public partial class AcmaSchemaAttribute : IObjectReference, IExtensibleDataObject
    {
        private string serializationName;

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

        static AcmaSchemaAttribute()
        {
            fromTermTableCache = new Dictionary<string, FromTerm>();
        }

        private static Dictionary<string, FromTerm> fromTermTableCache;

        public ExtensionDataObject ExtensionData { get; set; }

        public FromTerm DBTable
        {
            get
            {
                if (!AcmaSchemaAttribute.fromTermTableCache.ContainsKey(this.TableName))
                {
                    AcmaSchemaAttribute.fromTermTableCache.Add(this.TableName, FromTerm.Table(this.TableName, this.TableName, "[dbo]"));
                }

                return AcmaSchemaAttribute.fromTermTableCache[this.TableName];
            }
        }

        public bool IsInheritedInClass(string objectClass)
        {
            return (this.Mappings.Any(t => t.ObjectClass.Name == objectClass && t.InheritanceSourceAttribute != null));
        }

        public bool IsInheritedInClass(AcmaSchemaObjectClass objectClass)
        {
            return (this.Mappings.Any(t => t.ObjectClass == objectClass && t.InheritanceSourceAttribute != null));
        }

        public bool IsReadOnlyInClass(AcmaSchemaObjectClass objectClass)
        {
            return this.Mappings.Any(t => t.ObjectClass == objectClass && t.InheritanceSourceAttribute != null) ||
                this.ShadowObjectReferenceLinks.Any(t => t.ParentObjectClass == objectClass) ||
                this.BackLinks.Any(t => t.BackLinkObjectClass == objectClass) ||
                this.Name == "objectId" ||
                this.Name == "objectClass" ||
                this.Name == "shadowParent" ||
                this.Name == "shadowLink";

        }

        public AttributeType MmsType
        {
            get
            {
                return this.Type.ToAttributeType();
            }
        }

        public bool IsInheritanceSourceCandidate
        {
            get
            {
                if (this.Type == ExtendedAttributeType.Reference)
                {
                    if (this.Operation == AcmaAttributeOperation.AcmaInternalTemp)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsInheritable
        {
            get
            {
                if (this.Operation == AcmaAttributeOperation.AcmaInternalTemp)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool CanInherit
        {
            get
            {
                if (this.IsBuiltIn)
                {
                    return false;
                }
               
                if (this.Operation == AcmaAttributeOperation.AcmaInternalTemp)
                {
                    return false;
                }

                return true;
            }
        }

        public bool CanInheritFrom(AcmaSchemaAttribute inheritFromAttribute)
        {
            if (!this.CanInherit)
            {
                return false;
            }

            if (this.Type != inheritFromAttribute.Type)
            {
                return false;
            }

            if (this.IsMultivalued != inheritFromAttribute.IsMultivalued)
            {
                return false;
            }

            return true;
        }

        public bool IsInAVPTable
        {
            get
            {
                return this.TableName != "MA_Objects";
            }
        }

        public bool IsConstructable(AcmaSchemaObjectClass objectClass)
        {
            if (!objectClass.Attributes.Contains(this))
            {
                return false;
            }

            if (objectClass.BackLinks.Any(t => t.BackLinkAttribute == this))
            {
                return false;
            }

            if (objectClass.ShadowLinks.Any(t => t.ReferenceAttribute == this))
            {
                return false;
            }


            return true;
        }

        public bool IsValidBackLink(AcmaSchemaObjectClass backLinkObjectClass)
        {
            if (this.Type != ExtendedAttributeType.Reference)
            {
                return false;
            }

            if (this.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return false;
            }

            if (this.IsBuiltIn)
            {
                return false;
            }

            if (this.IsInheritedInClass(backLinkObjectClass))
            {
                return false;
            }

            if (!backLinkObjectClass.Attributes.Any(t => t == this))
            {
                return false;
            }

            if (backLinkObjectClass.BackLinks.Any(t => t.BackLinkAttribute == this))
            {
                return false;
            }

            return true;
        }

        public bool IsValidForwardLink(AcmaSchemaObjectClass forwardLinkObjectClass)
        {
            if (this.Type != ExtendedAttributeType.Reference)
            {
                return false;
            }

            if (this.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return false;
            }

            if (!forwardLinkObjectClass.Attributes.Any(t => t == this))
            {
                return false;
            }

            if (forwardLinkObjectClass.BackLinks.Any(t => t.BackLinkAttribute == this))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is AcmaSchemaAttribute)
            {
                AcmaSchemaAttribute otherAttribute = (AcmaSchemaAttribute)obj;

                if (this.ID == otherAttribute.ID)
                {
                    return true;
                }
            }

            return base.Equals(obj);
        }

        public static bool operator ==(AcmaSchemaAttribute a, AcmaSchemaAttribute b)
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

        public static bool operator !=(AcmaSchemaAttribute a, AcmaSchemaAttribute b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        private static Dictionary<string, AcmaSchemaAttribute> lostAndFound;

        public static void ClearLostFoundCache()
        {
            if (AcmaSchemaAttribute.lostAndFound != null)
            {
                AcmaSchemaAttribute.lostAndFound.Clear();
            }
        }

        public object GetRealObject(StreamingContext context)
        {
            return AcmaSchemaAttribute.FindAttributeOrSubstitute(this.serializationName);
        }

        public static AcmaSchemaAttribute FindAttributeOrSubstitute(string attributeName)
        {
            if (lostAndFound == null)
            {
                lostAndFound = new Dictionary<string, AcmaSchemaAttribute>();
            }

            if (lostAndFound.ContainsKey(attributeName))
            {
                return lostAndFound[attributeName];
            }

            AcmaSchemaAttribute attribute;

            try
            {
                attribute = ActiveConfig.DB.GetAttribute(attributeName);
            }
            catch (NoSuchAttributeException)
            {
                attribute = AcmaSchemaAttribute.GetSubstituteAttribute(attributeName);

                if (attribute == null)
                {
                    throw;
                }

                lostAndFound.Add(attributeName, attribute);
            }
            catch (NoSuchAttributeInObjectTypeException)
            {
                attribute = AcmaSchemaAttribute.GetSubstituteAttribute(attributeName);

                if (attribute == null)
                {
                    throw;
                }

                lostAndFound.Add(attributeName, attribute);
            }

            return attribute;
        }

        public delegate void MissingAttributeEventDelegate(MissingAttributeEventArgs args);

        public static event MissingAttributeEventDelegate MissingAttributeEvent;

        private static AcmaSchemaAttribute GetSubstituteAttribute(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                return null;
            }
            else if (AcmaSchemaAttribute.MissingAttributeEvent != null)
            {
                MissingAttributeEventArgs args = new MissingAttributeEventArgs(attributeName);
                AcmaSchemaAttribute.MissingAttributeEvent(args);

                return args.ReplacementAttribute;
            }

            return null;
        }
    }
}
