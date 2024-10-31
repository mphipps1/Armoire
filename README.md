# Armoire

## Database instructions

Open up PowerShell and navigate to the repo root.

First, ensure you have `dotnet-ef` tool package installed.

```
dotnet tool install --global dotnet-ef
```

You can now source the `DbSetup.ps1` script and run its `dbs` function to delete existing database/migration and create new ones.

```
. .\DbSetup.ps1
dbs
```

