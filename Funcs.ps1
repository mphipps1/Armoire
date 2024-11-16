# Database Setup
function dbs {
    $migrations_dir = ".\Armoire\Migrations"
    $db_path = "$($env:localappdata)\ArmoireData.db"

    if (Test-Path $migrations_dir) {
        Remove-Item -r -fo $migrations_dir
    }
    if (Test-Path $db_path) {
        Remove-Item $db_path
    }

    dotnet ef migrations add InitialCreate --project .\Armoire\Armoire.csproj
    dotnet ef database update --project .\Armoire\Armoire.csproj
}

# Delete Logs
function dl {
    $logs_path = "$($env:appdata)\ArmoireDebugOutput*"

    Remove-Item $logs_path
}

# Populate Database
function pdb {
    sqlite3 $env:localappdata\ArmoireData.db ".read commands.sql"
}

# Run All
function ra {
    dbs
    dl
}
