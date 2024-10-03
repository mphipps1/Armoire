using System;

namespace Armoire.Utils;

public class OsUtils
{
    public static bool IsWindows11()
    {
        //windows 11 machines will return a version build greater that 22000,
        //probably not the best long term solution as this will return true when windows 12 releases
        //https://stackoverflow.com/questions/69038560/detect-windows-11-with-net-framework-or-windows-api
        return Environment.OSVersion.Version.Build >= 22000;
    }
}
