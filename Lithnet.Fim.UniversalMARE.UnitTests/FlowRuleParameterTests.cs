using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Fim.UniversalMARE;
using Lithnet.Fim.Transforms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Fim.Core;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    [TestClass]
    public class FlowRuleParameterTests
    {
        private XmlConfigFile config;

        public FlowRuleParameterTests()
        {
            UniqueIDCache.ClearIdCache();

            this.config = new XmlConfigFile();
            GetDNComponentTransform transform1 = new GetDNComponentTransform();
            transform1.ID = "xform1";
            this.config.Transforms.Add(transform1);

            GetDNComponentTransform transform2 = new GetDNComponentTransform();
            transform2.ID = "xform2";
            this.config.Transforms.Add(transform2);

            MVBooleanToBitmaskTransform transform3 = new MVBooleanToBitmaskTransform();
            transform3.ID = "loopback";
            this.config.Transforms.Add(transform3);

            FlowRuleAlias alias1 = new FlowRuleAlias();
            alias1.Alias = "alias1";
            alias1.FlowRuleDefinition = "csattribute>>xform1>>mvattribute";
            this.config.FlowRuleAliases.Add(alias1);

            FlowRuleAlias alias2 = new FlowRuleAlias();
            alias2.Alias = "alias2";
            alias2.FlowRuleDefinition = "csattribute>>xform2>>mvattribute";
            this.config.FlowRuleAliases.Add(alias2);
        }

        [TestMethod]
        public void TestSingleAttributeJoinFlowRule()
        {
            string flowRuleName = "csattribute>>xform1";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Join);

            CollectionAssert.AreEqual(new List<string>() { "csattribute" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestMultiAttributeJoinFlowRule()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Join);

            CollectionAssert.AreEqual(new List<string>() { "csattribute1", "csattribute2", "csattribute3" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestMultiAttributeMultiTransformJoinFlowRule()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1>>xform2";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Join);

            CollectionAssert.AreEqual(new List<string>() { "csattribute1", "csattribute2", "csattribute3" }, frp.SourceAttributeNames);
            CollectionAssert.AreEqual(new List<string>() { "xform1", "xform2" }, frp.Transforms.Select(t => t.ID).ToList());
        }

        [TestMethod]
        public void TestInvalidJoinFlowRule1()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1>>xform2>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Join);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidJoinFlowRule2()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1<<xform2>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Join);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidJoinFlowRule3()
        {
            string flowRuleName = "csattribute1<<xform1<<xform2";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Join);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestSingleAttributeImportFlowRule()
        {
            string flowRuleName = "csattribute>>xform1>>mvattribute";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);

            Assert.AreEqual("mvattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "csattribute" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestAlias1FlowRule()
        {
            string flowRuleName = "alias1";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);

            Assert.AreEqual("mvattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "csattribute" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestAlias2FlowRule()
        {
            string flowRuleName = "alias2";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);

            Assert.AreEqual("mvattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "csattribute" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform2", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestMultiAttributeImportFlowRule()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1>>mvattribute";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);

            Assert.AreEqual("mvattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "csattribute1", "csattribute2", "csattribute3" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestMultiAttributeMultiTransformImportFlowRule()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1>>xform2>>mvattribute";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);

            Assert.AreEqual("mvattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "csattribute1", "csattribute2", "csattribute3" }, frp.SourceAttributeNames);

            CollectionAssert.AreEqual(new List<string>() {"xform1", "xform2"}, frp.Transforms.Select(t => t.ID).ToList());
        }


        [TestMethod]
        public void TestSingleAttributeExportFlowRule()
        {
            string flowRuleName = "csattribute<<xform1<<mvattribute";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
            
            Assert.AreEqual("csattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "mvattribute" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestMultiAttributeExportFlowRule()
        {
            string flowRuleName = "csattribute<<xform1<<mvattribute1+mvattribute2+mvattribute3";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);

            Assert.AreEqual("csattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "mvattribute3", "mvattribute2", "mvattribute1" }, frp.SourceAttributeNames);

            Assert.AreEqual(1, frp.Transforms.Count);
            Assert.AreEqual("xform1", frp.Transforms.First().ID);
        }

        [TestMethod]
        public void TestMultiAttributeMultiTransformExportFlowRule()
        {
            string flowRuleName = "csattribute<<xform1<<xform2<<mvattribute1+mvattribute2+mvattribute3";
            FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);

            Assert.AreEqual("csattribute", frp.TargetAttributeName);
            CollectionAssert.AreEqual(new List<string>() { "mvattribute3", "mvattribute2", "mvattribute1" }, frp.SourceAttributeNames);

            CollectionAssert.AreEqual(new List<string>() { "xform2", "xform1" }, frp.Transforms.Select(t => t.ID).ToList());
        }

        [TestMethod]
        public void TestInvalidImportFlowRule1()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1>>xform2>>";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidImportFlowRule2()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1<<xform2>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidImportFlowRule3()
        {
            string flowRuleName = "csattribute1<<xform1<<xform2<<mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Import);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }


        [TestMethod]
        public void TestInvalidExportFlowRule1()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3<<xform1<<xform2<<";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidExportFlowRule2()
        {
            string flowRuleName = "csattribute1+csattribute2+csattribute3>>xform1<<xform2>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidExportFlowRule3()
        {
            string flowRuleName = "csattribute1>>xform1>>xform2>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidLoopbackPositionOnImport()
        {
            string flowRuleName = "csattribute1>>loopback>>xform1>>xform2>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidMultipleLoopbackOnImport()
        {
            string flowRuleName = "csattribute1>>loopback>>xform1>>loopback>>mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidLoopbackPositionOnExport()
        {
            string flowRuleName = "csattribute1<<xform1<<loopback<<mvattribute";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

        [TestMethod]
        public void TestInvalidLoopbackOnJoin()
        {
            string flowRuleName = "csattribute1>>loopback";
            try
            {
                FlowRuleParameters frp = new FlowRuleParameters(this.config, flowRuleName, FlowRuleType.Export);
                Assert.Fail("The FRP did not throw the expected exception");
            }
            catch { }
        }

    }
}
