# Unit test database

The tests in Lithnet.Acma.UnitTests run against a real ACMA database that holds a test schema. `New-AcmaUnitTestDatabase.ps1` creates that database from nothing.

## Create the database

1. Build the unit test project: `dotnet build src\Lithnet.Acma.UnitTests`
2. In Windows PowerShell 5.1 (not PowerShell 7), run:

   ```powershell
   .\src\Lithnet.Acma.UnitTests\TestDatabase\New-AcmaUnitTestDatabase.ps1 -ServerName 'localhost\SQLEXPRESS' -Force
   ```

`-Force` drops the database first if it exists. You need rights to create databases on the server. The MIM Synchronization Service must be installed on the machine, because ACMA loads `Microsoft.MetadirectoryServicesEx` from it.

The script creates the empty database with ACMA's own installer code (`DBInstallUpgrader.CreateDB`, which `New-AcmaDatabase` also uses; its script is the deployment script of the Lithnet.Acma.Database project). It then adds the schema through ACMA's schema API, so the result is the same as building the schema in the ACMA editor. The schema is declared in tables at the top of the script.

## Run the tests

The tests and the script read the same settings:

| Environment variable | Default |
|---|---|
| `ACMA_UNITTEST_SQLSERVER` | `localhost` |
| `ACMA_UNITTEST_DATABASE` | `AcmaUnitTestDB2` |
| `ACMA_UNITTEST_LOGPATH` | `acma-unit-test.log` in the test output folder |

```powershell
$env:ACMA_UNITTEST_SQLSERVER = 'localhost\SQLEXPRESS'
dotnet test src\Lithnet.Acma.UnitTests
```

The tests create and delete their own objects, so the database can be reused between runs. Recreate it with `-Force` if a run is interrupted and leaves objects behind.

## Schema choices the tests do not fix

The original test database was lost, so the schema was rebuilt from what the tests use. These details are not pinned down by any test and were chosen to fit the test code:

- The names of the shadow link provisioning and reference attributes (`hasAdMgrAccount`, `hasTestShadow`, `testShadowAccount`, `hasTestShadowOfShadow`, `testShadowOfShadowAccount`) and the shadow link name `adMgrAccount`. The tests find the `testShadow` and `testShadowOfShadow` links by name and the `shadowAccountAuAdAdmin` link by its reference attribute `adMgrAccount`.
- `mailAddressFormat` is an integer; the tests write integers to it and compare it with `"1"`.
- The `unixUid` sequence starts at 1000; the tests only read its next value.
- All attributes are import/export and not indexed.
- The `group` class has only the built-in attributes; the tests only check that `sn` and `shadowParent` are not on it.
