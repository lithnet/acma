using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Acma.Service;
using Lithnet.Acma.ServiceModel;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class AcmaServiceGetResourcesTest
    {
        public AcmaServiceGetResourcesTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void ContinuationTokensReadEveryObjectOnce()
        {
            List<Guid> createdIds = new List<Guid>();

            try
            {
                for (int i = 0; i < 5; i++)
                {
                    Guid id = Guid.NewGuid();
                    MAObjectHologram hologram = ActiveConfig.DB.CreateMAObject(id, "person");
                    hologram.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), string.Format("test{0}@test.com", i));
                    hologram.CommitCSEntryChange();
                    createdIds.Add(id);
                }

                AcmaService service = new AcmaService();
                List<AcmaResource> resources = new List<AcmaResource>();
                string token = null;
                int pages = 0;

                do
                {
                    AcmaResourcePage page = service.GetResources("person", new List<string>() { "mail" }, 2, token);
                    Assert.IsTrue(page.Resources.Count <= 2, "A page held more resources than the page size");
                    resources.AddRange(page.Resources);
                    token = page.ContinuationToken;
                    pages++;
                    Assert.IsTrue(pages < 1000, "Paging did not end");
                }
                while (token != null);

                for (int i = 0; i < createdIds.Count; i++)
                {
                    List<AcmaResource> matches = resources.Where(t => t.ObjectID == createdIds[i]).ToList();
                    Assert.AreEqual(1, matches.Count, "Object {0} was not returned exactly once", createdIds[i]);
                    Assert.AreEqual(string.Format("test{0}@test.com", i), matches[0].Attributes["mail"].Single());
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
        public void APageSmallerThanThePageSizeHasNoContinuationToken()
        {
            Guid id = Guid.NewGuid();

            try
            {
                ActiveConfig.DB.CreateMAObject(id, "person");

                AcmaService service = new AcmaService();
                AcmaResourcePage page = service.GetResources("person", null, AcmaServiceConstants.MaximumPageSize, null);

                Assert.IsTrue(page.Resources.Count < AcmaServiceConstants.MaximumPageSize, "The test database holds more person objects than one page");
                Assert.IsNull(page.ContinuationToken);
                Assert.IsTrue(page.Resources.Any(t => t.ObjectID == id), "The object was not returned");
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        [TestMethod()]
        public void PageSizeOutsideTheLimitsIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<ArgumentOutOfRangeException>(() => service.GetResources("person", null, 0, null));
            AcmaServiceGetResourcesTest.AssertThrows<ArgumentOutOfRangeException>(() => service.GetResources("person", null, AcmaServiceConstants.MaximumPageSize + 1, null));
        }

        [TestMethod()]
        public void AnInvalidContinuationTokenIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<ArgumentException>(() => service.GetResources("person", null, 10, "not-a-token"));
        }

        [TestMethod()]
        public void AnUnknownClassIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<NoSuchClassException>(() => service.GetResources("noSuchClass", null, 10, null));
        }

        [TestMethod()]
        public void AnAttributeNotBoundToTheClassIsRejected()
        {
            AcmaService service = new AcmaService();

            AcmaServiceGetResourcesTest.AssertThrows<NoSuchAttributeInObjectTypeException>(() => service.GetResources("group", new List<string>() { "mail" }, 10, null));
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
