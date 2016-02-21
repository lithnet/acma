using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Lithnet.Acma.Presentation
{
    public class DBConnectionViewModel : ViewModelBase
    {
        public DBConnectionViewModel(Window window)
            : base()
        {
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
        }

        public string DisplayName
        {
            get
            {
                return "Connect to database";
            }
        }

        public string Server { get; set; }

        public string Database { get; set; }

        private bool CanCreate()
        {
            return !string.IsNullOrWhiteSpace(this.Server) && !string.IsNullOrWhiteSpace(this.Database);
        }

        private void Create(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }
    }
}
