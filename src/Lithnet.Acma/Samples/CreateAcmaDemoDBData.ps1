#region Script Header
<#
.SYNOPSIS
    Creates sample data for ACMA demonstration database
    
.DESCRIPTION
    This script populates an ACMA database with sample organizational units and personnel
    for demonstration and testing purposes. It creates a realistic organizational structure
    with departments, employees, and management hierarchies.
    
.PARAMETER ServerName
    SQL Server instance name hosting the ACMA database (default: localhost)
    
.PARAMETER DatabaseName
    Name of the ACMA database (default: AcmaDemo)
    
.PARAMETER ConfigFile
    Path to the ACMA configuration file (.acmax)
    
.PARAMETER LogFile
    Path for the ACMA log file (optional)
    
.PARAMETER LogLevel
    Logging level (Debug, Info, Warning, Error)
    
.EXAMPLE
    .\CreateAcmaDemoDBData.ps1
    Creates demo data using default parameters
    
.EXAMPLE
    .\CreateAcmaDemoDBData.ps1 -ServerName "SQL01" -DatabaseName "AcmaTest" -LogLevel Info
    Creates demo data on specific server with custom settings
    
.NOTES
    Author: Lithnet
    Version: 2.0
    Created: August 2025
    
    Prerequisites:
    - ACMA PowerShell module (AcmaPS) must be installed
    - SQL Server database must be accessible
    - ACMA configuration file must exist
    - Appropriate permissions for database operations
#>
#endregion

#region Parameters and Variables
param(
    [Parameter(HelpMessage = "SQL Server instance name")]
    [string]$ServerName = "localhost",
    
    [Parameter(HelpMessage = "ACMA database name")]
    [string]$DatabaseName = "AcmaDemo",
    
    [Parameter(HelpMessage = "Path to ACMA configuration file")]
    [string]$ConfigFile = "C:\ACMADemo\acma-demo.acmax",
    
    [Parameter(HelpMessage = "Path for log file")]
    [string]$LogFile = "C:\ACMADemo\demo-data-creation.log",
    
    [Parameter(HelpMessage = "Logging level")]
    [ValidateSet("Debug", "Info", "Warning", "Error")]
    [string]$LogLevel = "Info"
)

# Script configuration
$ErrorActionPreference = "Stop"
$VerbosePreference = "Continue"

# Sample data definitions
$DemoData = @{
    OrganizationalUnits = @(
        @{ DisplayName = "Finance"; OUNumber = 2001; Description = "Financial Operations and Accounting" }
        @{ DisplayName = "Information Technology"; OUNumber = 2002; Description = "IT Operations and Development" }
        @{ DisplayName = "Sales"; OUNumber = 2003; Description = "Sales and Marketing Operations" }
        @{ DisplayName = "Human Resources"; OUNumber = 2004; Description = "HR and Employee Services" }
        @{ DisplayName = "Operations"; OUNumber = 2005; Description = "Business Operations and Support" }
    )
    
    Personnel = @(
        @{ 
            FirstName = "John"; LastName = "Smith"; EmployeeNumber = 1000; 
            Department = "Information Technology"; Title = "IT Director"; 
            Email = "john.smith@company.com"; Phone = "+1-555-0101"
        }
        @{ 
            FirstName = "Sarah"; LastName = "Johnson"; EmployeeNumber = 1001; 
            Department = "Finance"; Title = "Finance Manager"; Manager = 1000;
            Email = "sarah.johnson@company.com"; Phone = "+1-555-0102"
        }
        @{ 
            FirstName = "Michael"; MiddleName = "James"; LastName = "Williams"; EmployeeNumber = 1002; 
            Department = "Sales"; Title = "Sales Director"; Manager = 1000;
            Email = "michael.williams@company.com"; Phone = "+1-555-0103"
        }
        @{ 
            FirstName = "Emily"; LastName = "Davis"; EmployeeNumber = 1003; 
            Department = "Human Resources"; Title = "HR Manager"; Manager = 1000;
            Email = "emily.davis@company.com"; Phone = "+1-555-0104"
        }
        @{ 
            FirstName = "Robert"; LastName = "Brown"; EmployeeNumber = 1004; 
            Department = "Information Technology"; Title = "Senior Developer"; Manager = 1000;
            Email = "robert.brown@company.com"; Phone = "+1-555-0105"
        }
        @{ 
            FirstName = "Jennifer"; LastName = "Wilson"; EmployeeNumber = 1005; 
            Department = "Finance"; Title = "Accountant"; Manager = 1001;
            Email = "jennifer.wilson@company.com"; Phone = "+1-555-0106"
        }
        @{ 
            FirstName = "David"; LastName = "Garcia"; EmployeeNumber = 1006; 
            Department = "Sales"; Title = "Sales Representative"; Manager = 1002;
            Email = "david.garcia@company.com"; Phone = "+1-555-0107"
        }
        @{ 
            FirstName = "Lisa"; LastName = "Martinez"; EmployeeNumber = 1007; 
            Department = "Operations"; Title = "Operations Coordinator"; Manager = 1000;
            Email = "lisa.martinez@company.com"; Phone = "+1-555-0108"
        }
    )
}
#endregion

#region Functions
function Write-LogMessage {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter()]
        [ValidateSet("Info", "Warning", "Error", "Success")]
        [string]$Level = "Info"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    switch ($Level) {
        "Info" { Write-Host $logMessage -ForegroundColor Cyan }
        "Warning" { Write-Warning $logMessage }
        "Error" { Write-Error $logMessage }
        "Success" { Write-Host $logMessage -ForegroundColor Green }
    }
    
    if ($LogFile) {
        Add-Content -Path $LogFile -Value $logMessage
    }
}

function Test-Prerequisites {
    Write-LogMessage "Checking prerequisites..."
    
    # Check if AcmaPS module is available
    if (-not (Get-Module -ListAvailable -Name "AcmaPS")) {
        throw "AcmaPS PowerShell module is not installed. Please install the ACMA PowerShell module."
    }
    
    # Check if configuration file exists
    if (-not (Test-Path $ConfigFile)) {
        throw "ACMA configuration file not found at: $ConfigFile"
    }
    
    # Check if log directory exists, create if needed
    if ($LogFile) {
        $logDir = Split-Path $LogFile -Parent
        if (-not (Test-Path $logDir)) {
            New-Item -Path $logDir -ItemType Directory -Force | Out-Null
            Write-LogMessage "Created log directory: $logDir"
        }
    }
    
    Write-LogMessage "Prerequisites check completed successfully" -Level Success
}

function Connect-AcmaDatabase {
    Write-LogMessage "Connecting to ACMA database..."
    
    try {
        # Import ACMA PowerShell module
        Import-Module AcmaPS -Force
        
        # Connect to ACMA engine
        $connectionParams = @{
            ServerName = $ServerName
            DatabaseName = $DatabaseName
            ConfigFile = $ConfigFile
            LogLevel = $LogLevel
        }
        
        if ($LogFile) {
            $connectionParams.LogFile = $LogFile
        }
        
        Connect-AcmaEngine @connectionParams
        Write-LogMessage "Successfully connected to ACMA database: $DatabaseName on $ServerName" -Level Success
    }
    catch {
        Write-LogMessage "Failed to connect to ACMA database: $($_.Exception.Message)" -Level Error
        throw
    }
}

function New-DemoOrganizationalUnits {
    Write-LogMessage "Creating organizational units..."
    $createdOUs = @{}
    
    foreach ($ouData in $DemoData.OrganizationalUnits) {
        try {
            Write-LogMessage "Creating organizational unit: $($ouData.DisplayName)"
            
            # Create new organizational unit object
            $orgUnit = Add-AcmaObject -ObjectClass orgUnit
            
            # Set attributes using modern syntax
            $orgUnit.displayName = $ouData.DisplayName
            $orgUnit.ouNumber = $ouData.OUNumber
            $orgUnit.description = $ouData.Description
            $orgUnit.created = Get-Date
            $orgUnit.isActive = $true
            
            # Save the object
            Save-AcmaObject $orgUnit
            
            # Store reference for later use
            $createdOUs[$ouData.DisplayName] = $orgUnit
            
            Write-LogMessage "Successfully created OU: $($ouData.DisplayName) (ID: $($orgUnit.ObjectId))" -Level Success
        }
        catch {
            Write-LogMessage "Failed to create organizational unit '$($ouData.DisplayName)': $($_.Exception.Message)" -Level Error
            throw
        }
    }
    
    return $createdOUs
}

function New-DemoPersonnel {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$OrganizationalUnits
    )
    
    Write-LogMessage "Creating personnel objects..."
    $createdPersons = @{}
    
    # First pass: Create all person objects without manager references
    foreach ($personData in $DemoData.Personnel) {
        try {
            Write-LogMessage "Creating person: $($personData.FirstName) $($personData.LastName)"
            
            # Create new person object
            $person = Add-AcmaObject -ObjectClass person
            
            # Set basic attributes
            $person.firstName = $personData.FirstName
            if ($personData.MiddleName) {
                $person.middleName = $personData.MiddleName
            }
            $person.sn = $personData.LastName
            $person.employeeNumber = $personData.EmployeeNumber
            $person.title = $personData.Title
            $person.mail = $personData.Email
            $person.telephoneNumber = $personData.Phone
            $person.created = Get-Date
            $person.accountDisabled = $false
            
            # Set organizational unit reference
            if ($OrganizationalUnits.ContainsKey($personData.Department)) {
                $person.orgUnit = $OrganizationalUnits[$personData.Department]
            }
            else {
                Write-LogMessage "Warning: Department '$($personData.Department)' not found for $($personData.FirstName) $($personData.LastName)" -Level Warning
            }
            
            # Generate display name
            $displayName = "$($personData.FirstName) $($personData.LastName)"
            if ($personData.MiddleName) {
                $displayName = "$($personData.FirstName) $($personData.MiddleName) $($personData.LastName)"
            }
            $person.displayName = $displayName
            
            # Generate sAMAccountName
            $samAccountName = ($personData.FirstName.Substring(0,1) + $personData.LastName).ToLower()
            $person.sAMAccountName = $samAccountName
            
            # Save the object
            Save-AcmaObject $person
            
            # Store reference for manager assignment
            $createdPersons[$personData.EmployeeNumber] = $person
            
            Write-LogMessage "Successfully created person: $displayName (ID: $($person.ObjectId), Employee: $($personData.EmployeeNumber))" -Level Success
        }
        catch {
            Write-LogMessage "Failed to create person '$($personData.FirstName) $($personData.LastName)': $($_.Exception.Message)" -Level Error
            throw
        }
    }
    
    # Second pass: Set manager references
    Write-LogMessage "Setting manager relationships..."
    foreach ($personData in $DemoData.Personnel) {
        if ($personData.Manager) {
            try {
                $person = $createdPersons[$personData.EmployeeNumber]
                $manager = $createdPersons[$personData.Manager]
                
                if ($person -and $manager) {
                    $person.manager = $manager
                    Save-AcmaObject $person
                    
                    Write-LogMessage "Set manager for $($person.displayName): $($manager.displayName)" -Level Success
                }
                else {
                    Write-LogMessage "Could not set manager relationship for employee $($personData.EmployeeNumber)" -Level Warning
                }
            }
            catch {
                Write-LogMessage "Failed to set manager for employee $($personData.EmployeeNumber): $($_.Exception.Message)" -Level Error
            }
        }
    }
    
    return $createdPersons
}

function Show-CreatedData {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$OrganizationalUnits,
        
        [Parameter(Mandatory = $true)]
        [hashtable]$Personnel
    )
    
    Write-LogMessage "Demo data creation summary:" -Level Success
    Write-LogMessage "============================"
    
    Write-LogMessage "Organizational Units Created: $($OrganizationalUnits.Count)"
    foreach ($ou in $OrganizationalUnits.Values) {
        Write-LogMessage "  - $($ou.displayName) (OU#: $($ou.ouNumber), ID: $($ou.ObjectId))"
    }
    
    Write-LogMessage "Personnel Created: $($Personnel.Count)"
    foreach ($person in $Personnel.Values | Sort-Object employeeNumber) {
        $managerInfo = if ($person.manager) { " | Manager: $($person.manager.displayName)" } else { "" }
        Write-LogMessage "  - $($person.displayName) (Emp#: $($person.employeeNumber), Dept: $($person.orgUnit.displayName)$managerInfo)"
    }
    
    Write-LogMessage "============================"
    Write-LogMessage "Demo data creation completed successfully!" -Level Success
}
#endregion

#region Main Execution
try {
    Write-LogMessage "Starting ACMA Demo Database Data Creation Script" -Level Success
    Write-LogMessage "Server: $ServerName | Database: $DatabaseName | Config: $ConfigFile"
    
    # Check prerequisites
    Test-Prerequisites
    
    # Connect to ACMA database
    Connect-AcmaDatabase
    
    # Create organizational units
    $createdOUs = New-DemoOrganizationalUnits
    
    # Create personnel
    $createdPersons = New-DemoPersonnel -OrganizationalUnits $createdOUs
    
    # Display summary
    Show-CreatedData -OrganizationalUnits $createdOUs -Personnel $createdPersons
    
    Write-LogMessage "ACMA demo data creation completed successfully!" -Level Success
}
catch {
    Write-LogMessage "Script execution failed: $($_.Exception.Message)" -Level Error
    Write-LogMessage "Stack trace: $($_.ScriptStackTrace)" -Level Error
    exit 1
}
finally {
    # Cleanup if needed
    Write-LogMessage "Script execution finished."
}
#endregion
