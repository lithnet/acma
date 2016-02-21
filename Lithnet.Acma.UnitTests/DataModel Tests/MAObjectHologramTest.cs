
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.Text.RegularExpressions;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;
using System.Collections.ObjectModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for MAObjectHologramTest and is intended
    ///to contain all MAObjectHologramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MAObjectHologramTest
    {
        public MAObjectHologramTest()
        {
            UnitTestControl.Initialize();
        }
        
        [TestMethod()]
        public void CommitSVAttributeString()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            this.CommitSVAttribute(testAttribute, "test.test@test.com");
        }

        [TestMethod()]
        public void CommitSVAttributeLong()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("unixUid");
            this.CommitSVAttribute(testAttribute, 55L);
        }

        [TestMethod()]
        public void CommitSVAttributeDateTime()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            this.CommitSVAttribute(testAttribute, DateTime.Parse("2010-01-01"));
        }

        [TestMethod()]
        public void CommitSVAttributeBoolean()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            this.CommitSVAttribute(testAttribute, true);
        }

        [TestMethod()]
        public void CommitSVAttributeBinary()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("objectSid");
            this.CommitSVAttribute(testAttribute, new byte[] { 0, 1, 2, 3, 4 });
        }

        [TestMethod()]
        public void CommitSVAttributeReference()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("supervisor");
            this.CommitSVAttribute(testAttribute, Guid.NewGuid());
        }

        [TestMethod()]
        public void CommitMVAttributeString()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            this.CommitMVAttribute(testAttribute, new List<object>() { "test.test@test.com", "test1.test1@test.com" });
        }

        [TestMethod()]
        public void CommitMVAttributeLong()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("expiryDates");
            this.CommitMVAttribute(testAttribute, new List<object>() { 55L, 66L, 77L, 5L });
        }

        [TestMethod()]
        public void CommitMVAttributeDateTime()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("dateTimeMV");
            this.CommitMVAttribute(testAttribute, new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01"), DateTime.Parse("2012-01-01") });
        }

        [TestMethod()]
        public void CommitMVAttributeReference()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("directReports");
            this.CommitMVAttribute(testAttribute, new List<object>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
        }

        [TestMethod()]
        public void CommitMVAttributeBinary()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("objectSids");
            this.CommitMVAttribute(testAttribute, new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } });
        }

        [TestMethod()]
        public void TestShadowProvisioning()
        {
            AcmaSchemaShadowObjectLink reference = ActiveConfig.DB.GetObjectClass("person").ShadowChildLinks.FirstOrDefault(t => t.ReferenceAttribute.Name == "adMgrAccount");
            AcmaSchemaObjectClass schemaShadowObject = ActiveConfig.DB.GetObjectClass("shadowAccountAuAdAdmin") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(reference.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(reference);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(reference.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }
            }
        }

        [TestMethod()]
        public void TestSubShadowProvisioning()
        {
            AcmaSchemaShadowObjectLink shadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadow");
            AcmaSchemaShadowObjectLink subShadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadowOfShadow");

            AcmaSchemaObjectClass shadowObjectClass = ActiveConfig.DB.GetObjectClass("testshadow") as AcmaSchemaObjectClass;
            AcmaSchemaObjectClass subShadowObjectClass = ActiveConfig.DB.GetObjectClass("testShadowOfShadow") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;
            Guid subShadowID = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(shadowLink.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(shadowLink);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(shadowLink.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.SetObjectModificationType(ObjectModificationType.Update, false);
                shadowObject.SetAttributeValue(subShadowLink.ProvisioningAttribute, true);
                MAObjectHologram subShadowObject = shadowObject.ProvisionShadowObject(subShadowLink);

                if (subShadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    subShadowID = subShadowObject.Id;
                }

                if (subShadowObject.ShadowParentId != shadowObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue subShadowChildId = shadowObject.GetSVAttributeValue(subShadowLink.ReferenceAttribute);

                if (subShadowChildId != subShadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }

                if (subShadowID != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(subShadowID);
                }
            }
        }

        [TestMethod()]
        public void TestSubShadowDeprovisioningBySubShadowDelete()
        {
            AcmaSchemaShadowObjectLink shadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadow");
            AcmaSchemaShadowObjectLink subShadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadowOfShadow");

            AcmaSchemaObjectClass shadowObjectClass = ActiveConfig.DB.GetObjectClass("testshadow") as AcmaSchemaObjectClass;
            AcmaSchemaObjectClass subShadowObjectClass = ActiveConfig.DB.GetObjectClass("testShadowOfShadow") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;
            Guid subShadowID = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(shadowLink.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(shadowLink);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(shadowLink.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.SetObjectModificationType(ObjectModificationType.Update, false);
                shadowObject.SetAttributeValue(subShadowLink.ProvisioningAttribute, true);
                MAObjectHologram subShadowObject = shadowObject.ProvisionShadowObject(subShadowLink);

                if (subShadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    subShadowID = subShadowObject.Id;
                }

                if (subShadowObject.ShadowParentId != shadowObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue subShadowChildId = shadowObject.GetSVAttributeValue(subShadowLink.ReferenceAttribute);

                if (subShadowChildId != subShadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                subShadowObject.Delete(true);

                shadowObject = UnitTestControl.DataContext.GetMAObject(shadowId, shadowObjectClass);

                subShadowChildId = shadowObject.GetSVAttributeValue(subShadowLink.ReferenceAttribute);

                if (!subShadowChildId.IsNull)
                {
                    Assert.Fail("The reference attribute for the sub shadow on the parent was not deleted");
                }

                if (shadowObject.GetSVAttributeValue(subShadowLink.ProvisioningAttribute).ValueBoolean == true)
                {
                    Assert.Fail("The provisioning attribute for the sub shadow on the parent was not deleted");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }

                if (subShadowID != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(subShadowID);
                }
            }
        }

        [TestMethod()]
        public void TestSubShadowDeprovisioningByShadowDelete()
        {
            AcmaSchemaShadowObjectLink shadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadow");
            AcmaSchemaShadowObjectLink subShadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadowOfShadow");

            AcmaSchemaObjectClass shadowObjectClass = ActiveConfig.DB.GetObjectClass("testshadow") as AcmaSchemaObjectClass;
            AcmaSchemaObjectClass subShadowObjectClass = ActiveConfig.DB.GetObjectClass("testShadowOfShadow") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;
            Guid subShadowID = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(shadowLink.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(shadowLink);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(shadowLink.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.SetObjectModificationType(ObjectModificationType.Update, false);
                shadowObject.SetAttributeValue(subShadowLink.ProvisioningAttribute, true);
                MAObjectHologram subShadowObject = shadowObject.ProvisionShadowObject(subShadowLink);

                if (subShadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    subShadowID = subShadowObject.Id;
                }

                if (subShadowObject.ShadowParentId != shadowObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue subShadowChildId = shadowObject.GetSVAttributeValue(subShadowLink.ReferenceAttribute);

                if (subShadowChildId != subShadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.Delete(true);


               subShadowObject = UnitTestControl.DataContext.GetMAObjectOrDefault(subShadowID);

                if (subShadowObject != null)
                {
                    Assert.Fail("The sub shadow object did not get deleted");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }

                if (subShadowID != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(subShadowID);
                }
            }
        }

        [TestMethod()]
        public void TestSubShadowDeprovisioningByParentDelete()
        {
            AcmaSchemaShadowObjectLink shadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadow");
            AcmaSchemaShadowObjectLink subShadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadowOfShadow");

            AcmaSchemaObjectClass shadowObjectClass = ActiveConfig.DB.GetObjectClass("testshadow") as AcmaSchemaObjectClass;
            AcmaSchemaObjectClass subShadowObjectClass = ActiveConfig.DB.GetObjectClass("testShadowOfShadow") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;
            Guid subShadowID = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(shadowLink.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(shadowLink);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(shadowLink.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.SetObjectModificationType(ObjectModificationType.Update, false);
                shadowObject.SetAttributeValue(subShadowLink.ProvisioningAttribute, true);
                MAObjectHologram subShadowObject = shadowObject.ProvisionShadowObject(subShadowLink);

                if (subShadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    subShadowID = subShadowObject.Id;
                }

                if (subShadowObject.ShadowParentId != shadowObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue subShadowChildId = shadowObject.GetSVAttributeValue(subShadowLink.ReferenceAttribute);

                if (subShadowChildId != subShadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                sourceObject.Delete(true);

                subShadowObject = UnitTestControl.DataContext.GetMAObjectOrDefault(subShadowID);

                if (subShadowObject != null)
                {
                    Assert.Fail("The sub shadow object did not get deleted");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }

                if (subShadowID != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(subShadowID);
                }
            }
        }

        [TestMethod()]
        public void TestSubShadowDeprovisioningByParentDeprovisionIndicator()
        {
            AcmaSchemaShadowObjectLink shadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadow");
            AcmaSchemaShadowObjectLink subShadowLink = ActiveConfig.DB.ShadowObjectLinks.First(t => t.Name == "testShadowOfShadow");

            AcmaSchemaObjectClass shadowObjectClass = ActiveConfig.DB.GetObjectClass("testshadow") as AcmaSchemaObjectClass;
            AcmaSchemaObjectClass subShadowObjectClass = ActiveConfig.DB.GetObjectClass("testShadowOfShadow") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;
            Guid subShadowID = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(shadowLink.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(shadowLink);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(shadowLink.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.SetObjectModificationType(ObjectModificationType.Update, false);
                shadowObject.SetAttributeValue(subShadowLink.ProvisioningAttribute, true);
                MAObjectHologram subShadowObject = shadowObject.ProvisionShadowObject(subShadowLink);

                if (subShadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    subShadowID = subShadowObject.Id;
                }

                if (subShadowObject.ShadowParentId != shadowObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue subShadowChildId = shadowObject.GetSVAttributeValue(subShadowLink.ReferenceAttribute);

                if (subShadowChildId != subShadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                sourceObject.SetAttributeValue(shadowLink.ProvisioningAttribute, false);
                sourceObject.CommitCSEntryChange();

                subShadowObject = UnitTestControl.DataContext.GetMAObjectOrDefault(subShadowID);

                if (subShadowObject != null)
                {
                    Assert.Fail("The sub shadow object did not get deleted");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }

                if (subShadowID != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(subShadowID);
                }
            }
        }


        [TestMethod()]
        public void TestShadowDeprovisioningByShadowDelete()
        {
            AcmaSchemaShadowObjectLink reference = ActiveConfig.DB.GetObjectClass("person").ShadowChildLinks.FirstOrDefault(t => t.ReferenceAttribute.Name == "adMgrAccount");
            AcmaSchemaObjectClass schemaShadowObject = ActiveConfig.DB.GetObjectClass("shadowAccountAuAdAdmin") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(reference.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(reference);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(reference.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                PrivateType privateType = new PrivateType(typeof(MAObjectHologram));
                PrivateObject privateObject = new PrivateObject(shadowObject, privateType);
                MAObjectHologram_Accessor privateShadowObject = new MAObjectHologram_Accessor(privateObject);

                privateShadowObject.Delete(true);

                if (privateShadowObject.DeletedTimestamp == 0)
                {
                    Assert.Fail("The shadow object was not deleted");
                }

                sourceObject = UnitTestControl.DataContext.GetMAObjectOrDefault(newId);
                AttributeValue provisioningId = sourceObject.GetSVAttributeValue(reference.ProvisioningAttribute);
                if (provisioningId == true)
                {
                    Assert.Fail("The shadow parent provisioning attribute was not cleared");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }
            }
        }

        [TestMethod()]
        public void TestShadowDeprovisioningByParentDeprovisionTrigger()
        {
            AcmaSchemaShadowObjectLink reference = ActiveConfig.DB.GetObjectClass("person").ShadowChildLinks.FirstOrDefault(t => t.ReferenceAttribute.Name == "adMgrAccount");
            AcmaSchemaObjectClass schemaShadowObject = ActiveConfig.DB.GetObjectClass("shadowAccountAuAdAdmin") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(reference.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(reference);
                sourceObject.CommitCSEntryChange();

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(reference.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.DeprovisionShadowObject(reference);
                sourceObject.CommitCSEntryChange();

                shadowObject = UnitTestControl.DataContext.GetMAObjectOrDefault(shadowId);

                if (shadowObject.DeletedTimestamp == 0)
                {
                    Assert.Fail("The shadow object was not deleted");
                }

                sourceObject = UnitTestControl.DataContext.GetMAObjectOrDefault(newId);
                AttributeValue provisioningId = sourceObject.GetSVAttributeValue(reference.ProvisioningAttribute);
                if (provisioningId == true)
                {
                    Assert.Fail("The shadow parent provisioning attribute was not cleared");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }
            }
        }

        [TestMethod()]
        public void TestShadowDeprovisioningByParentDelete()
        {
            AcmaSchemaShadowObjectLink reference = ActiveConfig.DB.GetObjectClass("person").ShadowChildLinks.FirstOrDefault(t => t.ReferenceAttribute.Name == "adMgrAccount");
            AcmaSchemaObjectClass schemaShadowObject = ActiveConfig.DB.GetObjectClass("shadowAccountAuAdAdmin") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObjectHologram));
                PrivateObject privateSourceObject = new PrivateObject(sourceObject, privateType);
                MAObjectHologram_Accessor targetSourceObject = new MAObjectHologram_Accessor(privateSourceObject);

                sourceObject.SetAttributeValue(reference.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(reference);

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                targetSourceObject.Delete(true);
                shadowObject = UnitTestControl.DataContext.GetMAObjectOrDefault(shadowId);

                if (shadowObject.DeletedTimestamp == 0)
                {
                    Assert.Fail("The shadow object was not deleted");
                }

                sourceObject = UnitTestControl.DataContext.GetMAObjectOrDefault(newId);
                AttributeValue provisioningId = sourceObject.GetSVAttributeValue(reference.ProvisioningAttribute);
                if (provisioningId == true)
                {
                    Assert.Fail("The shadow parent provisioning attribute was not cleared");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }
            }
        }

        [TestMethod()]
        public void TestShadowProvisioningUndelete()
        {
            AcmaSchemaShadowObjectLink reference = ActiveConfig.DB.GetObjectClass("person").ShadowChildLinks.FirstOrDefault(t => t.ReferenceAttribute.Name == "adMgrAccount");
            AcmaSchemaObjectClass schemaShadowObject = ActiveConfig.DB.GetObjectClass("shadowAccountAuAdAdmin") as AcmaSchemaObjectClass;

            Guid newId = Guid.NewGuid();
            Guid shadowId = Guid.Empty;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(reference.ProvisioningAttribute, true);
                MAObjectHologram shadowObject = sourceObject.ProvisionShadowObject(reference);
                sourceObject.CommitCSEntryChange();

                if (shadowObject == null)
                {
                    Assert.Fail("The shadow object was not created");
                }
                else
                {
                    shadowId = shadowObject.Id;
                }

                if (shadowObject.ShadowParentId != sourceObject.Id)
                {
                    Assert.Fail("The shadow object's parent ID was not set correctly");
                }

                AttributeValue shadowChildId = sourceObject.GetSVAttributeValue(reference.ReferenceAttribute);

                if (shadowChildId != shadowObject.Id)
                {
                    Assert.Fail("The shadow parent reference link was not set correctly");
                }

                shadowObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                shadowObject.CommitCSEntryChange();

                if (shadowObject.DeletedTimestamp == 0)
                {
                    Assert.Fail("The shadow object was not deleted");
                }

                sourceObject = UnitTestControl.DataContext.GetMAObjectOrDefault(newId);
                AttributeValue provisioningId = sourceObject.GetSVAttributeValue(reference.ProvisioningAttribute);
                if (provisioningId == true)
                {
                    Assert.Fail("The shadow parent provisioning attribute was not cleared");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(reference.ProvisioningAttribute, true);
                sourceObject.CommitCSEntryChange();
                shadowObject = sourceObject.ProvisionShadowObject(reference);

                if (shadowObject.Id != shadowId)
                {
                    Assert.Fail("The provisioning process created a new object instead of undeleting the old object");
                }

                if (shadowObject.DeletedTimestamp > 0)
                {
                    Assert.Fail("The provisioning process reconnected the old object, but didn't mark it as undeleted");
                }

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                if (shadowId != Guid.Empty)
                {
                    UnitTestControl.DataContext.DeleteMAObjectPermanent(shadowId);
                }
            }
        }

        [TestMethod()]
        public void TestReferenceLinkCreateSingle()
        {
            Guid childId = Guid.NewGuid();
            Guid supervisorId = Guid.NewGuid();

            try
            {
                MAObjectHologram childObject = UnitTestControl.DataContext.CreateMAObject(childId, "person");

                MAObjectHologram supervisorObject = UnitTestControl.DataContext.CreateMAObject(supervisorId, "person");

                childObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                childObject.CommitCSEntryChange();

                supervisorObject = UnitTestControl.DataContext.GetMAObjectOrDefault(supervisorId);

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.All(t => t.ValueGuid == childId))
                {
                    Assert.Fail("The reference link was not created");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(childId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(supervisorId);
            }
        }

        [TestMethod()]
        public void TestReferenceLinkCreateMulti()
        {
            Guid child1Id = Guid.NewGuid();
            Guid child2Id = Guid.NewGuid();
            Guid supervisorId = Guid.NewGuid();

            try
            {
                MAObjectHologram child1Object = UnitTestControl.DataContext.CreateMAObject(child1Id, "person");
                MAObjectHologram child2Object = UnitTestControl.DataContext.CreateMAObject(child2Id, "person");
                MAObjectHologram supervisorObject = UnitTestControl.DataContext.CreateMAObject(supervisorId, "person");

                child1Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                child1Object.CommitCSEntryChange();

                child2Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                child2Object.CommitCSEntryChange();

                supervisorObject = UnitTestControl.DataContext.GetMAObjectOrDefault(supervisorId);

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.Any(t => t.ValueGuid == child1Id))
                {
                    Assert.Fail("The reference link for child object 1 was not created");
                }


                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.Any(t => t.ValueGuid == child2Id))
                {
                    Assert.Fail("The reference link for child object 2 was not created");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(supervisorId);
            }
        }

        [TestMethod()]
        public void TestReferenceLinkDeleteSingle()
        {
            Guid childId = Guid.NewGuid();
            Guid supervisorId = Guid.NewGuid();

            try
            {
                MAObjectHologram childObject = UnitTestControl.DataContext.CreateMAObject(childId, "person");
                MAObjectHologram supervisorObject = UnitTestControl.DataContext.CreateMAObject(supervisorId, "person");

                childObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                childObject.CommitCSEntryChange();

                supervisorObject = UnitTestControl.DataContext.GetMAObjectOrDefault(supervisorId);

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.All(t => t.ValueGuid == childId))
                {
                    Assert.Fail("The reference link was not created");
                }

                childObject.SetObjectModificationType(ObjectModificationType.Update, false);
                childObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), null);
                childObject.CommitCSEntryChange();

                supervisorObject = UnitTestControl.DataContext.GetMAObjectOrDefault(supervisorId);

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).IsEmptyOrNull)
                {
                    Assert.Fail("The reference link was not deleted");
                }

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(childId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(supervisorId);
            }
        }

        [TestMethod()]
        public void TestReferenceLinkDeleteMulti()
        {
            Guid child1Id = Guid.NewGuid();
            Guid child2Id = Guid.NewGuid();
            Guid child3Id = Guid.NewGuid();
            Guid supervisorId = Guid.NewGuid();

            try
            {
                MAObjectHologram child1Object = UnitTestControl.DataContext.CreateMAObject(child1Id, "person");
                MAObjectHologram child2Object = UnitTestControl.DataContext.CreateMAObject(child2Id, "person");
                MAObjectHologram child3Object = UnitTestControl.DataContext.CreateMAObject(child3Id, "person");
                MAObjectHologram supervisorObject = UnitTestControl.DataContext.CreateMAObject(supervisorId, "person");

                child1Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                child1Object.CommitCSEntryChange();

                child2Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                child2Object.CommitCSEntryChange();

                child3Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), supervisorId);
                child3Object.CommitCSEntryChange();

                supervisorObject = UnitTestControl.DataContext.GetMAObjectOrDefault(supervisorId);

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.Any(t => t.ValueGuid == child1Id))
                {
                    Assert.Fail("The reference link for child object 1 was not created");
                }

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.Any(t => t.ValueGuid == child2Id))
                {
                    Assert.Fail("The reference link for child object 2 was not created");
                }

                if (!supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.Any(t => t.ValueGuid == child3Id))
                {
                    Assert.Fail("The reference link for child object 3 was not created");
                }

                child1Object.SetObjectModificationType(ObjectModificationType.Update, false);
                child1Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), null);
                child1Object.CommitCSEntryChange();

                child3Object.SetObjectModificationType(ObjectModificationType.Update, false);
                child3Object.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), null);
                child3Object.CommitCSEntryChange();

                supervisorObject = UnitTestControl.DataContext.GetMAObjectOrDefault(supervisorId);

                if (supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values.Count != 1)
                {
                    Assert.Fail("The reference contains the incorrect number of links");
                }

                if (supervisorObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).Values[0].ValueGuid != child2Id)
                {
                    Assert.Fail("The incorrect reference link was deleted");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child3Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(supervisorId);
            }
        }

        [TestMethod()]
        public void TestReferenceLinkCreateTwoWay()
        {
            Guid linkedAccount1Id = Guid.NewGuid();
            Guid linkedAccount2Id = Guid.NewGuid();
            Guid linkedAccount3Id = Guid.NewGuid();

            try
            {
                MAObjectHologram linkedAccount1 = UnitTestControl.DataContext.CreateMAObject(linkedAccount1Id, "person");
                linkedAccount1.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "Account1");
                linkedAccount1.CommitCSEntryChange();

                MAObjectHologram linkedAccount2 = UnitTestControl.DataContext.CreateMAObject(linkedAccount2Id, "person");
                linkedAccount2.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "Account2");
                linkedAccount2.CommitCSEntryChange();

                MAObjectHologram linkedAccount3 = UnitTestControl.DataContext.CreateMAObject(linkedAccount3Id, "person");
                linkedAccount3.SetAttributeValue(ActiveConfig.DB.GetAttribute("displayName"), "account3");
                linkedAccount3.CommitCSEntryChange();

                linkedAccount1.SetObjectModificationType(ObjectModificationType.Update, false);
                linkedAccount1.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("linkedAccounts"), new List<ValueChange>() { ValueChange.CreateValueAdd(linkedAccount3Id) });
                linkedAccount1.CommitCSEntryChange();

                linkedAccount2.SetObjectModificationType(ObjectModificationType.Update, false);
                linkedAccount2.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("linkedAccounts"), new List<ValueChange>() { ValueChange.CreateValueAdd(linkedAccount1Id) });
                linkedAccount2.CommitCSEntryChange();

                linkedAccount3.SetObjectModificationType(ObjectModificationType.Update, false);
                linkedAccount3.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("linkedAccounts"), new List<ValueChange>() { ValueChange.CreateValueAdd(linkedAccount2Id) });
                linkedAccount3.CommitCSEntryChange();


                linkedAccount1 = UnitTestControl.DataContext.GetMAObjectOrDefault(linkedAccount1Id);
                linkedAccount2 = UnitTestControl.DataContext.GetMAObjectOrDefault(linkedAccount2Id);
                linkedAccount3 = UnitTestControl.DataContext.GetMAObjectOrDefault(linkedAccount3Id);

                AttributeValues linkedAccount1Values = linkedAccount1.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("linkedAccounts"));
                AttributeValues linkedAccount2Values = linkedAccount2.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("linkedAccounts"));
                AttributeValues linkedAccount3Values = linkedAccount3.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("linkedAccounts"));

                if (!linkedAccount1Values.ContainsAllElements(new List<object>() { linkedAccount2Id, linkedAccount3Id }))
                {
                    Assert.Fail("Account 1 does not have the correct reference links");
                }

                if (!linkedAccount2Values.ContainsAllElements(new List<object>() { linkedAccount1Id, linkedAccount3Id }))
                {
                    Assert.Fail("Account 2 does not have the correct reference links");
                }

                if (!linkedAccount3Values.ContainsAllElements(new List<object>() { linkedAccount2Id, linkedAccount1Id }))
                {
                    Assert.Fail("Account 3 does not have the correct reference links");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(linkedAccount1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(linkedAccount2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(linkedAccount3Id);
            }
        }

        [TestMethod()]
        public void TestCSEntryChangeReplaceSVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Replace, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Replace, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateSVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateSVAttributeWithValueDelete()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com"), ValueChange.CreateValueDelete("test.test@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateSVAttributeWithValueDeleteOnNonExistientValue()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com"), ValueChange.CreateValueDelete("test5.test5@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateSVAttributeWithOnlyValueDelete()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueDelete("test.test@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>();
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Delete, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Delete, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeDeleteSVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>();
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Delete, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Delete, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeAddSVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Add, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Add, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeAddMVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Add, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Add, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateMVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test.test@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateMVAttributeWithValueDelete()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test4.test4@test.com"), ValueChange.CreateValueDelete("test1.test1@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test.test@test.com", "test4.test4@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateMVAttributeWithValueDeleteNonExistentValue()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test4.test4@test.com"), ValueChange.CreateValueDelete("test1.test1@test.com"), ValueChange.CreateValueDelete("test7.test7@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test.test@test.com", "test4.test4@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeUpdateMVAttributeWithDeleteAllValues()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueDelete("test1.test1@test.com"), ValueChange.CreateValueDelete("test.test@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<object> expectedValues = new List<object>();
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Update, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeReplaceMVAttribute()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com"), ValueChange.CreateValueAdd("test4.test4@test.com") };
            IList<object> startValues = new List<object>() { "test.test@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test4.test4@test.com" };
            this.TestAttachedCSEntryChangeAttributeChange(testAttribute, AttributeModificationType.Replace, startValues, valueChanges, expectedValues);
            this.TestDirectAttributeChange(testAttribute, AttributeModificationType.Replace, startValues, valueChanges, expectedValues);
        }

        [TestMethod()]
        public void TestValueChangeMergeAddDelete()
        {
            Guid sourceObjectId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceObjectId, "person");

                List<ValueChange> valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueAdd("value1"));

                sourceObject.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), valueChanges);
                AttributeChange attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");
                
                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Add)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }

                valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueDelete("value1"));
                sourceObject.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), valueChanges);
                attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange != null)
                {
                    Assert.Fail("The attribute change was still present");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceObjectId);
            }
        }

        [TestMethod()]
        public void TestValueChangeMergeDuplicateAdd()
        {
            Guid sourceObjectId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceObjectId, "person");

                List<ValueChange> valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueAdd("value1"));

                sourceObject.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), valueChanges);
                AttributeChange attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Add)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }

                valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueAdd("value1"));
                sourceObject.UpdateAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), valueChanges);
                attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Add)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceObjectId);
            }
        }
        
        [TestMethod()]
        public void TestValueChangeMergeDuplicateDelete()
        {
            Guid sourceObjectId = Guid.NewGuid();
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceObjectId, "person");

                sourceObject.SetAttributeValue(attribute, "value1");
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                List<ValueChange> valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueDelete("value1"));

                sourceObject.UpdateAttributeValue(attribute, valueChanges);
                AttributeChange attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Delete)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }

                valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueDelete("value1"));
                sourceObject.UpdateAttributeValue(attribute, valueChanges);
                attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Delete)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceObjectId);
            }
        }

        [TestMethod()]
        public void TestValueChangeMergeDeleteAdd()
        {
            Guid sourceObjectId = Guid.NewGuid();
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceObjectId, "person");

                sourceObject.SetAttributeValue(attribute, "value1");
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                List<ValueChange> valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueDelete("value1"));

                sourceObject.UpdateAttributeValue(attribute, valueChanges);
                AttributeChange attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Delete)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }

                valueChanges = new List<ValueChange>();
                valueChanges.Add(ValueChange.CreateValueAdd("value1"));
                sourceObject.UpdateAttributeValue(attribute, valueChanges);
                attributeChange = sourceObject.InternalAttributeChanges.FirstOrDefault(t => t.Name == "mailAlternateAddresses");

                if (attributeChange == null)
                {
                    Assert.Fail("The attribute change was not present");
                }
                else
                {
                    if (attributeChange.ValueChanges.Count != 1)
                    {
                        Assert.Fail("The incorrect number of value changes was present");
                    }

                    ValueChange change = attributeChange.ValueChanges.FirstOrDefault();

                    if (change == null)
                    {
                        Assert.Fail("The value change was not present");
                    }
                    else
                    {
                        if (change.ModificationType != ValueModificationType.Add)
                        {
                            Assert.Fail("The incorrect modification type was found");
                        }

                        if ((string)change.Value != "value1")
                        {
                            Assert.Fail("The incorrect value was found");
                        }
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceObjectId);
            }
        }

        [TestMethod]
        public void TestSVDereferenceOnObjectDelete()
        {
            Guid referencedObjectID = Guid.NewGuid();
            Guid referencingObjectID = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");
            AcmaSchemaAttribute refAttribute = ActiveConfig.DB.GetAttribute("supervisor");

            try
            {
                MAObjectHologram referencedObject = UnitTestControl.DataContext.CreateMAObject(referencedObjectID, objectClass);
                referencedObject.CommitCSEntryChange();

                MAObjectHologram referencingObject = UnitTestControl.DataContext.CreateMAObject(referencingObjectID, objectClass);
                referencingObject.SetAttributeValue(refAttribute, referencedObjectID.ToString());
                referencingObject.CommitCSEntryChange();

                if (referencingObject.GetSVAttributeValue(refAttribute).ValueGuid != referencedObjectID)
                {
                    Assert.Fail("The referenced object was not set");
                }

                referencedObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                referencedObject.CommitCSEntryChange();

                referencingObject = UnitTestControl.DataContext.GetMAObject(referencingObjectID, objectClass);

                if (!referencingObject.GetSVAttributeValue(refAttribute).IsNull)
                {
                    Assert.Fail("The referenced object was not cleared");
                }

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(referencedObjectID);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(referencingObjectID);
            }
        }

        [TestMethod]
        public void TestMVDereferenceOnObjectDelete()
        {
            Guid referencedObjectID = Guid.NewGuid();
            Guid referencingObjectID = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");
            AcmaSchemaAttribute refAttribute = ActiveConfig.DB.GetAttribute("directReports");

            try
            {
                MAObjectHologram referencedObject = UnitTestControl.DataContext.CreateMAObject(referencedObjectID, objectClass);
                referencedObject.CommitCSEntryChange();

                MAObjectHologram referencingObject = UnitTestControl.DataContext.CreateMAObject(referencingObjectID, objectClass);
                referencingObject.SetAttributeValue(refAttribute, referencedObjectID.ToString());
                referencingObject.CommitCSEntryChange();

                if (referencingObject.GetMVAttributeValues(refAttribute).First().ValueGuid != referencedObjectID)
                {
                    Assert.Fail("The referenced object was not set");
                }

                referencedObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                referencedObject.CommitCSEntryChange();

                referencingObject = UnitTestControl.DataContext.GetMAObject(referencingObjectID, objectClass);

                if (!referencingObject.GetMVAttributeValues(refAttribute).IsEmptyOrNull)
                {
                    Assert.Fail("The referenced object was not cleared");
                }

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(referencedObjectID);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(referencingObjectID);
            }
        }

        [TestMethod()]
        public void TestRaiseEvent()
        {
            Guid sourceObjectId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceObjectId, "person");

                PrivateType privateType = new PrivateType(typeof(MAObjectHologram));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObjectHologram_Accessor target = new MAObjectHologram_Accessor(privateObject);

                ClassConstructor personConstructor = new ClassConstructor();
                AcmaInternalExitEvent newevent = new AcmaInternalExitEvent();
                newevent.ID = "usernameChanged";
                newevent.Recipients = new System.Collections.ObjectModel.ObservableCollection<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("supervisor") };
                newevent.RuleGroup = new RuleGroup();
                newevent.RuleGroup.Items = new System.Collections.ObjectModel.ObservableCollection<RuleObject>();
                AttributeChangeRule rule = new AttributeChangeRule() { Attribute = ActiveConfig.DB.GetAttribute("accountName"), TriggerEvents = TriggerEvents.Add | TriggerEvents.Update };
                newevent.RuleGroup.Items.Add(rule);
                personConstructor.ExitEvents.Add(newevent);

                target.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "test");

                List<RaisedEvent> events = target.CreateExitEvents(personConstructor);

                foreach (RaisedEvent raisedEvent in events)
                {
                    if (raisedEvent.Source != sourceObject)
                    {
                        Assert.Fail("The event source was incorrect");
                    }

                    if (raisedEvent.EventID != "usernameChanged")
                    {
                        Assert.Fail("The wrong event was raised: " + raisedEvent.EventID);
                    }
                }


            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceObjectId);
            }
        }

        private void TestAttachedCSEntryChangeAttributeChange(AcmaSchemaAttribute testAttribute, AttributeModificationType modificationType, IList<object> startValues, IList<ValueChange> valueChanges, IList<object> expectedValues)
        {
            Guid newId = Guid.NewGuid();
            IList<object> addValues = valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

            try
            {
                // Set initial value
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(testAttribute, startValues);
                sourceObject.CommitCSEntryChange();

                // Set new value
                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = newId.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;

                switch (modificationType)
                {
                    case AttributeModificationType.Add:
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, addValues));
                        break;

                    case AttributeModificationType.Delete:
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(testAttribute.Name));
                        break;

                    case AttributeModificationType.Replace:
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(testAttribute.Name, addValues));
                        break;

                    case AttributeModificationType.Update:
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(testAttribute.Name, valueChanges));
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new UnknownOrUnsupportedModificationTypeException();
                }

                sourceObject.AttachCSEntryChange(csentry);

                // Test uncommitted value
                if (testAttribute.IsMultivalued)
                {
                    if (!sourceObject.GetMVAttributeValues(testAttribute).ContainsAllElements(expectedValues))
                    {
                        Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                    }
                }
                else
                {
                    if (modificationType == AttributeModificationType.Delete)
                    {
                        if (!sourceObject.GetSVAttributeValue(testAttribute).IsNull)
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                    else
                    {
                        if (sourceObject.GetSVAttributeValue(testAttribute) != expectedValues.FirstOrDefault())
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                }

                // Test committed value
                sourceObject.CommitCSEntryChange();
                if (testAttribute.IsMultivalued)
                {
                    if (!sourceObject.GetMVAttributeValues(testAttribute).ContainsAllElements(expectedValues))
                    {
                        Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                    }
                }
                else
                {
                    if (modificationType == AttributeModificationType.Delete)
                    {
                        if (!sourceObject.GetSVAttributeValue(testAttribute).IsNull)
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                    else
                    {
                        if (sourceObject.GetSVAttributeValue(testAttribute) != expectedValues.FirstOrDefault())
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        private void TestDirectAttributeChange(AcmaSchemaAttribute testAttribute, AttributeModificationType modificationType, IList<object> startValues, IList<ValueChange> valueChanges, IList<object> expectedValues)
        {
            Guid newId = Guid.NewGuid();
            IList<object> addValues = valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

            try
            {
                // Set initial value
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(testAttribute, startValues);
                sourceObject.CommitCSEntryChange();

                // Set new value
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                switch (modificationType)
                {
                    case AttributeModificationType.Add:
                        sourceObject.SetAttributeValue(testAttribute, addValues);
                        break;

                    case AttributeModificationType.Delete:
                        sourceObject.DeleteAttribute(testAttribute);
                        break;

                    case AttributeModificationType.Replace:
                        sourceObject.SetAttributeValue(testAttribute, addValues);
                        break;

                    case AttributeModificationType.Update:
                        sourceObject.UpdateAttributeValue(testAttribute, valueChanges);
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new UnknownOrUnsupportedModificationTypeException();
                }

                // Test uncommitted value
                if (testAttribute.IsMultivalued)
                {
                    if (!sourceObject.GetMVAttributeValues(testAttribute).ContainsAllElements(expectedValues))
                    {
                        Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                    }
                }
                else
                {
                    if (modificationType == AttributeModificationType.Delete)
                    {
                        if (!sourceObject.GetSVAttributeValue(testAttribute).IsNull)
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                    else
                    {
                        if (sourceObject.GetSVAttributeValue(testAttribute) != expectedValues.FirstOrDefault())
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                }

                // Test committed value
                sourceObject.CommitCSEntryChange();
                if (testAttribute.IsMultivalued)
                {
                    if (!sourceObject.GetMVAttributeValues(testAttribute).ContainsAllElements(expectedValues))
                    {
                        Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                    }
                }
                else
                {
                    if (modificationType == AttributeModificationType.Delete)
                    {
                        if (!sourceObject.GetSVAttributeValue(testAttribute).IsNull)
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                    else
                    {
                        if (sourceObject.GetSVAttributeValue(testAttribute) != expectedValues.FirstOrDefault())
                        {
                            Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                        }
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        private void CommitSVAttribute(AcmaSchemaAttribute testAttribute, object testValue)
        {
            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);
                sourceObject.SetAttributeValue(testAttribute, new List<object>() { testValue });

                // Test uncommitted value
                if (sourceObject.GetSVAttributeValue(testAttribute) != testValue)
                {
                    Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                }

                // Test committed value
                sourceObject.CommitCSEntryChange();
                if (sourceObject.GetSVAttributeValue(testAttribute) != testValue)
                {
                    Assert.Fail("The MAObjectHologram failed to commit the attribute change");
                }

                // Test underlying database value
                if (target.DBGetSVAttributeValue(testAttribute) != testValue)
                {
                    Assert.Fail("The MAObjectHologram failed to commit the attribute change to the database");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        private void CommitMVAttribute(AcmaSchemaAttribute testAttribute, IList<object> testValues)
        {
            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);
                sourceObject.SetAttributeValue(testAttribute, testValues);

                // Test uncommitted value
                if (!sourceObject.GetMVAttributeValues(testAttribute).ContainsAllElements(testValues))
                {
                    Assert.Fail("The MAObjectHologram failed to record the attribute change in the CSEntryChange");
                }

                // Test committed value
                sourceObject.CommitCSEntryChange();
                if (!sourceObject.GetMVAttributeValues(testAttribute).ContainsAllElements(testValues))
                {
                    Assert.Fail("The MAObjectHologram failed to commit the attribute change");
                }

                // Test underlying database value
                if (!target.DBGetAttributeValues(testAttribute).ContainsAllElements(testValues))
                {
                    Assert.Fail("The MAObjectHologram failed to commit the attribute change to the database");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
