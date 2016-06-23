using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections;
using System.Management.Automation;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.PS
{
    public class AcmaObjectAttributeCollection
    {
        private MAObjectHologram hologram;

        internal AcmaObjectAttributeCollection(MAObjectHologram hologram)
        {
            if (hologram == null)
            {
                throw new ArgumentNullException("hologram");
            }

            this.hologram = hologram;
        }

        public void Delete(string name)
        {
            AcmaSchemaAttribute attribute = this.hologram.ObjectClass.GetAttribute(name);

            if (this.hologram.AcmaModificationType == TriggerEvents.Unconfigured)
            {
                this.hologram.SetObjectModificationType(TriggerEvents.Update);
            }

            this.hologram.SetAttributeValue(attribute, null);
        }

        public object[] this [string name]
        {
            get
            {
                AcmaSchemaAttribute attribute = this.hologram.ObjectClass.GetAttribute(name);

                if (attribute.IsMultivalued)
                {
                    if (attribute.Type == ExtendedAttributeType.Reference)
                    {
                        List<AcmaPSObject> objects = new List<AcmaPSObject>();
                        foreach(var value in this.hologram.GetMVAttributeValues(attribute).Values)
                        {
                            MAObjectHologram maObject = ActiveConfig.DB.GetMAObjectOrDefault(value.ValueGuid);
                            if (maObject != null)
                            {
                                objects.Add(new AcmaPSObject(maObject));
                            }
                        }

                        return objects.ToArray();
                    }
                    else
                    {
                        return this.hologram.GetMVAttributeValues(attribute).Values.Select(t => t.Value).ToArray();
                    }
                }
                else
                {
                    List<object> list = new List<object>();
                    AttributeValue value = this.hologram.GetSVAttributeValue(attribute);

                    if (attribute.Type == ExtendedAttributeType.Reference)
                    {
                        MAObjectHologram maObject = ActiveConfig.DB.GetMAObjectOrDefault(value.ValueGuid);
                        if (maObject != null)
                        {
                            list.Add(new AcmaPSObject(maObject));
                        }
                    }
                    else
                    {
                        list.Add(value.Value);
                    }
                     
                    return list.ToArray();
                }
            }
            set
            {
                AcmaSchemaAttribute attribute = this.hologram.ObjectClass.GetAttribute(name);

                if (this.hologram.AcmaModificationType == TriggerEvents.Unconfigured)
                {
                    this.hologram.SetObjectModificationType(TriggerEvents.Update);
                }

                if (value == null)
                {
                    this.hologram.SetAttributeValue(attribute, null);
                    return;
                }

                List<object> values = new List<object>();

                if (attribute.Type == ExtendedAttributeType.Reference)
                {
                    if (value.Count() > 0)
                    {
                        if (value.First() is string)
                        {
                            if (value.Any(t => !(t is string)))
                            {
                                throw new InvalidOperationException("The values of a reference attribute must be a list of GUIDs or an AcmaPSObject");
                            }

                            values.AddRange(value);
                        }
                        else if (value.First() is Guid)
                        {
                            if (value.Any(t => !(t is Guid)))
                            {
                                throw new InvalidOperationException("The values of a reference attribute must be a list of GUIDs or an AcmaPSObject");
                            }

                            values.AddRange(value);
                        }
                        else if (value.First() is PSObject)
                        {
                            if (value.Any(t => !(t is PSObject)))
                            {
                                throw new InvalidOperationException("The values of a reference attribute must be a list of GUIDs or an AcmaPSObject");
                            }

                            IEnumerable<AcmaPSObject> unwrappedObjects = value.Cast<PSObject>().Select(t => t.BaseObject).Cast<AcmaPSObject>();

                            values.AddRange(unwrappedObjects.Select(t => t.Hologram.ObjectID as object));
                        }
                        else if (value.First() is AcmaPSObject)
                        {
                            if (value.Any(t => !(t is AcmaPSObject)))
                            {
                                throw new InvalidOperationException("The values of a reference attribute must be a list of GUIDs or an AcmaPSObject");
                            }

                            values.AddRange(value.Select(t => ((AcmaPSObject)t).Hologram.ObjectID as object));
                        }
                        else
                        {
                            throw new InvalidOperationException("The values of a reference attribute must be a list of GUIDs or an AcmaPSObject");
                        }
                    }
                }
                else
                {
                    foreach (object val in value)
                    {
                        values.Add(TypeConverter.ConvertData(val, attribute.Type));
                    }
                }

                this.hologram.SetAttributeValue(attribute, values);
            }
        }
    }
}
