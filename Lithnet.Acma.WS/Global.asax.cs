using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Lithnet.Logging;
using System.Configuration;

namespace Lithnet.Acma.WS
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();
            Logger.LogPath = ConfigurationManager.AppSettings["logfile"];
        }
    }
}