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
#if Mac
public class MacCrossPlatform : ICrossPlatform
{
    public bool IsOnBattery()
    {
        bool result = false;
        
        string output = GetBatteryInformation();
            
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
        int batteryLevel = 0;
        string output = GetBatteryInformation();
        
        //use Regex matching to get the battery level number from the pmset output
        Match batteryLevelNumber = Regex.Match(output, @"(\d+)%");
        if (batteryLevelNumber.Success)
        {
            batteryLevel = int.Parse(batteryLevelNumber.Groups[1].Value);
            
        }
        return batteryLevel;
    }

    public int BatteryLifeRemainingInSeconds()
    {
        // initialize result
        int secondsRemaining = 0;

        string output = GetBatteryInformation();    
        
        //check if on battery
        bool onBat = IsOnBattery();
        
        //use a regex match to check for the time remaining from the output.
        var timeMatch = Regex.Match(output, @"(\d+):(\d+) remaining");
        
        //only works if on battery.
        if (timeMatch.Success && onBat.Equals(true))
        {
            int hoursRemaining = int.Parse(timeMatch.Groups[1].Value);
            int minutesRemaining = int.Parse(timeMatch.Groups[2].Value);
            secondsRemaining = (hoursRemaining * 60 + minutesRemaining) * 60;
        }
        else
        {
            return -1;
        }
        return secondsRemaining;
    }

    public ICrossPlatform.Location GetLocation()
    {
        ICrossPlatform.Location deviceLocation = new ICrossPlatform.Location();
        
        //TODO How accurate does the location need to be? Can I use IP based geolocation services?
        
        
        return deviceLocation;
    }
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

    private static string GetBatteryInformation()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = "-c \"pmset -g batt\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        return output;
    }
}
#endif