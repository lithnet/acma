del $(ProjectDir)Lithnet.Acma.PackagedMA.xml /f
"C:\Program Files\Microsoft Forefront Identity Manager\2010\Synchronization Service\Bin\mapackager.exe" /e:$(ProjectDir)ExportedMA.xml /c:$(ProjectDir)MAPackageInformation.xml /o:$(ProjectDir)Lithnet.Acma.PackagedMA.xml
