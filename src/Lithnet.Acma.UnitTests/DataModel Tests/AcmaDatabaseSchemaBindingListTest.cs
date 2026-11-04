using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Acma.DataModel;
using Lithnet.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    /// Tests that schema objects added through an IBindingList can be created one after another on the same AcmaDatabase instance.
    /// Each test opens its own AcmaDatabase, creates the schema objects it needs, and deletes them again with fresh instances.
    /// </summary>
    [TestClass()]
    public class AcmaDatabaseSchemaBindingListTest
    {
        private const string ShadowClassName = "bindingListTestShadow";

        private const string ReferenceClassName = "bindingListTestReference";

        private const string MappingClassName = "bindingListTestMapping";

        public AcmaDatabaseSchemaBindingListTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void CreateTwoShadowLinksOnOneInstance()
        {
            List<string> attributeNames = new List<string>() { "bindingListTestProv1", "bindingListTestRef1", "bindingListTestProv2", "bindingListTestRef2" };

            try
            {
                AcmaDatabase setup = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                setup.CreateAttribute("bindingListTestProv1", ExtendedAttributeType.Boolean, false, AcmaAttributeOperation.ImportExport, false, false);
                setup.CreateAttribute("bindingListTestRef1", ExtendedAttributeType.Reference, false, AcmaAttributeOperation.ImportExport, false, false);
                setup.CreateAttribute("bindingListTestProv2", ExtendedAttributeType.Boolean, false, AcmaAttributeOperation.ImportExport, false, false);
                setup.CreateAttribute("bindingListTestRef2", ExtendedAttributeType.Reference, false, AcmaAttributeOperation.ImportExport, false, false);
                setup.CreateObjectClass(ShadowClassName, setup.GetObjectClass("person"));

                AcmaDatabase db = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                AcmaSchemaObjectClass shadowClass = db.GetObjectClass(ShadowClassName);
                db.CreateShadowLink(shadowClass, db.GetAttribute("bindingListTestProv1"), db.GetAttribute("bindingListTestRef1"), "bindingListTestLink1");
                db.CreateShadowLink(shadowClass, db.GetAttribute("bindingListTestProv2"), db.GetAttribute("bindingListTestRef2"), "bindingListTestLink2");

                AcmaDatabase verify = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                Assert.AreEqual(2, verify.ShadowObjectLinks.Count(t => t.ShadowObjectClass.Name == ShadowClassName));
            }
            finally
            {
                AcmaDatabaseSchemaBindingListTest.DeleteTestSchema(ShadowClassName, attributeNames);
            }
        }

        [TestMethod()]
        public void CreateTwoReferenceLinksOnOneInstance()
        {
            List<string> attributeNames = new List<string>() { "bindingListTestForward1", "bindingListTestBack1", "bindingListTestForward2", "bindingListTestBack2" };

            try
            {
                AcmaDatabase setup = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                AcmaSchemaObjectClass setupClass = setup.CreateObjectClass(ReferenceClassName, false);

                foreach (string name in attributeNames)
                {
                    setup.CreateMapping(setupClass, setup.CreateAttribute(name, ExtendedAttributeType.Reference, false, AcmaAttributeOperation.ImportExport, false, false));
                }

                AcmaDatabase db = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                AcmaSchemaObjectClass objectClass = db.GetObjectClass(ReferenceClassName);
                db.CreateReferenceLink(objectClass, db.GetAttribute("bindingListTestForward1"), objectClass, db.GetAttribute("bindingListTestBack1"));
                db.CreateReferenceLink(objectClass, db.GetAttribute("bindingListTestForward2"), objectClass, db.GetAttribute("bindingListTestBack2"));

                AcmaDatabase verify = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                Assert.AreEqual(2, verify.ReferenceLinks.Count(t => t.ForwardLinkObjectClass.Name == ReferenceClassName));
            }
            finally
            {
                AcmaDatabaseSchemaBindingListTest.DeleteTestSchema(ReferenceClassName, attributeNames);
            }
        }

        [TestMethod()]
        public void CreateTwoMappingsThroughDatabaseBindingListOnOneInstance()
        {
            List<string> attributeNames = new List<string>() { "bindingListTestMapped1", "bindingListTestMapped2" };

            try
            {
                AcmaDatabase setup = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                setup.CreateObjectClass(MappingClassName, false);

                foreach (string name in attributeNames)
                {
                    setup.CreateAttribute(name, ExtendedAttributeType.String, false, AcmaAttributeOperation.ImportExport, false, false);
                }

                AcmaDatabase db = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                AcmaSchemaObjectClass objectClass = db.GetObjectClass(MappingClassName);
                db.CreateMapping(objectClass, db.GetAttribute("bindingListTestMapped1"), db.MappingsBindingList);
                db.CreateMapping(objectClass, db.GetAttribute("bindingListTestMapped2"), db.MappingsBindingList);

                AcmaDatabase verify = AcmaDatabaseSchemaBindingListTest.OpenDatabase();
                Assert.AreEqual(2, verify.GetObjectClass(MappingClassName).Mappings.Count(t => attributeNames.Contains(t.Attribute.Name)));
            }
            finally
            {
                AcmaDatabaseSchemaBindingListTest.DeleteTestSchema(MappingClassName, attributeNames);
            }
        }

        private static AcmaDatabase OpenDatabase()
        {
            return new AcmaDatabase(UnitTestControl.ServerName, UnitTestControl.DatabaseName);
        }

        /// <summary>
        /// Deletes the links on the test class, the class itself (which deletes its mappings) and then the test attributes.
        /// Fresh instances are used so that a failed test cannot leave the cleanup with a binding list in a broken state.
        /// </summary>
        private static void DeleteTestSchema(string objectClassName, IList<string> attributeNames)
        {
            AcmaDatabase db = AcmaDatabaseSchemaBindingListTest.OpenDatabase();

            foreach (AcmaSchemaShadowObjectLink link in db.ShadowObjectLinks.Where(t => t.ShadowObjectClass.Name == objectClassName).ToList())
            {
                db.DeleteShadowLink(link);
            }

            foreach (AcmaSchemaReferenceLink link in db.ReferenceLinks.Where(t => t.ForwardLinkObjectClass.Name == objectClassName).ToList())
            {
                db.DeleteReferenceLink(link);
            }

            if (db.HasObjectClass(objectClassName))
            {
                db.DeleteObjectClass(db.GetObjectClass(objectClassName));
            }

            db = AcmaDatabaseSchemaBindingListTest.OpenDatabase();

            foreach (string name in attributeNames)
            {
                if (db.HasAttribute(name))
                {
                    db.DeleteAttribute(db.GetAttribute(name));
                }
            }
        }
    }
}
