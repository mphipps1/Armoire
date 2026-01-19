using Armoire.Interfaces;

namespace Armoire;

#if WINDOWS
public class WindowsCrossPlatform : ICrossPlatform
{
    public bool IsOnBattery() =>System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline ;
    public int BatteryLevel() => System.Windows.Forms.SystemInformation.PowerStatus.BatteryLifePercent * 100;
    public int BatteryLifeRemainingInSeconds() => System.Windows.Forms.SystemInformation.PowerStatus.BatteryLifeRemaining;

    public Location GetLocation(){
           GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.TryStart(false, TimeSpan.FromMilliseconds(3000));
            while (watcher.Status != GeoPositionStatus.Ready)
            {
                if(watcher.Permission == GeoPositionPermission.Denied)
                {
                    wvm.WeatherDesc = "Could not load weather data.";
                    wvm.CurrentTemp = "Be sure to enable location in Windows Privacy Settings.";
                    return new WeatherResponse();
                }
                Thread.Sleep(200);
            }
            int i = 0;
            return new ICrossPlatform.Location() {
                Latitude = watcher.Position.Coordinate.Point.Position.Latitude,
                Longitude = watcher.Position.Coordinate.Point.Position.Longitude
                };
    }   
        //https://stackoverflow.com/questions/462381/restarting-windows-from-within-a-net-application
        public void Restart()
        {
            StartShutDown("-f -r -t 5");
        }

        public void LogOff()
        {
            StartShutDown("-l");
        }

        public void Shutdown()
        {
            StartShutDown("-f -s -t 5");
        }

        private void StartShutDown(string param)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C shutdown " + param;
            Process.Start(proc);
        }
}
#endif