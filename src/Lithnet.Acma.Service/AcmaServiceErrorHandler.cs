using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Lithnet.Logging;

namespace Lithnet.Acma.Service
{
    /// <summary>
    /// Logs every exception the resource service throws, and returns its message to the client as the fault reason.
    /// The stack trace stays in the service log.
    /// </summary>
    public class AcmaServiceErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            Logger.WriteLine("The resource service could not complete a request");
            Logger.WriteException(error);

            // Keeps the client's session open, so a refused request does not fault its channel
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is FaultException)
            {
                return;
            }

            FaultException faultException = new FaultException(error.Message);
            fault = Message.CreateMessage(version, faultException.CreateMessageFault(), faultException.Action);
        }
    }
}
