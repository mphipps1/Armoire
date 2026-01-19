using System.Reflection.Metadata.Ecma335;
using Armoire.Interfaces;
using Avalonia.Controls;

namespace Armoire;

#if Mac
public class MacCrossPlatform : ICrossPlatform
{
    public bool IsOnBattery() => false;
    public int BatteryLevel() => 100;
    public int BatteryLifeRemainingInSeconds() => 86400;
    public ICrossPlatform.Location GetLocation() => new ICrossPlatform.Location();
    public void Restart()
    {
        throw new System.NotImplementedException();
    }

    public void LogOff()
    {
        throw new System.NotImplementedException();
    }

    public void Shutdown()
    {
        throw new System.NotImplementedException();
    }
}
#endif