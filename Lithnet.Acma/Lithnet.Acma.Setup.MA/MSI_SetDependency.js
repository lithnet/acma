// MSI_SetActionSequence.js <msi-file> <table> <action> <parent>
// Performs a post-build fixup of an msi to set the specified table/action/sequence

// Constant values from Windows Installer SDK
var msiOpenDatabaseModeTransact = 1;

var msiViewModifyInsert         = 1;
var msiViewModifyUpdate         = 2;
var msiViewModifyAssign         = 3;
var msiViewModifyReplace        = 4;
var msiViewModifyDelete         = 6;

if (WScript.Arguments.Length != 1)
{
    WScript.StdErr.WriteLine("Usage: " + WScript.ScriptName + " file table action parent");
    WScript.Quit(1);
}

var filespec = WScript.Arguments(0);

var installer = WScript.CreateObject("WindowsInstaller.Installer");
var database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);

WScript.StdOut.WriteLine("Opened database: " + filespec);

try
{   
    var sql = "SELECT Property, Signature_ FROM AppSearch";
    var view = database.OpenView(sql);	
    WScript.StdOut.WriteLine("Opened view");

    view.Execute();
    WScript.StdOut.WriteLine("Executed view");

    var record = view.Fetch();	
    WScript.StdOut.WriteLine("Got record");

    var sigFimInstallDir;
    var sigValidFimVersion;

    if (record)
    {		
    	while (record)
    	{
    	    if (record.StringData(1) == "FIMINSTALLDIR")
		    {
		        sigFimInstallDir = record.StringData(2);
		        WScript.StdOut.WriteLine("Found: " + record.StringData(1) + ", " + record.StringData(2));

		    }
		    else if (record.StringData(1) == "VALIDFIMVERSION")
		    {
		        sigValidFimVersion = record.StringData(2);
		        WScript.StdOut.WriteLine("Found: " + record.StringData(1) + ", " + record.StringData(2));
		    }
		    else
		    {
		        WScript.StdOut.WriteLine("Disregarding: " + record.StringData(1) + ", " + record.StringData(2));
		    }

    		record = view.Fetch();
    	}

    	view.Close();
    }

    if (sigFimInstallDir == null)
    {
        throw("Could not find the FIMINSTALLDIR property")
    }

    if (sigValidFimVersion == null) {
        throw ("Could not find the VALIDFIMVERSION property")
    }

    var sql = "SELECT Signature_, Parent FROM DrLocator WHERE Signature_ = '" + sigValidFimVersion + "'";
    var view = database.OpenView(sql);
    WScript.StdOut.WriteLine("Opened view");

    view.Execute();
    WScript.StdOut.WriteLine("Executed view");

    var record = view.Fetch();
    WScript.StdOut.WriteLine("Got row");

    if (record)
    {
        WScript.StdOut.WriteLine("Found: " + record.StringData(1) + ", " + record.StringData(2));
        
        record.StringData(2) = sigFimInstallDir;
        WScript.StdOut.WriteLine("Set property");

        view.Modify(msiViewModifyReplace, record);
        WScript.StdOut.WriteLine("Modified record");

        view.Close();
        WScript.StdOut.WriteLine("Closed view");

        database.Commit();
        WScript.StdOut.WriteLine("Committed database changes");

    }
    else
    {
        throw ("Could not find the DrLocator table entry")
    }

}
catch(e)
{
    WScript.StdErr.WriteLine(e.stack);
    WScript.StdErr.WriteLine(e.description);
    WScript.StdErr.WriteLine(e.message);
    WScript.StdErr.WriteLine(e.toString());

    WScript.Quit(1);
}
