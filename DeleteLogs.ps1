function dls {
    $logs_path = "$($env:appdata)\ArmoireDebugOutput*"

    Remove-Item $logs_path
}
