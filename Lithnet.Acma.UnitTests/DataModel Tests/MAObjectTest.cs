
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

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for MAObjectHologramTest and is intended
    ///to contain all MAObjectHologramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MAObjectTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        /// <summary>
        ///A test for Rollback
        ///</summary>
        [TestMethod()]
        public void RollbackSVAttributeChange()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject); 

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                if (target.DBGetAttributeValues(testAttribute).First().ValueString != "test.test@test.com")
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test1.test1@test.com") });

                if (target.DBGetAttributeValues(testAttribute).First().ValueString != "test1.test1@test.com")
                {
                    Assert.Fail("The MAObject failed to record the pending attribute change");
                }

                target.Rollback();

                if (target.DBGetAttributeValues(testAttribute).First().ValueString != "test.test@test.com")
                {
                    Assert.Fail("The MAObject failed to rollback the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void RollbackMVAttributeChange()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { 
                        ValueChange.CreateValueAdd("1test.test@test.com"),
                        ValueChange.CreateValueAdd("2test1.test1@test.com") ,
                        ValueChange.CreateValueAdd("3test2.test2@test.com") });

                target.Commit();
                if (!target.DBGetAttributeValues(testAttribute).Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object>{
                    "1test.test@test.com",
                    "2test1.test1@test.com",
                    "3test2.test2@test.com"}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { 
                        ValueChange.CreateValueAdd("test1.test1@test.com"),
                        ValueChange.CreateValueDelete("1test.test@test.com"),
                        ValueChange.CreateValueDelete("2test1.test1@test.com") ,
                        ValueChange.CreateValueDelete("3test2.test2@test.com") });

                if (!target.DBGetAttributeValues(testAttribute).Values.SequenceEqual(new List<object>() { "test1.test1@test.com" }))
                {
                    Assert.Fail("The MAObject failed to record the pending attribute change");
                }

                target.Rollback();
                if (!target.DBGetAttributeValues(testAttribute).Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object>{
                    "1test.test@test.com",
                    "2test1.test1@test.com",
                    "3test2.test2@test.com"}))
                {
                    Assert.Fail("The MAObject failed to rollback the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }
        
        [TestMethod()]
        public void CommitSVAttributeChange()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject); 

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();
                
                if (target.DBGetSVAttributeValue(testAttribute).ValueString != "test.test@test.com")
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeChange()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(
                    testAttribute, 
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd("1test.test@test.com"),
                        ValueChange.CreateValueAdd("2test1.test1@test.com") ,
                        ValueChange.CreateValueAdd("3test2.test2@test.com") });

                target.Commit();
                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object>{
                    "1test.test@test.com",
                    "2test1.test1@test.com",
                    "3test2.test2@test.com"}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeChangeDuplicateValue()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(
                    testAttribute,
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd("1test.test@test.com"),
                        ValueChange.CreateValueAdd("2test1.test1@test.com") ,
                        ValueChange.CreateValueAdd("2test1.test1@test.com") });

                target.Commit();
                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object>{
                    "1test.test@test.com",
                    "2test1.test1@test.com"}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasSVAttributeUncommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });

                if (!target.DBHasAttribute(testAttribute))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasSVAttributeCommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                if (!target.DBHasAttribute(testAttribute))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }
        
        [TestMethod()]
        public void HasSVAttributeValueUncommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });

                if (!target.DBHasAttributeValue(testAttribute, "test.test@test.com"))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasSVAttributeValueCommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                if (!target.DBHasAttributeValue(testAttribute, "test.test@test.com"))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasMVAttributeUncommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });

                if (!target.DBHasAttribute(testAttribute))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasMVAttributeCommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                if (!target.DBHasAttribute(testAttribute))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasMVAttributeValueUncommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });

                if (!target.DBHasAttributeValue(testAttribute, "test1.test1@test.com"))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }

                if (!target.DBHasAttributeValue(testAttribute, "test.test@test.com"))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void HasMVAttributeValueCommitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);
                
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.Commit();

                if (!target.DBHasAttributeValue(testAttribute, "test1.test1@test.com"))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }

                if (!target.DBHasAttributeValue(testAttribute, "test.test@test.com"))
                {
                    Assert.Fail("The MAObject failed to record the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }
        

        [TestMethod()]
        public void DeleteSVAttributeUncomitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });

                target.DBDeleteAttribute(testAttribute);

                if (!target.DBGetSVAttributeValue(testAttribute).IsNull)
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteSVAttributeComitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();
                
                target.DBDeleteAttribute(testAttribute);
                target.Commit();

                if (!target.DBGetSVAttributeValue(testAttribute).IsNull)
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteMVAttributeUncomitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.DBDeleteAttribute(testAttribute);

                if (target.DBGetAttributeValues(testAttribute).Values.Count > 0)
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteMVAttributeComitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.Commit();
                target.DBDeleteAttribute(testAttribute);
                target.Commit();

                if (target.DBGetAttributeValues(testAttribute).Values.Count > 0)
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DeleteMVAttributeValueComitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.Commit();
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test.test@test.com")});
                target.Commit();

                if (!target.DBGetAttributeValues(testAttribute).Values.SequenceEqual(new List<object> { "test1.test1@test.com"}))
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteMVAttributeValueUncomitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test.test@test.com") });

                if (!target.DBGetAttributeValues(testAttribute).Values.SequenceEqual(new List<object> { "test1.test1@test.com" }))
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DeleteSVAttributeValueUncomitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test.test@test.com") });
                target.DBDeleteAttribute(testAttribute);

                if (!target.DBGetSVAttributeValue(testAttribute).IsNull)
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteSVAttributeValueComitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test.test@test.com") });
                target.Commit();

                if (!target.DBGetSVAttributeValue(testAttribute).IsNull)
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteSVAttributeValueNotExistentComitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test1.test1@test.com") });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != "test.test@test.com")
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteSVAttributeValueNotExistentUncomitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test1.test1@test.com") });

                if (target.DBGetSVAttributeValue(testAttribute) != "test.test@test.com")
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DeleteMVAttributeValueNotExistentComitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.Commit();
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test2.test2@test.com") });
                target.Commit();

                if (!target.DBGetAttributeValues(testAttribute).Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object> { "test.test@test.com", "test1.test1@test.com" }))
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DeleteMVAttributeValueNotExistentUncomitted()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") });
                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete("test2.test2@test.com") });

                if (!target.DBGetAttributeValues(testAttribute).Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object> { "test.test@test.com", "test1.test1@test.com" }))
                {
                    Assert.Fail("The MAObject failed to delete the attribute");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void GetSVAttributeInherited()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("supervisorAccountName");
            AcmaSchemaAttribute referenceAttribute = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute targetAttribute = ActiveConfig.DB.GetAttribute("accountName");

            Guid parentId = Guid.NewGuid();
            Guid childId = Guid.NewGuid();

            try
            {
                MAObjectHologram parentObject = MAObjectHologram.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(targetAttribute, "myaccountName");
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(childId, "person");
                sourceObject.SetAttributeValue(referenceAttribute, parentId);
                sourceObject.CommitCSEntryChange();

                if (sourceObject.GetSVAttributeValue(testAttribute).ValueString != "myaccountName")
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(parentId);
                MAObjectHologram.DeleteMAObjectPermanent(childId);

            }
        }

        [TestMethod()]
        public void GetMVAttributeInherited()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("peers");
            AcmaSchemaAttribute referenceAttribute = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute targetAttribute = ActiveConfig.DB.GetAttribute("directReports");

          //<attribute inherit-from="supervisor" inherit-from-attribute="directReports">peers</attribute>

            //ActiveConfig.DB.CreateMapping(ActiveConfig.DB.GetObjectClass("person"), testAttribute, referenceAttribute, targetAttribute);

            Guid parentId = Guid.NewGuid();
            Guid childId = Guid.NewGuid();
            List<object> references = new List<object>() { new Guid("14075f8e-f918-4614-8a85-51af25faf582"), new Guid("24075f8e-f918-4614-8a85-51af25faf582"), childId };
            try
            {
                MAObjectHologram parentObject = MAObjectHologram.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(targetAttribute, references);
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(childId, "person");
                sourceObject.SetAttributeValue(referenceAttribute, parentId);
                sourceObject.CommitCSEntryChange();

                AttributeValues values = sourceObject.GetMVAttributeValues(testAttribute);

                if (!values.ContainsAllElements(references))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(parentId);
                MAObjectHologram.DeleteMAObjectPermanent(childId);

            }
        }



        [TestMethod()]
        public void CommitSVAttributeString()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mail");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != "test.test@test.com")
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitSVAttributeLong()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("unixUid");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(55L) });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != 55L)
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitSVAttributeBoolean()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(true) });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != true)
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitSVAttributeBinary()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("objectSid");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(new byte[] {0,1,2,3,4}) });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != new byte[] {0,1,2,3,4})
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitSVAttributeReference()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("supervisor");
            Guid newId = Guid.NewGuid();
            Guid reference = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(reference) });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != reference)
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitSVAttributeDateTime()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(DateTime.Parse("2010-01-01")) });
                target.Commit();

                if (target.DBGetSVAttributeValue(testAttribute) != DateTime.Parse("2010-01-01"))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeString()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);
                
                target.DBUpdateAttribute(
                    testAttribute,
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd("1test.test@test.com"),
                        ValueChange.CreateValueAdd("2test1.test1@test.com") ,
                        ValueChange.CreateValueAdd("3test2.test2@test.com") });

                target.Commit();
                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.OrderBy(t => t.ValueString).SequenceEqual(new List<object>{
                    "1test.test@test.com",
                    "2test1.test1@test.com",
                    "3test2.test2@test.com"}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeLong()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("expiryDates");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(
                    testAttribute,
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd(5L),
                        ValueChange.CreateValueAdd(10L) ,
                        ValueChange.CreateValueAdd(15L) });

                target.Commit();
                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.OrderBy(t => t.ValueLong).SequenceEqual(new List<object>{
                    5L,
                    10L,
                    15L}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeDateTime()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("dateTimeMV");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(
                    testAttribute,
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd(DateTime.Parse("2010-01-01")),
                        ValueChange.CreateValueAdd(DateTime.Parse("2011-01-01")) ,
                        ValueChange.CreateValueAdd(DateTime.Parse("2012-01-01")) });

                target.Commit();
                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.OrderBy(t => t.ValueDateTime).SequenceEqual(new List<object>{
                    DateTime.Parse("2010-01-01"),
                    DateTime.Parse("2011-01-01"),
                    DateTime.Parse("2012-01-01")}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeBinary()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("objectSids");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(
                    testAttribute,
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd(new byte[] {0,1,2,3,4}),
                        ValueChange.CreateValueAdd(new byte[] {1,2,3,4,5}) ,
                        ValueChange.CreateValueAdd(new byte[] {2,3,4,5,6}) });
                target.Commit();

                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.SequenceEqual(new List<object>{
                    new byte[] {0,1,2,3,4},
                    new byte[] {1,2,3,4,5},
                    new byte[] {2,3,4,5,6}}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void CommitMVAttributeReference()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("directReports");
            Guid newId = Guid.NewGuid();
            Guid ref1 = new Guid("1937e7a5-b249-465e-b1cc-a8547556d681");
            Guid ref2 = new Guid("2937e7a5-b249-465e-b1cc-a8547556d681");
            Guid ref3 = new Guid("3937e7a5-b249-465e-b1cc-a8547556d681");

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);
                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                target.DBUpdateAttribute(
                    testAttribute,
                    new List<ValueChange>() { 
                        ValueChange.CreateValueAdd(ref1),
                        ValueChange.CreateValueAdd(ref2) ,
                        ValueChange.CreateValueAdd(ref3) });
                target.Commit();

                AttributeValues values = target.DBGetAttributeValues(testAttribute);

                if (!values.Values.OrderBy(t => t.ValueGuid).SequenceEqual(new List<object>{
                    ref1,
                    ref2,
                    ref3}))
                {
                    Assert.Fail("The MAObject failed to commit the attribute change");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ThrowExceptionOnInheritedSVAttributeModfication()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("supervisorAccountName");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);

                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                try
                {
                    target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                    Assert.Fail("The MAObject did not throw an exception on an inherted value modification");
                }
                catch (InheritedValueModificationException)
                {
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ThrowExceptionOnInheritedMVAttributeModfication()
        {
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("peers");
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                PrivateType privateType = new PrivateType(typeof(MAObject));
                PrivateObject privateObject = new PrivateObject(sourceObject, privateType);

                MAObject_Accessor target = new MAObjectHologram_Accessor(privateObject);

                try
                {
                    target.DBUpdateAttribute(testAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com") });
                    Assert.Fail("The MAObject did not throw an exception on an inherted value modification");
                }
                catch (InheritedValueModificationException)
                {
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
