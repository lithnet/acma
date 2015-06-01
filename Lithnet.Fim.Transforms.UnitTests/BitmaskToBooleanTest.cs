using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;

namespace Lithnet.Fim.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for TrimStringTransformTest and is intended to contain all TrimStringTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BitmaskToBooleanTransformTest
    {
        private TestContext testContextInstance;
       
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

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            BitmaskToBooleanTransform transformToSeralize = new BitmaskToBooleanTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Flag = 2;
            UniqueIDCache.ClearIdCache();

            BitmaskToBooleanTransform deserializedTransform = (BitmaskToBooleanTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Flag, deserializedTransform.Flag);
        }

        [TestMethod()]
        public void BitmaskTestTrue()
        {
            BitmaskToBooleanTransform transform = new BitmaskToBooleanTransform();
            transform.Flag = 2;

            this.ExecuteBitmaskToBooleanTest(transform, 514, true);
        }

        [TestMethod()]
        public void BitmaskTestFalse()
        {
            BitmaskToBooleanTransform transform = new BitmaskToBooleanTransform();
            transform.Flag = 2;

            this.ExecuteBitmaskToBooleanTest(transform, 512, false);
        }

        private void ExecuteBitmaskToBooleanTest(BitmaskToBooleanTransform transform, long sourceValue, bool expectedValue)
        {
            bool outValue = (bool)transform.TransformValue(sourceValue).FirstOrDefault();
            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
