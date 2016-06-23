using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Lithnet.Acma;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.UnitTests
{
    [TestClass]
    public class AcmaCSEntryChangeTest
    {
        public AcmaCSEntryChangeTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod]
        public void TestSerialization()
        {
            AcmaCSEntryChange toSerialize = new AcmaCSEntryChange();

            toSerialize.ObjectModificationType = ObjectModificationType.Update;
            toSerialize.ObjectType = "person";
            toSerialize.DN = "testDN";
            toSerialize.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sn", "myValue"));
            toSerialize.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object>() { "myValue1", "myValue2" }));
            toSerialize.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("accountName"));
            toSerialize.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("activeExpiryDate", 900L));
            toSerialize.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("displayName", new List<ValueChange>() { ValueChange.CreateValueAdd("addValue"), ValueChange.CreateValueDelete("deleteValue") }));
            toSerialize.AnchorAttributes.Add(AnchorAttribute.Create("myAnchor1", "99"));
            toSerialize.AnchorAttributes.Add(AnchorAttribute.Create("myAnchor2", "my second test anchor"));


            AcmaCSEntryChange deserialized = UnitTestControl.XmlSerializeRoundTrip<AcmaCSEntryChange>(toSerialize);

            Assert.AreEqual(toSerialize.ObjectModificationType, deserialized.ObjectModificationType);
            Assert.AreEqual(toSerialize.ObjectType, deserialized.ObjectType);
            Assert.AreEqual(toSerialize.DN, deserialized.DN);

            Assert.AreEqual(toSerialize.AttributeChanges[0].Name, deserialized.AttributeChanges[0].Name);
            Assert.AreEqual(toSerialize.AttributeChanges[0].ModificationType, deserialized.AttributeChanges[0].ModificationType);
            Assert.AreEqual(toSerialize.AttributeChanges[0].ValueChanges[0].Value, deserialized.AttributeChanges[0].ValueChanges[0].Value);
            Assert.AreEqual(toSerialize.AttributeChanges[0].ValueChanges[0].ModificationType, deserialized.AttributeChanges[0].ValueChanges[0].ModificationType);

            Assert.AreEqual(toSerialize.AttributeChanges[1].Name, deserialized.AttributeChanges[1].Name);
            Assert.AreEqual(toSerialize.AttributeChanges[1].ModificationType, deserialized.AttributeChanges[1].ModificationType);
            Assert.AreEqual(toSerialize.AttributeChanges[1].ValueChanges[0].Value, deserialized.AttributeChanges[1].ValueChanges[0].Value);
            Assert.AreEqual(toSerialize.AttributeChanges[1].ValueChanges[0].ModificationType, deserialized.AttributeChanges[1].ValueChanges[0].ModificationType);
            Assert.AreEqual(toSerialize.AttributeChanges[1].ValueChanges[1].Value, deserialized.AttributeChanges[1].ValueChanges[1].Value);
            Assert.AreEqual(toSerialize.AttributeChanges[1].ValueChanges[1].ModificationType, deserialized.AttributeChanges[1].ValueChanges[1].ModificationType);

            Assert.AreEqual(toSerialize.AttributeChanges[2].Name, deserialized.AttributeChanges[2].Name);
            Assert.AreEqual(toSerialize.AttributeChanges[2].ModificationType, deserialized.AttributeChanges[2].ModificationType);
            
            Assert.AreEqual(toSerialize.AttributeChanges[3].Name, deserialized.AttributeChanges[3].Name);
            Assert.AreEqual(toSerialize.AttributeChanges[3].ModificationType, deserialized.AttributeChanges[3].ModificationType);
            Assert.AreEqual(toSerialize.AttributeChanges[3].ValueChanges[0].Value, deserialized.AttributeChanges[3].ValueChanges[0].Value);
            Assert.AreEqual(toSerialize.AttributeChanges[3].ValueChanges[0].ModificationType, deserialized.AttributeChanges[3].ValueChanges[0].ModificationType);

            Assert.AreEqual(toSerialize.AttributeChanges[4].Name, deserialized.AttributeChanges[4].Name);
            Assert.AreEqual(toSerialize.AttributeChanges[4].ModificationType, deserialized.AttributeChanges[4].ModificationType);
            Assert.AreEqual(toSerialize.AttributeChanges[4].ValueChanges[0].Value, deserialized.AttributeChanges[4].ValueChanges[0].Value);
            Assert.AreEqual(toSerialize.AttributeChanges[4].ValueChanges[0].ModificationType, deserialized.AttributeChanges[4].ValueChanges[0].ModificationType);
            Assert.AreEqual(toSerialize.AttributeChanges[4].ValueChanges[1].Value, deserialized.AttributeChanges[4].ValueChanges[1].Value);
            Assert.AreEqual(toSerialize.AttributeChanges[4].ValueChanges[1].ModificationType, deserialized.AttributeChanges[4].ValueChanges[1].ModificationType);

            Assert.AreEqual(toSerialize.AnchorAttributes[0].Name, deserialized.AnchorAttributes[0].Name);
            Assert.AreEqual(toSerialize.AnchorAttributes[0].Value, deserialized.AnchorAttributes[0].Value);

            Assert.AreEqual(toSerialize.AnchorAttributes[1].Name, deserialized.AnchorAttributes[1].Name);
            Assert.AreEqual(toSerialize.AnchorAttributes[1].Value, deserialized.AnchorAttributes[1].Value);
        }
    }
}
