using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using Lithnet.Logging;
using System.Management.Automation;
using Lithnet.MetadirectoryServices;
using System.Collections;

namespace Lithnet.Acma.PS
{
    public class AcmaPSObject : PSObject, IDisposable
    {
        private MAObjectHologram hologram;

        internal AcmaPSObject(MAObjectHologram hologram)
        {
            if (hologram == null)
            {
                throw new ArgumentNullException("hologram");
            }

            this.hologram = hologram;
            this.LoadProperties();
        }

        private void LoadProperties()
        {
            foreach (AcmaSchemaAttribute attribute in this.hologram.ObjectClass.Attributes.OrderBy(t => t.Name))
            {
                PSNoteProperty prop = null;

                if (attribute.IsMultivalued)
                {
                    object[] values = this.hologram.GetMVAttributeValues(attribute).Values.Select(t => t.Value).ToArray();

                    if (values.Length == 0)
                    {
                        prop = new PSNoteProperty(attribute.Name, new AttributeValueArrayList());
                    }
                    //if (values.Length == 1)
                    //{
                    //    prop = new PSNoteProperty(attribute.Name, values.First());
                    //}
                    else 
                    {
                        prop = new PSNoteProperty(attribute.Name, new AttributeValueArrayList(values));
                    }
                }
                else
                {
                    AttributeValue value = this.hologram.GetSVAttributeValue(attribute);
                    if (value.IsNull)
                    {
                        prop = new PSNoteProperty(attribute.Name, null);
                    }
                    else
                    {
                        prop = new PSNoteProperty(attribute.Name, value.Value);
                    }
                }

                if (prop != null)
                {
                    this.Properties.Add(prop);
                }
            }
        }

        internal MAObjectHologram Hologram
        {
            get
            {
                return this.hologram;
            }
        }

        internal MAObjectHologram GetResourceWithAppliedChanges()
        {
            foreach (var property in this.Properties)
            {
                if (ActiveConfig.DB.GetAttribute(property.Name).IsReadOnlyInClass(this.Hologram.ObjectClass))
                {
                    continue;
                }

                byte[] byteArray = property.Value as byte[];

                if (byteArray != null)
                {
                    this.SetSingleValuedAttribute(property, property.Value);
                }
                else
                {
                    IList resourceValues = property.Value as IList;

                    if (resourceValues != null)
                    {
                        this.SetMultivaluedAttribute(property, resourceValues);
                    }
                    else
                    {
                        this.SetSingleValuedAttribute(property, property.Value);
                    }
                }
            }

            return this.Hologram;
        }

        private void SetMultivaluedAttribute(PSPropertyInfo property, IList resourceValues)
        {
            List<object> newValues = new List<object>();

            foreach (object value in resourceValues)
            {
                AcmaPSObject resourceValue = value as AcmaPSObject;

                if (resourceValue != null)
                {
                    newValues.Add(resourceValue.Hologram.ObjectID);
                }
                else
                {
                    newValues.Add(this.UnwrapPSObject(value));
                }
            }

            this.Hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute(property.Name), newValues);
        }

        private void SetSingleValuedAttribute(PSPropertyInfo property, object value)
        {
            AcmaPSObject resourceValue = property.Value as AcmaPSObject;

            if (resourceValue != null)
            {
                this.Hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute(property.Name), resourceValue.Hologram.ObjectID);
            }
            else
            {
                this.Hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute(property.Name), value);
            }
        }

        private object UnwrapPSObject(object value)
        {
            PSObject psObject = value as PSObject;

            if (psObject != null)
            {
                return psObject.BaseObject;
            }
            else
            {
                return value;
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.hologram = null;
                }

                // shared cleanup logic
                disposed = true;
            }
        }

        ~AcmaPSObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}