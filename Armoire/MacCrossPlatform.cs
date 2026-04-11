using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Armoire.Interfaces;
using Armoire.Utils;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace Armoire;

public class MacCrossPlatform : ICrossPlatform
{
    public bool IsOnBattery()
    {
        bool result = false;
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "pmset",
            Arguments = "-g batt",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
            
        //check if device is on battery or not
        if (output.Contains("AC Power"))
        {
            result = false;
                
        } else if (output.Contains("Battery Power"))
        {
            result = true;
        }

        return result;
    }

    public int BatteryLevel()
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "pmset",
                Arguments = "-g batt",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            //use Regex matching to get the battery level number from the pmset output
            Match batteryLevelNumber = Regex.Match(output, @"(\d+)%");
            if (batteryLevelNumber.Success)
            {
                int batteryLevel = int.Parse(batteryLevelNumber.Groups[1].Value);
                return batteryLevel;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return -1;
    }

public int BatteryLifeRemainingInSeconds() => 86400;
    public ICrossPlatform.Location GetLocation() => new ICrossPlatform.Location();
    public void Restart()
    {
        Process.Start("osascript", "-e 'tell app \"System Events\" to restart'");
    }

    public void LogOff()
    {
        Process.Start("osascript", "-e 'tell app \"System Events\" to log out'");
    }   

    public void Shutdown()
    {
        Process.Start("osascript", "-e 'tell app \"System Events\" to shut down'");
    }

    public IReadOnlyList<ICrossPlatform.RunningApplicationInfo> GetRunningApplications()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "osascript",
            Arguments =
                "-e 'tell application \"System Events\" to get {name, bundle identifier, unix id, path} of (processes where background only is false)'",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        
        using var process = Process.Start(processStartInfo);
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        List<ICrossPlatform.RunningApplicationInfo> runningApplicationsList = new List<ICrossPlatform.RunningApplicationInfo>();
        
        string[] runningApps = output.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i + 3 < runningApps.Length; i += 4)
        {
            string name = runningApps[i];
            string bundleIdentifier = runningApps[i + 1];
            int processId = int.TryParse(runningApps[i + 2], out var parsedProcessId) ? parsedProcessId : 0;
            string appPath = runningApps[i + 3];
            
            // Using the bundleID and the process ID to create a stable ID for macOS
            string stableId = $"{bundleIdentifier}-{processId}";
            
            runningApplicationsList.Add(new ICrossPlatform.RunningApplicationInfo
            {
                StableId = stableId,
                ProcessId = processId,
                DisplayName = name,
                BundleIdentifier = bundleIdentifier,
                ApplicationPath = appPath
            });
        }
        return runningApplicationsList.AsReadOnly();
    }

    public Task BringApplicationToForegroundAsync(int processId)
    {
        throw new System.NotImplementedException();
    }

    public Task CloseApplicationAsync(int processId)
    {
        throw new System.NotImplementedException();
    }

    public Bitmap GetAppIcon(string appID)
    {
        string bundleIdentifier = appID;
        
        //get the Application path from bundle ID
        string appPath = $"/Applications/{bundleIdentifier}.app";
        if (!Directory.Exists(appPath)) return null;
        
        // .icns file is in Contents/Resources
        string iconPath = Path.Combine(appPath, "Conents", "Resources", "AppIcon.icns");
        
        if (File.Exists(iconPath))
            return new Bitmap(iconPath);
        return null;
    }
}