using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;

namespace Lithnet.Acma
{
    [DataContract(Name = "acma", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class XmlConfigFile : UINotifyPropertyChanges, IExtensibleDataObject
    {
        public XmlConfigFile()
        {
            this.Initialize();
        }

        [DataMember(Name = "config-version", Order = 0)]
        public string ConfigVersion { get; set; }

        [DataMember(Name = "description", Order = 1)]
        public string Description { get; set; }

        [DataMember(Name = "transforms", Order = 5)]
        public TransformKeyedCollection Transforms { get; set; }

        [DataMember(Name = "class-constructors", Order = 10)]
        public ClassConstructors ClassConstructors { get; set; }

        [DataMember(Name = "operation-events", Order = 15)]
        public AcmaEvents OperationEvents {get;set;}

        public string FileName { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }

        public IEnumerable<SchemaAttributeUsage> GetAttributeUsage(AcmaSchemaAttribute attribute, AcmaSchemaObjectClass objectClass)
        {
            if (this.ClassConstructors.Contains(objectClass.Name))
            {
                return this.ClassConstructors[objectClass.Name].GetAttributeUsage(string.Empty, attribute);
            }

            return null;
        }

        public IEnumerable<SchemaAttributeUsage> GetAttributeUsage(AcmaSchemaAttribute attribute)
        {
            foreach(ClassConstructor constructor in this.ClassConstructors)
            {
                foreach (SchemaAttributeUsage usage in constructor.GetAttributeUsage(string.Empty, attribute))
                {
                    yield return usage;
                }
            }
        }

        private void Initialize()
        {
            TransformGlobal.HostProcessSupportsNativeDateTime = true;
            this.Transforms = new TransformKeyedCollection();
            this.ClassConstructors = new ClassConstructors();
            this.OperationEvents = new AcmaEvents();
        }

        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
