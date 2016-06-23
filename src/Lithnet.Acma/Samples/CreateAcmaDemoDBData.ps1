Connect-AcmaEngine -ServerName localhost -DatabaseName AcmaDemo -ConfigFile "C:\ACMADemo\acma-demo.acmax" -LogFile "C:\ACMADemo\demo.log" -LogLevel Debug

$orgUnit1 = Add-AcmaObject -ObjectClass orgUnit
$orgUnit1.Attributes["displayName"] = "Finance"
$orgUnit1.Attributes["ouNumber"] = 2001
$orgUnit1.Commit()

$orgUnit2 = Add-AcmaObject -ObjectClass orgUnit
$orgUnit2.Attributes["displayName"] = "IT"
$orgUnit2.Attributes["ouNumber"] = 2002
$orgUnit2.Commit()

$orgUnit3 = Add-AcmaObject -ObjectClass orgUnit
$orgUnit3.Attributes["displayName"] = "Sales"
$orgUnit3.Attributes["ouNumber"] = 2003
$orgUnit3.Commit()

$person1 = Add-AcmaObject -ObjectClass person
$person1.Attributes["firstName"] = "John"
$person1.Attributes["sn"] = "Smith"
$person1.Attributes["employeeNumber"] = 1000
$person1.Attributes["orgUnit"] = $orgUnit2
$person1.Commit();

$person2 = Add-AcmaObject -ObjectClass person
$person2.Attributes["firstName"] = "William"
$person2.Attributes["sn"] = "Keys"
$person2.Attributes["employeeNumber"] = 1001
$person2.Attributes["orgUnit"] = $orgUnit3
$person2.Attributes["manager"] = $person1
$person2.Commit();

$person3 = Add-AcmaObject -ObjectClass person
$person3.Attributes["firstName"] = "William"
$person3.Attributes["middleName"] = "John"
$person3.Attributes["sn"] = "Keys"
$person3.Attributes["employeeNumber"] = 1002
$person3.Attributes["orgUnit"] = $orgUnit1
$person3.Attributes["manager"] = $person1
$person3.Commit();

$person4 = Add-AcmaObject -ObjectClass person
$person4.Attributes["sn"] = "Stewart"
$person4.Attributes["employeeNumber"] = 1003
$person4.Attributes["orgUnit"] = $orgUnit3
$person4.Attributes["manager"] = $person1
$person4.Commit();

$person5 = Add-AcmaObject -ObjectClass person
$person5.Attributes["firstName"] = "William"
$person5.Attributes["sn"] = "Keys"
$person5.Attributes["employeeNumber"] = 1004
$person5.Attributes["orgUnit"] = $orgUnit1
$person5.Attributes["manager"] = $person1
$person5.Commit();
