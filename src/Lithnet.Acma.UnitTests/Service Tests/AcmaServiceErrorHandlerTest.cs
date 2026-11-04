using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Lithnet.Acma.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class AcmaServiceErrorHandlerTest
    {
        [TestMethod()]
        public void FaultReasonIsTheExceptionMessage()
        {
            AcmaServiceErrorHandler handler = new AcmaServiceErrorHandler();
            ArgumentOutOfRangeException error = new ArgumentOutOfRangeException("pageSize", "The page size must be between 1 and 1000");
            Message fault = null;

            handler.ProvideFault(error, MessageVersion.Soap12WSAddressing10, ref fault);

            Assert.IsNotNull(fault, "No fault was provided");
            MessageFault messageFault = MessageFault.CreateFault(fault, int.MaxValue);
            Assert.AreEqual(error.Message, messageFault.Reason.GetMatchingTranslation().Text);
            Assert.IsFalse(messageFault.HasDetail, "The fault carried exception detail");
        }

        [TestMethod()]
        public void FaultExceptionIsPassedThrough()
        {
            AcmaServiceErrorHandler handler = new AcmaServiceErrorHandler();
            Message fault = null;

            handler.ProvideFault(new FaultException("Thrown by the service"), MessageVersion.Soap12WSAddressing10, ref fault);

            Assert.IsNull(fault, "A fault thrown by the service was replaced");
        }

        [TestMethod()]
        public void SessionIsKeptOpen()
        {
            AcmaServiceErrorHandler handler = new AcmaServiceErrorHandler();

            Assert.IsTrue(handler.HandleError(new InvalidOperationException("Test")));
        }
    }
}
