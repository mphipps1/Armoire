using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.Utils
{
    public static class WindowsFunctionalities
    {
        //https://stackoverflow.com/questions/462381/restarting-windows-from-within-a-net-application
        public static void Restart()
        {
            StartShutDown("-f -r -t 5");
        }

        public static void LogOff()
        {
            StartShutDown("-l");
        }

        public static void Shutdown()
        {
            StartShutDown("-f -s -t 5");
        }

        private static void StartShutDown(string param)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C shutdown " + param;
            Process.Start(proc);
        }
    }
}
