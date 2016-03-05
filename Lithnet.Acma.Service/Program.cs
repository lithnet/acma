using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Lithnet.Logging;

namespace Lithnet.Acma.Service
{
    static class Program
    {
        private static ServiceMain service;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Program.service = new ServiceMain();
                Program.service.Start(null);
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new ServiceMain() 
                };

                ServiceBase.Run(ServicesToRun);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.WriteLine("Unhandled exception");
            Logger.WriteException((Exception)e.ExceptionObject);
        }
    }
}
