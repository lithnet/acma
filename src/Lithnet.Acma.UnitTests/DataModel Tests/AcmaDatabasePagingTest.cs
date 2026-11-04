using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Lithnet.Acma.DataModel;
using Lithnet.Acma.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class AcmaDatabasePagingTest
    {
        public AcmaDatabasePagingTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void PagesReturnEveryObjectOnceInObjectIdOrder()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                for (int i = 0; i < 7; i++)
                {
                    Guid id = Guid.NewGuid();
                    ActiveConfig.DB.CreateMAObject(id, "person");
                    createdIds.Add(id);
                }

                List<Guid> pagedIds = AcmaDatabasePagingTest.ReadAllPages("person", 3);

                foreach (Guid id in createdIds)
                {
                    Assert.AreEqual(1, pagedIds.Count(t => t == id), "Object {0} was not returned exactly once", id);
                }

                for (int i = 1; i < pagedIds.Count; i++)
                {
                    if (new SqlGuid(pagedIds[i - 1]).CompareTo(new SqlGuid(pagedIds[i])) >= 0)
                    {
                        Assert.Fail("Objects were not returned in object ID order: {0} came before {1}", pagedIds[i - 1], pagedIds[i]);
                    }
                }
            }
            finally
            {
                foreach (Guid id in createdIds)
                {
                    ActiveConfig.DB.DeleteMAObjectPermanent(id);
                }
            }
        }

        [TestMethod()]
        public void PagesDoNotReturnDeletedObjects()
        {
            Guid id = Guid.NewGuid();

            try
            {
                MAObjectHologram hologram = ActiveConfig.DB.CreateMAObject(id, "person");
                hologram.Delete(false);

                Assert.AreNotEqual(0, ActiveConfig.DB.GetMAObjectOrDefault(id).DeletedTimestamp, "The test object was not marked as deleted");

                List<Guid> pagedIds = AcmaDatabasePagingTest.ReadAllPages("person", 100);

                Assert.IsFalse(pagedIds.Contains(id), "A deleted object was returned");
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        [TestMethod()]
        public void PagesReturnOnlyTheRequestedClass()
        {
            Guid personId = Guid.NewGuid();
            Guid groupId = Guid.NewGuid();

            try
            {
                ActiveConfig.DB.CreateMAObject(personId, "person");
                ActiveConfig.DB.CreateMAObject(groupId, "group");

                List<Guid> pagedIds = AcmaDatabasePagingTest.ReadAllPages("person", 100);

                Assert.IsTrue(pagedIds.Contains(personId), "The person object was not returned");
                Assert.IsFalse(pagedIds.Contains(groupId), "An object of another class was returned");
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(personId);
                ActiveConfig.DB.DeleteMAObjectPermanent(groupId);
            }
        }

        [TestMethod()]
        public void PageSizeBelowOneIsRejected()
        {
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            try
            {
                ActiveConfig.DB.GetMAObjectsPage(objectClass, Guid.Empty, 0);
                Assert.Fail("A page size of 0 was accepted");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        [TestMethod()]
        public void ProjectedResourceCarriesOnlyTheRequestedAttributes()
        {
            Guid id = Guid.NewGuid();

            try
            {
                MAObjectHologram hologram = ActiveConfig.DB.CreateMAObject(id, "person");
                hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test");
                hologram.CommitCSEntryChange();

                AcmaResource resource = hologram.ToAcmaResource(new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("mail") });

                Assert.AreEqual(id, resource.ObjectID);
                Assert.AreEqual("person", resource.ObjectType);
                Assert.AreEqual("test.test@test.com", resource.Attributes["mail"].Single());
                Assert.AreEqual(id.ToString(), resource.Attributes["objectId"].Single());
                Assert.AreEqual("person", resource.Attributes["objectClass"].Single());
                Assert.IsFalse(resource.Attributes.ContainsKey("sn"), "An attribute that was not requested was returned");
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        [TestMethod()]
        public void ProjectedResourceOmitsEmptyAttributes()
        {
            Guid id = Guid.NewGuid();

            try
            {
                MAObjectHologram hologram = ActiveConfig.DB.CreateMAObject(id, "person");

                AcmaResource resource = hologram.ToAcmaResource(new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("mail") });

                Assert.IsFalse(resource.Attributes.ContainsKey("mail"), "An empty attribute was returned");
                Assert.AreEqual(id.ToString(), resource.Attributes["objectId"].Single());
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        private static List<Guid> ReadAllPages(string objectClassName, int pageSize)
        {
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(objectClassName);
            List<Guid> ids = new List<Guid>();
            Guid after = Guid.Empty;

            while (true)
            {
                IList<MAObjectHologram> page = ActiveConfig.DB.GetMAObjectsPage(objectClass, after, pageSize);
                Assert.IsTrue(page.Count <= pageSize, "A page held more objects than the page size");
                ids.AddRange(page.Select(t => t.ObjectID));

                if (page.Count < pageSize)
                {
                    return ids;
                }

                after = page[page.Count - 1].ObjectID;
            }
        }
    }
}
