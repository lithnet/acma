using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Lithnet.Acma.Service;
using Lithnet.Acma.ServiceModel;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class AcmaServiceGetResourcesTest
    {
        private static readonly List<string> ReadAttributes = new List<string>() { "mail", "mailAlternateAddresses", "supervisorAccountName" };

        public AcmaServiceGetResourcesTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void EveryPageHoldsTheObjectsAsTheyWereWhenTheReadStarted()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                Guid supervisorId = AcmaServiceGetResourcesTest.CreatePerson(createdIds, "before", Guid.Empty);
                List<Guid> personIds = new List<Guid>();

                for (int i = 0; i < 4; i++)
                {
                    personIds.Add(AcmaServiceGetResourcesTest.CreatePerson(createdIds, "before" + i, supervisorId));
                }

                AcmaService service = new AcmaService();
                List<AcmaResource> resources = new List<AcmaResource>();
                AcmaResourcePage page = service.GetResources("person", AcmaServiceGetResourcesTest.ReadAttributes, 2);
                resources.AddRange(page.Resources);
                Assert.IsNotNull(page.ContinuationToken, "The read fitted in one page");

                // Single-valued, multivalued and inherited values change, one object is deleted and one is created, between pages
                foreach (Guid id in personIds.Concat(new[] { supervisorId }))
                {
                    MAObjectHologram hologram = ActiveConfig.DB.GetMAObjectOrDefault(id);
                    hologram.SetObjectModificationType(ObjectModificationType.Update, false);
                    hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "after@test.com");
                    hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "after-alt@test.com" });
                    hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "after");
                    hologram.CommitCSEntryChange();
                }

                ActiveConfig.DB.DeleteMAObjectPermanent(personIds[3]);
                Guid createdDuringRead = AcmaServiceGetResourcesTest.CreatePerson(createdIds, "during", Guid.Empty);

                int pages = 1;

                while (page.ContinuationToken != null)
                {
                    page = service.GetResources("person", AcmaServiceGetResourcesTest.ReadAttributes, 2, page.ContinuationToken);
                    resources.AddRange(page.Resources);
                    pages++;
                    Assert.IsTrue(pages < 10000, "Paging did not end");
                }

                Assert.AreEqual(resources.Count, resources.Select(t => t.ObjectID).Distinct().Count(), "An object was returned more than once");
                Assert.IsFalse(resources.Any(t => t.ObjectID == createdDuringRead), "An object created during the read was returned");

                AcmaResource supervisor = resources.Single(t => t.ObjectID == supervisorId);
                Assert.AreEqual("before@test.com", supervisor.Attributes["mail"].Single());

                for (int i = 0; i < personIds.Count; i++)
                {
                    AcmaResource person = resources.Single(t => t.ObjectID == personIds[i]);
                    Assert.AreEqual(string.Format("before{0}@test.com", i), person.Attributes["mail"].Single());
                    CollectionAssert.AreEquivalent(new List<string>() { string.Format("before{0}-alt1@test.com", i), string.Format("before{0}-alt2@test.com", i) }, person.Attributes["mailAlternateAddresses"].ToList());
                    Assert.AreEqual("before", person.Attributes["supervisorAccountName"].Single());
                }
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void TheReadNeitherSeesNorWaitsForUncommittedWrites()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                Guid supervisorId = AcmaServiceGetResourcesTest.CreatePerson(createdIds, "before", Guid.Empty);
                Guid personId = AcmaServiceGetResourcesTest.CreatePerson(createdIds, "before0", supervisorId);

                // A write that holds locks on the object's row, its multivalued values and the row it inherits from.
                // A read that is not a snapshot would wait for it, and time out
                using (SqlConnection connection = new SqlConnection(ActiveConfig.DB.ConnectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        SqlCommand command = new SqlCommand(
                            "UPDATE [dbo].[MA_Objects] SET [mail] = 'uncommitted@test.com' WHERE [objectId] = @personId; " +
                            "UPDATE [dbo].[MA_Attributes] SET [attributeValueString] = 'uncommitted-alt@test.com' WHERE [objectId] = @personId AND [attributeName] = 'mailAlternateAddresses'; " +
                            "UPDATE [dbo].[MA_Objects] SET [accountName] = 'uncommitted' WHERE [objectId] = @supervisorId",
                            connection,
                            transaction);
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@supervisorId", supervisorId);
                        command.ExecuteNonQuery();

                        AcmaResource person = AcmaServiceGetResourcesTest.ReadAll(new AcmaService()).Single(t => t.ObjectID == personId);

                        Assert.AreEqual("before0@test.com", person.Attributes["mail"].Single());
                        CollectionAssert.AreEquivalent(new List<string>() { "before0-alt1@test.com", "before0-alt2@test.com" }, person.Attributes["mailAlternateAddresses"].ToList());
                        Assert.AreEqual("before", person.Attributes["supervisorAccountName"].Single());

                        transaction.Rollback();
                    }
                }
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void AReadThatFitsInOnePageHasNoContinuationToken()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                Guid id = AcmaServiceGetResourcesTest.CreatePerson(createdIds, "single", Guid.Empty);

                AcmaResourcePage page = new AcmaService().GetResources("person", null, AcmaServiceConstants.MaximumPageSize);

                Assert.IsTrue(page.Resources.Count < AcmaServiceConstants.MaximumPageSize, "The test database holds more person objects than one page");
                Assert.IsNull(page.ContinuationToken);
                Assert.IsTrue(page.Resources.Any(t => t.ObjectID == id), "The object was not returned");
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void TheLastPageEndsTheRead()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "a", Guid.Empty);
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "b", Guid.Empty);

                AcmaService service = new AcmaService();
                AcmaResourcePage page = service.GetResources("person", null, 1);
                string token = page.ContinuationToken;

                while (page.ContinuationToken != null)
                {
                    page = service.GetResources("person", null, 1, page.ContinuationToken);
                }

                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 1, token));
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void AnErrorEndsTheRead()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "a", Guid.Empty);
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "b", Guid.Empty);

                AcmaService service = new AcmaService();
                string token = service.GetResources("person", null, 1).ContinuationToken;

                AcmaServiceGetResourcesTest.AssertThrows<ArgumentOutOfRangeException>(() => service.GetResources("person", null, 0, token));
                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 1, token));
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void ReleaseEndsTheRead()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "a", Guid.Empty);
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "b", Guid.Empty);

                AcmaService service = new AcmaService();
                string token = service.GetResources("person", null, 1).ContinuationToken;

                service.ReleaseResources(token);

                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 1, token));
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void AnIdleReadExpires()
        {
            List<Guid> createdIds = new List<Guid>();
            TimeSpan expiry = AcmaService.ReadContextExpiry;

            try
            {
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "a", Guid.Empty);
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "b", Guid.Empty);
                AcmaService.ReadContextExpiry = TimeSpan.FromSeconds(1);

                AcmaService service = new AcmaService();
                string token = service.GetResources("person", null, 1).ContinuationToken;

                Thread.Sleep(TimeSpan.FromSeconds(2));

                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 1, token));
            }
            finally
            {
                AcmaService.ReadContextExpiry = expiry;
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        [TestMethod()]
        public void PageSizeOutsideTheLimitsIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<ArgumentOutOfRangeException>(() => service.GetResources("person", null, 0));
            AcmaServiceGetResourcesTest.AssertThrows<ArgumentOutOfRangeException>(() => service.GetResources("person", null, AcmaServiceConstants.MaximumPageSize + 1));
        }

        [TestMethod()]
        public void AnUnknownContinuationTokenIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 10, "not-a-token"));
            AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 10, Guid.NewGuid().ToString()));
        }

        [TestMethod()]
        public void AnUnknownClassIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<NoSuchClassException>(() => service.GetResources("noSuchClass", null, 10));
        }

        [TestMethod()]
        public void AnAttributeNotBoundToTheClassIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<NoSuchAttributeInObjectTypeException>(() => service.GetResources("group", new List<string>() { "mail" }, 10));
        }

        [TestMethod()]
        public void ADifferentObjectTypeOrAttributeListEndsTheRead()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "a", Guid.Empty);
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "b", Guid.Empty);
                AcmaServiceGetResourcesTest.CreatePerson(createdIds, "c", Guid.Empty);

                AcmaService service = new AcmaService();

                string token = service.GetResources("person", new List<string>() { "mail" }, 1).ContinuationToken;
                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", new List<string>() { "accountName" }, 1, token));
                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", new List<string>() { "mail" }, 1, token));

                token = service.GetResources("person", null, 1).ContinuationToken;
                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("group", null, 1, token));
                AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 1, token));
            }
            finally
            {
                AcmaServiceGetResourcesTest.Delete(createdIds);
            }
        }

        private static List<AcmaResource> ReadAll(AcmaService service)
        {
            List<AcmaResource> resources = new List<AcmaResource>();
            string continuationToken = null;

            do
            {
                AcmaResourcePage page = service.GetResources("person", AcmaServiceGetResourcesTest.ReadAttributes, 2, continuationToken);
                resources.AddRange(page.Resources);
                continuationToken = page.ContinuationToken;
            }
            while (continuationToken != null);

            return resources;
        }

        private static Guid CreatePerson(List<Guid> createdIds, string name, Guid supervisorId)
        {
            Guid id = Guid.NewGuid();
            MAObjectHologram hologram = ActiveConfig.DB.CreateMAObject(id, "person");
            hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), name);
            hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), name + "@test.com");
            hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { name + "-alt1@test.com", name + "-alt2@test.com" });

            if (supervisorId != Guid.Empty)
            {
                hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
            }

            hologram.CommitCSEntryChange();
            createdIds.Add(id);
            return id;
        }

        private static void Delete(List<Guid> ids)
        {
            foreach (Guid id in ids)
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        private static void AssertThrows<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                return;
            }

            Assert.Fail("{0} was not thrown", typeof(T).Name);
        }
    }
}
