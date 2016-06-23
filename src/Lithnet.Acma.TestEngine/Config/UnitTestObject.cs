using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lithnet.Acma;
using Lithnet.Logging;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Lithnet.Common.ObjectModel;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "unit-test-object", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [KnownType(typeof(UnitTestGroup))]
    [KnownType(typeof(UnitTest))]
    public class UnitTestObject : UINotifyPropertyChanges, IIdentifiedObject
    {
        public const string CacheGroupName = "unittestobject";

        private string id;

        private bool deserializing;

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "test-id")]
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                try
                {
                    this.id = UniqueIDCache.SetID(this, value, UnitTestObject.CacheGroupName, this.deserializing);

                    if (this.id == null && UniqueIDCache.HasObject(UnitTestObject.CacheGroupName, this))
                    {
                        UniqueIDCache.RemoveItem(this, UnitTestObject.CacheGroupName);
                    }
                    else if (!UniqueIDCache.HasObject(UnitTestObject.CacheGroupName, this))
                    {
                        UniqueIDCache.AddItem(this, UnitTestObject.CacheGroupName);
                    }

                    this.RemoveError("Id");
                }
                catch (DuplicateIdentifierException)
                {
                    this.AddError("Id", "The specified ID is already in use");
                }
            }
        }

        public UnitTestObject()
        {
            this.Initialize();
        }

        private void Initialize()
        {
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;
        }

    }
}
