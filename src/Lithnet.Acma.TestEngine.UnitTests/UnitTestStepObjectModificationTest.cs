using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Acma;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using Lithnet.Acma.UnitTests;

namespace Lithnet.Acma.TestEngine.UnitTests
{
    [TestClass]
    public class UnitTestStepObjectModificationTest
    {
        public UnitTestStepObjectModificationTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod]
        public void TestSerialization()
        {
            UnitTestStepObjectModification toSerlialize = new UnitTestStepObjectModification();

            AcmaCSEntryChange csentry = new AcmaCSEntryChange();

            csentry.ObjectModificationType = ObjectModificationType.Update;
            csentry.ObjectType = "person";
            csentry.DN = "testDN";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sn", "myValue"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object>() { "myValue1", "myValue2" }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("accountName"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("activeExpiryDate", 900L));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("displayName", new List<ValueChange>() { ValueChange.CreateValueAdd("addValue"), ValueChange.CreateValueDelete("deleteValue") }));
            csentry.AnchorAttributes.Add(AnchorAttribute.Create("myAnchor1", "99"));
            csentry.AnchorAttributes.Add(AnchorAttribute.Create("myAnchor2", "my second test anchor"));
            
            toSerlialize.CSEntryChange = csentry;

            UnitTestStepObjectModification deserialized = UnitTestControl.XmlSerializeRoundTrip<UnitTestStepObjectModification>(toSerlialize);

            CSEntryChange deserializedCSEntry = deserialized.CSEntryChange;

            Assert.AreEqual(csentry.ObjectModificationType, deserializedCSEntry.ObjectModificationType);
            Assert.AreEqual(csentry.ObjectType, deserializedCSEntry.ObjectType);
            Assert.AreEqual(csentry.DN, deserializedCSEntry.DN);

            Assert.AreEqual(csentry.AttributeChanges[0].Name, deserializedCSEntry.AttributeChanges[0].Name);
            Assert.AreEqual(csentry.AttributeChanges[0].ModificationType, deserializedCSEntry.AttributeChanges[0].ModificationType);
            Assert.AreEqual(csentry.AttributeChanges[0].ValueChanges[0].Value, deserializedCSEntry.AttributeChanges[0].ValueChanges[0].Value);
            Assert.AreEqual(csentry.AttributeChanges[0].ValueChanges[0].ModificationType, deserializedCSEntry.AttributeChanges[0].ValueChanges[0].ModificationType);

            Assert.AreEqual(csentry.AttributeChanges[1].Name, deserializedCSEntry.AttributeChanges[1].Name);
            Assert.AreEqual(csentry.AttributeChanges[1].ModificationType, deserializedCSEntry.AttributeChanges[1].ModificationType);
            Assert.AreEqual(csentry.AttributeChanges[1].ValueChanges[0].Value, deserializedCSEntry.AttributeChanges[1].ValueChanges[0].Value);
            Assert.AreEqual(csentry.AttributeChanges[1].ValueChanges[0].ModificationType, deserializedCSEntry.AttributeChanges[1].ValueChanges[0].ModificationType);
            Assert.AreEqual(csentry.AttributeChanges[1].ValueChanges[1].Value, deserializedCSEntry.AttributeChanges[1].ValueChanges[1].Value);
            Assert.AreEqual(csentry.AttributeChanges[1].ValueChanges[1].ModificationType, deserializedCSEntry.AttributeChanges[1].ValueChanges[1].ModificationType);

            Assert.AreEqual(csentry.AttributeChanges[2].Name, deserializedCSEntry.AttributeChanges[2].Name);
            Assert.AreEqual(csentry.AttributeChanges[2].ModificationType, deserializedCSEntry.AttributeChanges[2].ModificationType);

            Assert.AreEqual(csentry.AttributeChanges[3].Name, deserializedCSEntry.AttributeChanges[3].Name);
            Assert.AreEqual(csentry.AttributeChanges[3].ModificationType, deserializedCSEntry.AttributeChanges[3].ModificationType);
            Assert.AreEqual(csentry.AttributeChanges[3].ValueChanges[0].Value, deserializedCSEntry.AttributeChanges[3].ValueChanges[0].Value);
            Assert.AreEqual(csentry.AttributeChanges[3].ValueChanges[0].ModificationType, deserializedCSEntry.AttributeChanges[3].ValueChanges[0].ModificationType);

            Assert.AreEqual(csentry.AttributeChanges[4].Name, deserializedCSEntry.AttributeChanges[4].Name);
            Assert.AreEqual(csentry.AttributeChanges[4].ModificationType, deserializedCSEntry.AttributeChanges[4].ModificationType);
            Assert.AreEqual(csentry.AttributeChanges[4].ValueChanges[0].Value, deserializedCSEntry.AttributeChanges[4].ValueChanges[0].Value);
            Assert.AreEqual(csentry.AttributeChanges[4].ValueChanges[0].ModificationType, deserializedCSEntry.AttributeChanges[4].ValueChanges[0].ModificationType);
            Assert.AreEqual(csentry.AttributeChanges[4].ValueChanges[1].Value, deserializedCSEntry.AttributeChanges[4].ValueChanges[1].Value);
            Assert.AreEqual(csentry.AttributeChanges[4].ValueChanges[1].ModificationType, deserializedCSEntry.AttributeChanges[4].ValueChanges[1].ModificationType);

            Assert.AreEqual(csentry.AnchorAttributes[0].Name, deserializedCSEntry.AnchorAttributes[0].Name);
            Assert.AreEqual(csentry.AnchorAttributes[0].Value, deserializedCSEntry.AnchorAttributes[0].Value);

            Assert.AreEqual(csentry.AnchorAttributes[1].Name, deserializedCSEntry.AnchorAttributes[1].Name);
            Assert.AreEqual(csentry.AnchorAttributes[1].Value, deserializedCSEntry.AnchorAttributes[1].Value);
        }
    }
}
