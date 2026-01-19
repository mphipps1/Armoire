# Armoire

## Description

A dock for Windows that helps users organize their apps into "drawers" to fit their unique workflows (WIP).

## Affiliation

ICSI 499 Capstone at SUNY at Albany

## System Requirements

Windows 10 or 11

## Setup (Do this before you run the app)

### Database instructions

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

Run Visual Studio as an administrator.

Load the solution in Visual Studio and run it with the green play button.
