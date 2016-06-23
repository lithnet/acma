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
using System.Collections.ObjectModel;


namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class AcmaInternalExitEventTest
    {
        public AcmaInternalExitEventTest()
        {
            UnitTestControl.Initialize();
        }
        
        [TestMethod()]
        public void TestSerialization()
        {
            AcmaInternalExitEvent toSerialize = new AcmaInternalExitEvent();
            toSerialize.ID = "testName";
            toSerialize.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            toSerialize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });
            toSerialize.Recipients.Add(ActiveConfig.DB.GetAttribute("supervisor"));
            toSerialize.Recipients.Add(ActiveConfig.DB.GetAttribute("directReports"));
            toSerialize.RecipientQueries = new ObservableCollection<DBQueryObject>();
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            toSerialize.RecipientQueries.Add(group);
            group.DBQueries.Add(new DBQueryByValue( ActiveConfig.DB.GetAttribute("sn"), ValueOperator.EndsWith, ActiveConfig.DB.GetAttribute("firstName")));

            Lithnet.Common.ObjectModel.UniqueIDCache.ClearIdCache();
            AcmaInternalExitEvent deserialized = UnitTestControl.XmlSerializeRoundTrip<AcmaInternalExitEvent>(toSerialize);

            Assert.AreEqual(toSerialize.ID, deserialized.ID);
            Assert.AreEqual(toSerialize.RuleGroup.Operator, deserialized.RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)toSerialize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.RuleGroup.Items[0]).TriggerEvents);
            CollectionAssert.AreEqual(toSerialize.Recipients, deserialized.Recipients);

            DBQueryGroup deserializedGroup = toSerialize.RecipientQueries[0] as DBQueryGroup;

            Assert.AreEqual(group.Operator, deserializedGroup.Operator);
            Assert.AreEqual(((DBQueryByValue)group.DBQueries[0]).SearchAttribute, ((DBQueryByValue)deserializedGroup.DBQueries[0]).SearchAttribute);
            Assert.AreEqual(((DBQueryByValue)group.DBQueries[0]).Operator, ((DBQueryByValue)deserializedGroup.DBQueries[0]).Operator);
            Assert.AreEqual(((DBQueryByValue)group.DBQueries[0]).ValueDeclarations[0].Declaration, ((DBQueryByValue)deserializedGroup.DBQueries[0]).ValueDeclarations[0].Declaration);
        }
    }
}
