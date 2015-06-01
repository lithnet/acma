using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Host;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Lithnet.Acma.EmailAddressMangement 
{
    public class EmailAddressManagement : IDisposable
    {
        private PowerShell psInstance;

        private InitialSessionState sessionState;

        private Runspace runspace;

        private string DatabaseName = "Lithnet.Acma";

        private string ServerName = "localhost";

        private string ConfigFile = @"D:\MAData\Acma\acma-prod.acma.xml";

        private string LogFile = @"D:\MAData\ACMA\acma-ps.log";

        public EmailAddressManagement()
        {
            this.psInstance = PowerShell.Create();
            this.sessionState = InitialSessionState.CreateDefault();
            this.sessionState.ImportPSModule(new string[] { "AcmaCmdlets" });
            this.runspace = RunspaceFactory.CreateRunspace(this.sessionState);
            runspace.Open();

            Pipeline pipeline = runspace.CreatePipeline();
            Command command = new Command("Connect-AcmaEngine");
            command.Parameters.Add("DatabaseName", this.DatabaseName);
            command.Parameters.Add("ServerName", this.ServerName);
            command.Parameters.Add("LogFile", this.LogFile);
            pipeline.Commands.Add(command);

            // execute the script 
            Collection<PSObject> results = pipeline.Invoke();

        }

        public string GetMailAddress(string accountName)
        {
            if (accountName == null)
            {
                throw new ArgumentNullException("accountName");
            }

            Pipeline pipeline = runspace.CreatePipeline();
            Command command = new Command("New-AcmaDBQuery");
            command.Parameters.Add("Attribute", "accountName");
            command.Parameters.Add("Operator", "Equals");
            command.Parameters.Add("Value", accountName);
            pipeline.Commands.Add(command);

            // execute the script 
            Collection<PSObject> results = pipeline.Invoke();

            if (results.Count == 0)
            {
                throw new InvalidOperationException("The cmdlet returned no results");
            }



        }

        private bool disposed;

        ~EmailAddressManagement()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!disposed)
            {
                if (isDisposing)
                {
                    if (this.runspace != null)
                    {
                        this.runspace.Close();
                    }
                }

                // Release unmanaged resources here
            }

            disposed = true;
        }
    }
}
