using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for ReferenceLookupConstructorTest and is intended
    ///to contain all ReferenceLookupConstructorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReferenceLookupConstructorTest
    {
        public ReferenceLookupConstructorTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            ReferenceLookupConstructor toSeralize = new ReferenceLookupConstructor();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.ID = "abc123";
            toSeralize.Description = "some description";
            toSeralize.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            toSeralize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });
            toSeralize.MultipleResultAction = MultipleResultAction.UseFirst;
            toSeralize.QueryGroup = new DBQueryGroup() { Operator = GroupOperator.None };
            toSeralize.QueryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.EndsWith, ActiveConfig.DB.GetAttribute("firstName")));
            UniqueIDCache.ClearIdCache();
            ReferenceLookupConstructor deserialized = UnitTestControl.XmlSerializeRoundTrip<ReferenceLookupConstructor>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.ID, deserialized.ID);
            Assert.AreEqual(toSeralize.Description, deserialized.Description);
            Assert.AreEqual(toSeralize.MultipleResultAction, deserialized.MultipleResultAction);
            Assert.AreEqual(toSeralize.RuleGroup.Operator, deserialized.RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)toSeralize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.RuleGroup.Items[0]).TriggerEvents);

            Assert.AreEqual(toSeralize.QueryGroup.Operator, deserialized.QueryGroup.Operator);
            Assert.AreEqual(((DBQueryByValue)toSeralize.QueryGroup.DBQueries[0]).SearchAttribute, ((DBQueryByValue)deserialized.QueryGroup.DBQueries[0]).SearchAttribute);
            Assert.AreEqual(((DBQueryByValue)toSeralize.QueryGroup.DBQueries[0]).Operator, ((DBQueryByValue)deserialized.QueryGroup.DBQueries[0]).Operator);
            Assert.AreEqual(((DBQueryByValue)toSeralize.QueryGroup.DBQueries[0]).ValueDeclarations[0].Declaration, ((DBQueryByValue)deserialized.QueryGroup.DBQueries[0]).ValueDeclarations[0].Declaration);
        }

        [TestMethod()]
        public void ReferenceLookupUseAllResults()
        {
            ReferenceLookupConstructor constructor = new ReferenceLookupConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("displayNameSharers");
            constructor.MultipleResultAction = MultipleResultAction.UseAll;
            constructor.QueryGroup = new DBQueryGroup() { Operator = GroupOperator.All };
            constructor.QueryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("displayName"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("displayName")));

            Guid object1Id = Guid.NewGuid();
            Guid object2Id = Guid.NewGuid();
            Guid object3Id = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            try
            {
                MAObjectHologram object1 = ActiveConfig.DB.CreateMAObject(object1Id, "person");
                object1.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object1.CommitCSEntryChange();

                MAObjectHologram object2 = ActiveConfig.DB.CreateMAObject(object2Id, "person");
                object2.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object2.CommitCSEntryChange();

                MAObjectHologram object3 = ActiveConfig.DB.CreateMAObject(object3Id, "person");
                object3.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                constructor.Execute(object3);
                object3.CommitCSEntryChange();

                object3 = ActiveConfig.DB.GetMAObject(object3Id, objectClass);
                AttributeValues values = object3.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("displayNameSharers"));

                if (values.IsEmptyOrNull)
                {
                    Assert.Fail("The constructor did not create any results");
                }

                if (!values.ContainsAllElements(new List<object> { object1Id, object2Id }))
                {
                    Assert.Fail("The constructor did not create the correct values");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(object1Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object2Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object3Id);
            }
        }

        [TestMethod()]
        public void ReferenceLookupUseFirstResult()
        {
            ReferenceLookupConstructor constructor = new ReferenceLookupConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("displayNameSharers");
            constructor.MultipleResultAction = MultipleResultAction.UseFirst;
            constructor.QueryGroup = new DBQueryGroup() { Operator = GroupOperator.All };
            constructor.QueryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("displayName"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("displayName")));

            Guid object1Id = Guid.NewGuid();
            Guid object2Id = Guid.NewGuid();
            Guid object3Id = Guid.NewGuid();

            try
            {
                MAObjectHologram object1 = ActiveConfig.DB.CreateMAObject(object1Id, "person");
                object1.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object1.CommitCSEntryChange();

                MAObjectHologram object2 = ActiveConfig.DB.CreateMAObject(object2Id, "person");
                object2.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object2.CommitCSEntryChange();

                MAObjectHologram object3 = ActiveConfig.DB.CreateMAObject(object3Id, "person");
                object3.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                constructor.Execute(object3);
                object3.CommitCSEntryChange();
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

                object3 = ActiveConfig.DB.GetMAObject(object3Id, objectClass);
                AttributeValues values = object3.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("displayNameSharers"));

                if (values.IsEmptyOrNull)
                {
                    Assert.Fail("The constructor did not create any results");
                }

                if (!values.ContainsAllElements(new List<object> { object1Id }))
                {
                    Assert.Fail("The constructor did not create the correct values");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(object1Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object2Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object3Id);
            }
        }

        [TestMethod()]
        public void ReferenceLookupUseNoResults()
        {
            ReferenceLookupConstructor constructor = new ReferenceLookupConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("displayNameSharers");
            constructor.MultipleResultAction = MultipleResultAction.UseNone;
            constructor.QueryGroup = new DBQueryGroup() { Operator = GroupOperator.All };
            constructor.QueryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("displayName"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("displayName")));

            Guid object1Id = Guid.NewGuid();
            Guid object2Id = Guid.NewGuid();
            Guid object3Id = Guid.NewGuid();

            try
            {
                MAObjectHologram object1 = ActiveConfig.DB.CreateMAObject(object1Id, "person");
                object1.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object1.CommitCSEntryChange();

                MAObjectHologram object2 = ActiveConfig.DB.CreateMAObject(object2Id, "person");
                object2.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object2.CommitCSEntryChange();

                MAObjectHologram object3 = ActiveConfig.DB.CreateMAObject(object3Id, "person");
                object3.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                constructor.Execute(object3);
                object3.CommitCSEntryChange();
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

                object3 = ActiveConfig.DB.GetMAObject(object3Id, objectClass);
                AttributeValues values = object3.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("displayNameSharers"));

                if (!values.IsEmptyOrNull)
                {
                    Assert.Fail("The constructor did not create the correct values");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(object1Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object2Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object3Id);
            }
        }

        [TestMethod()]
        public void ReferenceLookupErrorOnMultipleResults()
        {
            ReferenceLookupConstructor constructor = new ReferenceLookupConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("displayNameSharers");
            constructor.MultipleResultAction = MultipleResultAction.Error;
            constructor.QueryGroup = new DBQueryGroup() { Operator = GroupOperator.All };
            constructor.QueryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("displayName"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("displayName")));

            Guid object1Id = Guid.NewGuid();
            Guid object2Id = Guid.NewGuid();
            Guid object3Id = Guid.NewGuid();

            try
            {
                MAObjectHologram object1 = ActiveConfig.DB.CreateMAObject(object1Id, "person");
                object1.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object1.CommitCSEntryChange();

                MAObjectHologram object2 = ActiveConfig.DB.CreateMAObject(object2Id, "person");
                object2.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                object2.CommitCSEntryChange();

                try
                {
                    MAObjectHologram object3 = ActiveConfig.DB.CreateMAObject(object3Id, "person");
                    object3.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "My Display Name");
                    constructor.Execute(object3);
                    object3.CommitCSEntryChange();
                    Assert.Fail("The expected exception was not throw");
                }
                catch (MultipleMatchException)
                {
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(object1Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object2Id);
                ActiveConfig.DB.DeleteMAObjectPermanent(object3Id);
            }
        }

    }
}
