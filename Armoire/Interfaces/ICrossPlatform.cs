namespace Armoire.Interfaces;

public interface ICrossPlatform
{
    public class Location
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
    
    bool IsOnBattery();

    int BatteryLevel();
    int BatteryLifeRemainingInSeconds();
    Location GetLocation();
    void Restart();
    void LogOff();
    void Shutdown();
    
    public static ICrossPlatform Instance
    {
        get
        { // TODO
            return new MacCrossPlatform();
        }
    }
}