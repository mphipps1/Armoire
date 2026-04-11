using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace Armoire.Interfaces;

public interface ICrossPlatform
{
    public class Location
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class RunningApplicationInfo
    {
        public required string StableId { get; set; }
        public required int ProcessId { get; set; }
        public required string DisplayName { get; set; }
        public string? BundleIdentifier { get; set; }
        public required string? ApplicationPath { get; set; }
    }
    
    bool IsOnBattery();

    int BatteryLevel();
    int BatteryLifeRemainingInSeconds();
    Location GetLocation();
    void Restart();
    void LogOff();
    void Shutdown();
    IReadOnlyList<RunningApplicationInfo> GetRunningApplications();
    Task BringApplicationToForegroundAsync(int processId);
    Task CloseApplicationAsync(int processId);
    
    Bitmap GetAppIcon(string appID);
    
    
    public static ICrossPlatform Instance
    {
        get
        { // TODO
            return new MacCrossPlatform();
        }
    }
}