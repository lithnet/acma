
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.Text.RegularExpressions;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    [TestClass]
    public class SafetyRuleTest
    {
        public SafetyRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestUnsafeSingleValue()
        {
            SafetyRule rule = new SafetyRule();
            rule.Pattern = @"^fimtest\+.+@.+\..+$";
            rule.AcmaSchemaMapping = ActiveConfig.DB.Mappings.First(t => t.Attribute.Name == "mail" && t.ObjectClass.Name == "person");
            rule.NullAllowed = true;

            AttributeValue value = new AttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

            try
            {
                rule.Validate(value);
                Assert.Fail("The rule did not fail the validation attempt");
            }
            catch (SafetyRuleViolationException)
            {
            }

            value = new AttributeValue(ActiveConfig.DB.GetAttribute("mail"), "fimtest+test.test@test.com");
            rule.Validate(value);
        }

        [TestMethod()]
        public void TestUnsafeNullValue()
        {
            SafetyRule rule = new SafetyRule();
            rule.Pattern = @"^fimtest\+.+@.+\..+$";
            rule.AcmaSchemaMapping = ActiveConfig.DB.Mappings.First(t => t.Attribute.Name == "mail" && t.ObjectClass.Name == "person");
            rule.NullAllowed = false;

            AttributeValue value = new AttributeValue(ActiveConfig.DB.GetAttribute("mail"), null);

            try
            {
                rule.Validate(value);
                Assert.Fail("The rule did not fail the validation attempt");
            }
            catch (SafetyRuleViolationException)
            {
            }

            value = new AttributeValue(ActiveConfig.DB.GetAttribute("mail"), "fimtest+test.test@test.com");
            rule.Validate(value);
        }

        [TestMethod()]
        public void TestUnsafeMultivalue()
        {
            SafetyRule rule = new SafetyRule();
            rule.Pattern = @"^fimtest\+.+@.+\..+$";
            rule.AcmaSchemaMapping = ActiveConfig.DB.Mappings.First(t => t.Attribute.Name == "mailAlternateAddresses" && t.ObjectClass.Name == "person");
            rule.NullAllowed = false;

            AttributeValues values = new InternalAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "fimtest+test2@test.com", "test.test@test.com" });

            try
            {
                rule.Validate(values);
                Assert.Fail("The rule did not fail the validation attempt");
            }
            catch (SafetyRuleViolationException)
            {
            }

            values = new InternalAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "fimtest+test.test@test.com", "fimtest+test2@test.com" });
            rule.Validate(values);
        }
    }
}
