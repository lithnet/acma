Add-PSSnapin acmacmdlets
New-AcmaDatabase -ServerName localhost -DatabaseName AcmaDemo
Connect-AcmaEngine -ServerName localhost -DatabaseName AcmaDemo

try
{
# Create person attributes
Add-AcmaSchemaAttribute -Name firstName -Type String -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name middleName -Type String -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name sn -Type String  -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name employeeNumber -Type String -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name orgUnit -Type Reference -IsIndexed $true -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name unixUid -Type Integer -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name adminAccount -Type Reference -IsMultivalued $false -Operation ImportOnly
Add-AcmaSchemaAttribute -Name hasAdminAccount -Type Boolean -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name orgUnitName -Type String -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name manager -Type Reference -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name directReports -Type Reference -IsMultivalued $true -Operation ImportExport
Add-AcmaSchemaAttribute -Name displayName -Type String  -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name mail -Type String -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name accountName -Type String  -IsIndexed $false -IsMultivalued $false -Operation ImportExport
Add-AcmaSchemaAttribute -Name homeFolderPath -Type String  -IsIndexed $false -IsMultivalued $false -Operation ImportExport

# Create OU attributes
Add-AcmaSchemaAttribute -Name ouNumber -Type String -IsIndexed $false -IsMultivalued $false -Operation ImportExport

# Create object classes
Add-AcmaSchemaObjectClass -Name orgUnit -IsUndeletable $false
Add-AcmaSchemaObjectClass -Name person -IsUndeletable $true
Add-AcmaSchemaObjectClass -Name shadowAdminAccount -IsUndeletable $false -ShadowFrom person

# Create orgUnit bindings
Add-AcmaSchemaBinding -Attribute ouNumber -ObjectClass orgUnit
Add-AcmaSchemaBinding -Attribute displayName -ObjectClass orgUnit

# Create person bindings
Add-AcmaSchemaBinding -Attribute firstName -ObjectClass person
Add-AcmaSchemaBinding -Attribute sn -ObjectClass person
Add-AcmaSchemaBinding -Attribute middleName -ObjectClass person
Add-AcmaSchemaBinding -Attribute employeeNumber -ObjectClass person
Add-AcmaSchemaBinding -Attribute orgUnit -ObjectClass person
Add-AcmaSchemaBinding -Attribute unixUid -ObjectClass person
Add-AcmaSchemaBinding -Attribute orgUnitName -ObjectClass person -InheritanceSourceAttribute displayName -InheritanceSourceClass orgUnit -InheritanceSourceReference orgUnit
Add-AcmaSchemaBinding -Attribute manager -ObjectClass person
Add-AcmaSchemaBinding -Attribute displayName -ObjectClass person
Add-AcmaSchemaBinding -Attribute mail -ObjectClass person
Add-AcmaSchemaBinding -Attribute accountName -ObjectClass person
Add-AcmaSchemaBinding -Attribute hasAdminAccount -ObjectClass person
Add-AcmaSchemaBinding -Attribute adminAccount -ObjectClass person
Add-AcmaSchemaBinding -Attribute directReports -ObjectClass person
Add-AcmaSchemaBinding -Attribute homeFolderPath -ObjectClass person

# Create person back links
Add-AcmaSchemaReferenceBackLink -SourceObjectClass person -SourceAttribute manager -TargetObjectClass person -TargetAttribute directReports

# Create shadowAdminAccount bindings
Add-AcmaSchemaBinding -Attribute accountName -ObjectClass shadowAdminAccount
Add-AcmaSchemaBinding -Attribute mail -ObjectClass shadowAdminAccount
Add-AcmaSchemaBinding -Attribute displayName -ObjectClass shadowAdminAccount
Add-AcmaSchemaBinding -Attribute firstName -ObjectClass shadowAdminAccount -InheritanceSourceAttribute firstName -InheritanceSourceClass person -InheritanceSourceReference shadowParent
Add-AcmaSchemaBinding -Attribute middleName -ObjectClass shadowAdminAccount -InheritanceSourceAttribute middleName -InheritanceSourceClass person -InheritanceSourceReference shadowParent
Add-AcmaSchemaBinding -Attribute sn -ObjectClass shadowAdminAccount -InheritanceSourceAttribute sn -InheritanceSourceClass person -InheritanceSourceReference shadowParent
Add-AcmaSchemaBinding -Attribute employeeNumber -ObjectClass shadowAdminAccount -InheritanceSourceAttribute employeeNumber -InheritanceSourceClass person -InheritanceSourceReference shadowParent

# Create constants
Add-AcmaConstant -Name homeFolderPathRoot -Value '%temp%\acmausers'
Add-AcmaConstant -Name mailSuffix -Value 'acma-demo.com'

# Create sequences
Add-AcmaSequence -Name homeFolderGroup -StartValue 1 -IncrementBy 1 -MinValue 1 -MaxValue 10
Add-AcmaSequence -Name unixUid -StartValue 20000 -IncrementBy 1

# Create shadow links
Add-AcmaShadowObjectLink -Name adminAccount -ReferenceAttribute adminAccount -ProvisioningAttribute hasAdminAccount -ShadowObjectClass shadowAdminAccount

}
catch
{
    $_.Exception.ToString();
    throw;
}