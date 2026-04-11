namespace Armoire.Models;

/*
 * Model to hold information about running applications.
 * Works cross-platform and macOS.
 */
public class RunningApplicationInfo
{
    public string StableId {get; set;} =  string.Empty;
    public int ProcessId {get; set;}
    public string DisplayName {get; set;} = string.Empty;
    public string? BundleIdentifier {get; set;}
    public string? ApplicationPath {get; set;}
    
    public RunningApplicationInfo() {}
    
    public RunningApplicationInfo(
        string stableId,
        int processId,
        string displayName,
        string? bundleIdentifier = null,
        string? applicationPath = null)
    
    
    {
        StableId = stableId;
        ProcessId = processId;
        DisplayName = displayName;
        BundleIdentifier = bundleIdentifier;
        ApplicationPath = applicationPath;
    }
}