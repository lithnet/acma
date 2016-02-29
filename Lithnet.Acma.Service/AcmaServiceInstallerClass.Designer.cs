namespace Lithnet.Acma.Service
{
    partial class AcmaServiceInstallerClass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.acmaProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.acmaServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // acmaProcessInstaller
            // 
            this.acmaProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.acmaProcessInstaller.Password = null;
            this.acmaProcessInstaller.Username = null;
            // 
            // acmaServiceInstaller
            // 
            this.acmaServiceInstaller.DisplayName = "Lithnet ACMA";
            this.acmaServiceInstaller.ServiceName = "acma";
            this.acmaServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.acmaProcessInstaller,
            this.acmaServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller acmaProcessInstaller;
        private System.ServiceProcess.ServiceInstaller acmaServiceInstaller;
    }
}