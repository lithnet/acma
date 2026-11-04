<#
.SYNOPSIS
Creates the SQL database that the Lithnet.Acma.UnitTests suite runs against.

.DESCRIPTION
Creates an empty ACMA database with ACMA's own installer code (DBInstallUpgrader.CreateDB, the same
path the New-AcmaDatabase cmdlet uses), then creates the unit-test schema through ACMA's schema API
(AcmaDatabase.CreateObjectClass, CreateAttribute, CreateMapping, CreateReferenceLink,
CreateShadowLink and CreateSequence). Using the product's own code means the schema rows, the
generated columns and the validation rules are exactly what ACMA itself produces.

The script loads Lithnet.Acma.dll from the unit test project's build output, so build the unit
test project first. It must run in Windows PowerShell 5.1, because ACMA targets .NET Framework 4.8
and uses LINQ to SQL.

.PARAMETER ServerName
The SQL Server instance. Defaults to the ACMA_UNITTEST_SQLSERVER environment variable, then localhost.

.PARAMETER DatabaseName
The database to create. Defaults to the ACMA_UNITTEST_DATABASE environment variable, then AcmaUnitTestDB2.

.PARAMETER AssemblyPath
The folder that holds the built Lithnet.Acma.dll. Defaults to the unit test project's bin\Debug\net48 folder.

.PARAMETER Force
Drops the database first if it already exists. Without it, the script stops when the database exists.

.EXAMPLE
.\New-AcmaUnitTestDatabase.ps1 -ServerName 'localhost\AMS' -Force
#>
[CmdletBinding()]
param
(
    [string] $ServerName = $(if ($env:ACMA_UNITTEST_SQLSERVER) { $env:ACMA_UNITTEST_SQLSERVER } else { 'localhost' }),

    [string] $DatabaseName = $(if ($env:ACMA_UNITTEST_DATABASE) { $env:ACMA_UNITTEST_DATABASE } else { 'AcmaUnitTestDB2' }),

    [string] $AssemblyPath,

    [switch] $Force
)

Set-StrictMode -Version 3
$ErrorActionPreference = 'Stop'

if ($PSVersionTable.PSEdition -ne 'Desktop')
{
    throw 'Run this script in Windows PowerShell 5.1 (powershell.exe). ACMA targets .NET Framework 4.8 and cannot load in PowerShell 7.'
}

#region Test schema
# Every name below is referenced by the unit tests. Where the tests do not pin a detail down (a shadow
# link name, a provisioning attribute name, the sequence start value), the README says so.

$objectClasses = @(
    @{ Name = 'person'; AllowResurrection = $true }
    @{ Name = 'group' }
    @{ Name = 'shadowAccountAuAdAdmin'; ShadowFrom = 'person'; AllowResurrection = $true }
    @{ Name = 'testshadow'; ShadowFrom = 'person' }
    @{ Name = 'testShadowOfShadow'; ShadowFrom = 'testshadow' }
)

$attributes = @(
    # String
    @{ Name = 'accountName'; Type = 'String' }
    @{ Name = 'displayName'; Type = 'String' }
    @{ Name = 'firstName'; Type = 'String' }
    @{ Name = 'mail'; Type = 'String' }
    @{ Name = 'personID'; Type = 'String' }
    @{ Name = 'sn'; Type = 'String' }
    @{ Name = 'supervisorAccountName'; Type = 'String' }
    @{ Name = 'mailAlternateAddresses'; Type = 'String'; IsMultivalued = $true }
    @{ Name = 'sapPreviousPersonIds'; Type = 'String'; IsMultivalued = $true }

    # Integer
    @{ Name = 'activeExpiryDate'; Type = 'Integer' }
    @{ Name = 'mailAddressFormat'; Type = 'Integer' }
    @{ Name = 'sapExpiryDate'; Type = 'Integer' }
    @{ Name = 'sapPersonId'; Type = 'Integer' }
    @{ Name = 'unixGid'; Type = 'Integer' }
    @{ Name = 'unixUid'; Type = 'Integer' }
    @{ Name = 'expiryDates'; Type = 'Integer'; IsMultivalued = $true }
    @{ Name = 'expiryDates2'; Type = 'Integer'; IsMultivalued = $true }

    # DateTime
    @{ Name = 'dateTimeSV'; Type = 'DateTime' }
    @{ Name = 'dateTimeMV'; Type = 'DateTime'; IsMultivalued = $true }
    @{ Name = 'dateTimeMV2'; Type = 'DateTime'; IsMultivalued = $true }

    # Boolean
    @{ Name = 'connectedToCallista'; Type = 'Boolean' }
    @{ Name = 'connectedToSap'; Type = 'Boolean' }
    @{ Name = 'hasAdMgrAccount'; Type = 'Boolean' }
    @{ Name = 'hasTestShadow'; Type = 'Boolean' }
    @{ Name = 'hasTestShadowOfShadow'; Type = 'Boolean' }

    # Binary
    @{ Name = 'objectSid'; Type = 'Binary' }
    @{ Name = 'objectSids'; Type = 'Binary'; IsMultivalued = $true }

    # Reference
    @{ Name = 'adMgrAccount'; Type = 'Reference' }
    @{ Name = 'supervisor'; Type = 'Reference' }
    @{ Name = 'testShadowAccount'; Type = 'Reference' }
    @{ Name = 'testShadowOfShadowAccount'; Type = 'Reference' }
    @{ Name = 'directReports'; Type = 'Reference'; IsMultivalued = $true }
    @{ Name = 'displayNameSharers'; Type = 'Reference'; IsMultivalued = $true }
    @{ Name = 'peers'; Type = 'Reference'; IsMultivalued = $true }

    # Temporary (acma-internal-temp): never stored, only held in an object's pending changes.
    # MAObjectHologramTest.UpdateMVTempAttributeOnExistingObject and the DVC*MVTemp* tests in
    # DeclarativeValueConstructorTest write it with add and conditional modifications on existing objects.
    @{ Name = 'tempMV'; Type = 'String'; IsMultivalued = $true; Operation = 'AcmaInternalTemp' }
)

# Plain bindings: object class -> attributes
$bindings = [ordered]@{
    'person' = @(
        'accountName', 'displayName', 'firstName', 'mail', 'personID', 'sn', 'mailAlternateAddresses', 'sapPreviousPersonIds',
        'activeExpiryDate', 'mailAddressFormat', 'sapExpiryDate', 'sapPersonId', 'unixGid', 'unixUid', 'expiryDates', 'expiryDates2',
        'dateTimeSV', 'dateTimeMV', 'dateTimeMV2',
        'connectedToCallista', 'connectedToSap', 'hasAdMgrAccount', 'hasTestShadow',
        'objectSid', 'objectSids',
        'adMgrAccount', 'supervisor', 'testShadowAccount', 'directReports', 'displayNameSharers',
        'tempMV'
    )
    'testshadow' = @('hasTestShadowOfShadow', 'testShadowOfShadowAccount')
}

# Inherited bindings: the attribute takes its value from InheritedAttribute on the object that SourceReference points to
$inheritedBindings = @(
    @{ ObjectClass = 'person'; Attribute = 'supervisorAccountName'; SourceReference = 'supervisor'; SourceObjectClass = 'person'; InheritedAttribute = 'accountName' }
    @{ ObjectClass = 'person'; Attribute = 'peers'; SourceReference = 'supervisor'; SourceObjectClass = 'person'; InheritedAttribute = 'directReports' }
)

# Reference back links: setting ForwardLink on an object adds that object to BackLink on the referenced object
$referenceLinks = @(
    @{ ForwardObjectClass = 'person'; ForwardLink = 'supervisor'; BackObjectClass = 'person'; BackLink = 'directReports' }
)

$shadowLinks = @(
    @{ Name = 'adMgrAccount'; ShadowObjectClass = 'shadowAccountAuAdAdmin'; ProvisioningAttribute = 'hasAdMgrAccount'; ReferenceAttribute = 'adMgrAccount' }
    @{ Name = 'testShadow'; ShadowObjectClass = 'testshadow'; ProvisioningAttribute = 'hasTestShadow'; ReferenceAttribute = 'testShadowAccount' }
    @{ Name = 'testShadowOfShadow'; ShadowObjectClass = 'testShadowOfShadow'; ProvisioningAttribute = 'hasTestShadowOfShadow'; ReferenceAttribute = 'testShadowOfShadowAccount' }
)

$sequences = @(
    @{ Name = 'unixUid'; StartValue = 1000; IncrementBy = 1 }
)
#endregion

function Invoke-MasterQuery
{
    param ([string] $Query)

    $connection = New-Object System.Data.SqlClient.SqlConnection("Server=$ServerName;Database=master;Integrated Security=True")
    $connection.Open()

    try
    {
        $command = $connection.CreateCommand()
        $command.CommandText = $Query
        $command.CommandTimeout = 120
        return $command.ExecuteScalar()
    }
    finally
    {
        $connection.Dispose()
    }
}

if (-not $AssemblyPath)
{
    $AssemblyPath = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) '..\bin\Debug\net48'
}

$AssemblyPath = (Resolve-Path $AssemblyPath).Path
$acmaAssembly = Join-Path $AssemblyPath 'Lithnet.Acma.dll'

if (-not (Test-Path $acmaAssembly))
{
    throw "Lithnet.Acma.dll was not found in '$AssemblyPath'. Build the unit test project first (dotnet build src\Lithnet.Acma.UnitTests)."
}

# ACMA references Microsoft.MetadirectoryServicesEx, which the resolver loads from the local MIM Synchronization Service install
Add-Type -Path (Join-Path $AssemblyPath 'Lithnet.MetadirectoryServices.Resolver.dll')
[Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver]::RegisterResolver()
Add-Type -Path (Join-Path $AssemblyPath 'Lithnet.MetadirectoryServices.dll')
Add-Type -Path $acmaAssembly

$quotedName = $DatabaseName.Replace(']', ']]')
$literalName = $DatabaseName.Replace("'", "''")

if ($null -ne (Invoke-MasterQuery "SELECT database_id FROM sys.databases WHERE name = N'$literalName'"))
{
    if (-not $Force)
    {
        throw "The database '$DatabaseName' already exists on '$ServerName'. Use -Force to drop and recreate it."
    }

    Write-Host "Dropping database '$DatabaseName' on '$ServerName'"
    [void](Invoke-MasterQuery "ALTER DATABASE [$quotedName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$quotedName];")
}

Write-Host "Creating database '$DatabaseName' on '$ServerName'"
$upgrader = New-Object Lithnet.Acma.DBInstallUpgrader($ServerName)
$upgrader.CreateDB($DatabaseName)

$db = New-Object Lithnet.Acma.AcmaDatabase($ServerName, $DatabaseName)

# AcmaDatabase caches the schema it has read. Like the ACMA PowerShell cmdlets, clear the cache after
# every schema change so that the lookups which follow see it.

foreach ($a in $attributes)
{
    $isMultivalued = $a.ContainsKey('IsMultivalued') -and $a.IsMultivalued
    $operation = if ($a.ContainsKey('Operation')) { [Lithnet.Acma.AcmaAttributeOperation]$a.Operation } else { [Lithnet.Acma.AcmaAttributeOperation]::ImportExport }
    Write-Verbose "Attribute $($a.Name)"
    [void]$db.CreateAttribute($a.Name, [Lithnet.MetadirectoryServices.ExtendedAttributeType]$a.Type, $isMultivalued, $operation, $false, $false)
    $db.ClearCache()
}

foreach ($c in $objectClasses)
{
    Write-Verbose "Object class $($c.Name)"
    $allowResurrection = $c.ContainsKey('AllowResurrection') -and $c.AllowResurrection
    $shadowFrom = $null

    if ($c.ContainsKey('ShadowFrom'))
    {
        $shadowFrom = $db.GetObjectClass($c.ShadowFrom)
    }

    [void]$db.CreateObjectClass($c.Name, $allowResurrection, $shadowFrom)
    $db.ClearCache()
}

foreach ($className in $bindings.Keys)
{
    $objectClass = $db.GetObjectClass($className)

    foreach ($attributeName in $bindings[$className])
    {
        Write-Verbose "Binding $className.$attributeName"
        [void]$db.CreateMapping($objectClass, $db.GetAttribute($attributeName))
        $db.ClearCache()
    }
}

foreach ($b in $inheritedBindings)
{
    Write-Verbose "Inherited binding $($b.ObjectClass).$($b.Attribute) <- $($b.SourceReference)->$($b.InheritedAttribute)"
    [void]$db.CreateMapping(
        $db.GetObjectClass($b.ObjectClass),
        $db.GetAttribute($b.Attribute),
        $db.GetAttribute($b.SourceReference),
        $db.GetObjectClass($b.SourceObjectClass),
        $db.GetAttribute($b.InheritedAttribute))
    $db.ClearCache()
}

foreach ($l in $referenceLinks)
{
    Write-Verbose "Reference link $($l.ForwardObjectClass).$($l.ForwardLink) -> $($l.BackObjectClass).$($l.BackLink)"
    [void]$db.CreateReferenceLink(
        $db.GetObjectClass($l.ForwardObjectClass),
        $db.GetAttribute($l.ForwardLink),
        $db.GetObjectClass($l.BackObjectClass),
        $db.GetAttribute($l.BackLink))
    $db.ClearCache()
}

foreach ($s in $shadowLinks)
{
    Write-Verbose "Shadow link $($s.Name)"
    [void]$db.CreateShadowLink(
        $db.GetObjectClass($s.ShadowObjectClass),
        $db.GetAttribute($s.ProvisioningAttribute),
        $db.GetAttribute($s.ReferenceAttribute),
        $s.Name)
    $db.ClearCache()
}

foreach ($s in $sequences)
{
    Write-Verbose "Sequence $($s.Name)"
    [void]$db.CreateSequence($s.Name, $s.StartValue, $s.IncrementBy, $null, $null, $false)
}

Write-Host "Created '$DatabaseName' on '$ServerName': $(@($objectClasses).Count) object classes, $(@($attributes).Count) attributes, $(@($referenceLinks).Count) reference link(s), $(@($shadowLinks).Count) shadow links, $(@($sequences).Count) sequence(s)"
