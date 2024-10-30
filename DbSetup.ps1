function dbs {
    Remove-Item -r -fo .\Armoire\Migrations
    Remove-Item .\Armoire\ArmoireData.db
    dotnet ef migrations add InitialCreate --project .\Armoire\Armoire.csproj
    dotnet ef database update --project .\Armoire\Armoire.csproj
}
