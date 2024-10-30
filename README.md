# Armoire

## Database instructions

Open up PowerShell and navigate to the repo root.

First, ensure you have `dotnet-ef` tool package installed.

```
dotnet tool install --global dotnet-ef
```

Now you can create and apply an initial migration.

```
dotnet ef migrations add InitialCreate --project .\Armoire\Armoire.csproj
dotnet ef database update --project .\Armoire\Armoire.csproj
```

