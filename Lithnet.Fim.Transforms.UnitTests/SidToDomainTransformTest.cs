using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Security.Principal;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.UnitTests
{
    [TestClass()]
    public class SidToDomainTransformTest
    {
        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            SidToDomainTransform transformToSeralize = new SidToDomainTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Format = DomainFormat.DomainName;
            UniqueIDCache.ClearIdCache();

            SidToDomainTransform deserializedTransform = (SidToDomainTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Format, deserializedTransform.Format);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void SidBytesToDomainSidBytes()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainSidBinary;
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, sidBytes, domainSidBytes);
        }

        [TestMethod()]
        public void SidBytesToDomainSidString()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainSidString;

            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, sidBytes, domainSid.Value);
        }

        [TestMethod()]
        public void SidBytesToDomainName()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainName;
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, sidBytes, Environment.UserDomainName);
        }


        [TestMethod()]
        public void SidStringToDomainSidBytes()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainSidBinary;
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, sid.Value, domainSidBytes);
        }

        [TestMethod()]
        public void SidStringToDomainSidString()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainSidString;

            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, sid.Value, domainSid.Value);
        }

        [TestMethod()]
        public void SidStringToDomainName()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainName;
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, sid.Value, Environment.UserDomainName);
        }

        [TestMethod()]
        public void SidStringToUnknownDomainName()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainName;

            this.ExecuteTest(transform, "S-1-5-21-606000587-1099826126-3276809063-1118", "S-1-5-21-606000587-1099826126-3276809063");
        }

        [TestMethod()]
        public void SidBase64StringToDomainSidBytes()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainSidBinary;
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, Convert.ToBase64String(sidBytes), domainSidBytes);
        }

        [TestMethod()]
        public void SidBase64StringToDomainSidString()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainSidString;

            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, Convert.ToBase64String(sidBytes), domainSid.Value);
        }

        [TestMethod()]
        public void SidBase64StringToDomainName()
        {
            SidToDomainTransform transform = new SidToDomainTransform();
            transform.Format = DomainFormat.DomainName;
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier domainSid = sid.AccountDomainSid;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            byte[] domainSidBytes = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(domainSidBytes, 0);

            string domainSidString = Utils.ConvertSidToString(domainSidBytes);

            this.ExecuteTest(transform, Convert.ToBase64String(sidBytes), Environment.UserDomainName);
        }


        private void ExecuteTest(SidToDomainTransform transform, string sourceValue, byte[] expectedValue)
        {
            byte[] outValue = transform.TransformValue(sourceValue).FirstOrDefault() as byte[];

            CollectionAssert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(SidToDomainTransform transform, byte[] sourceValue, byte[] expectedValue)
        {
            byte[] outValue = transform.TransformValue(sourceValue).FirstOrDefault() as byte[];

            CollectionAssert.AreEqual(expectedValue, outValue);
        }


        private void ExecuteTest(SidToDomainTransform transform, byte[] sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(SidToDomainTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

    }
}
