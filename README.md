# Armoire

## Description

A dock for Windows that uses "drawers" to organize apps to fit users' unique workflows (WIP).

## Attribution

ICSI 499 Capstone at SUNY at Albany

## Database instructions

Open up PowerShell and navigate to the repo root.

First, ensure you have `dotnet-ef` tool package installed.

```
dotnet tool install --global dotnet-ef
```

You can now source the `Funcs.ps1` script and run its `dbs` function to delete existing database/migration and create new ones.

```
. .\Funcs.ps1
dbs
```

## Run instructions

Load the solution in Visual Studio and run it with the green play button.
