using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Armoire.ViewModels;

public partial class ApplicationMonitorViewModel : DrawerAsContentsViewModel
{
    public static ObservableCollection<ItemViewModel> ProcessList { get; } =
        new ObservableCollection<ItemViewModel>();

    public static List<string> RunningAppNames { get; set; } = new List<string>();

    private static bool isMonitoring = true;

    public static bool isRunning;

    public ApplicationMonitorViewModel(string? parentID, int drawerHierarchy)
        : base(parentID, drawerHierarchy)
    {
        Name = "Running Applications";
        Id = "MONITOR";
    }

    //[RelayCommand]
    //public void DisplayProcess()
    //{
    //    for (int j = 0; j < 5; j++)
    //    {

    //        if (ProcessList.Count > 0)
    //        {
    //            for (int i = 0; i < ProcessList.Count; i++)
    //            {
    //                bool isNewProcess = ProcessList.Any(obj => obj.ExecutablePath == RunningApps[j].StartInfo.FileName);
    //                if (!isNewProcess)
    //                {
    //                    ProcessList.Add(new ItemViewModel(RunningApps[j].StartInfo.FileName));
    //                    return;
    //                }
    //            }


    //        }
    //        else
    //        {
    //            ProcessList.Add(new ItemViewModel(RunningApps[j].StartInfo.FileName));
    //        }

    //    }
    //}

    public async void GetInitialRunningApps()
    {
        ArrayList badProcesses = new ArrayList();
        badProcesses.Add("TextInputHost");
        badProcesses.Add("svchost");

        var processes = Process.GetProcesses();
        var checkingApps = CheckRunningApplication(this);

        //foreach (Process process in processes)
        //{
        //    if (!String.IsNullOrEmpty(process.MainWindowTitle))
        //    {
        //        if (badProcesses.Contains(process.ProcessName))
        //            continue;
        //        Debug.WriteLine(process.ProcessName);
        //        RunningAppNames.Add(process.ProcessName);
        //        GeneratedDrawer.Contents.Add(
        //            new RunningItemViewModel(Id, DrawerHierarchy, GeneratedDrawer, process)
        //        );
        //    }
        //}
        await checkingApps;
    }

    //https://stackoverflow.com/questions/44602890/c-sharp-how-to-get-tabs-from-chrome-in-multiple-windows

    //Work in Progress
    public static async Task CheckRunningApplication(DrawerAsContentsViewModel dac)
    {
        ArrayList browserWindows = new ArrayList();
        while (isMonitoring)
        {
            await Task.Delay(300);
            var processes = Process.GetProcesses();

            Dictionary<string, Process> apps = new Dictionary<string, Process>();
            int i = 0;
            foreach (Process process in processes)
            {
                if (process.MainWindowHandle.ToInt32() > 0)
                { 
                    apps.Add(process.ProcessName + process.MainWindowHandle, process);
                }
                i++;
                // delaying this to prevent all the work being done at once
                // without this, dragging the dock around would lag every 1 seconds or so
                // but now we're spreading the work out
                if(i == 15)
                {
                    await Task.Delay(35);
                    i = 0;
                }
            }
            ArrayList badProcesses = new ArrayList();
            badProcesses.Add("TextInputHost");
            badProcesses.Add("svchost");
            badProcesses.Add("ApplicationFrameHost");
            bool foundApp = false;
            foreach (string appName in RunningAppNames.ToList())
            {
                foundApp = false;
                foreach(var n in apps.Keys)
                {
                    if (appName.StartsWith(n))
                    {
                        foundApp = true;
                        break;
                    }
                }
                if(foundApp)
                    continue;
                if (browserWindows.Contains(appName))
                    continue;
                RunningAppNames.Remove(appName);
                foreach (var cuvm in dac.GeneratedDrawer.Contents.ToList())
                {
                    if (cuvm is RunningItemViewModel rivm)
                    {
                        if (appName.StartsWith(rivm.ProcessName) || rivm.RunningProcess.MainWindowTitle == "")
                        {
                            rivm.DeleteMe = true;
                            Debug.WriteLine("Deleting " + appName);
                        }
                    }
                }
            }


            foreach (string appName in apps.Keys)
            {
                // Handling browsers in a seperate collection. Browsers can have multiple windows, each has a different
                // MainWindowHandle, but one process manages both of these, meaning its MainWindowHandle switches between each window
                if (IsBrowser(appName) && !browserWindows.Contains(appName))
                    browserWindows.Add(appName + apps[appName].MainWindowHandle);
                //if (!apps[appName].Responding)
                //    continue;
                foundApp = false;
                foreach (var n in RunningAppNames)
                {
                    if (appName.StartsWith(n) || n.StartsWith(appName))
                    {
                        foundApp = true;
                        break;
                    }
                }
                if (foundApp)
                    continue;

                foreach (string s in badProcesses)
                {
                    if (appName.StartsWith(s))
                        foundApp = true;
                }
                if (foundApp)
                    continue;
                if (String.IsNullOrEmpty(appName))
                    continue;
                if (String.IsNullOrEmpty(apps[appName].MainWindowTitle))
                    continue;
                if (appName.Equals("Drag"))
                    continue;
                
                Debug.WriteLine("Adding " + appName + apps[appName].MainWindowHandle);
                RunningAppNames.Add(appName + apps[appName].MainWindowHandle);
                dac.GeneratedDrawer.Contents.Add(new RunningItemViewModel(dac.Id, dac.DrawerHierarchy, dac.GeneratedDrawer, apps[appName]));
            }

            //updating the processes and names in each RunningItem
            foreach (var cuvm in dac.GeneratedDrawer.Contents )
            {
                var rivm = cuvm as RunningItemViewModel;
                if (rivm == null)
                    continue;
                if(apps.ContainsKey(rivm.RunningProcess.ProcessName + rivm.RunningProcess.MainWindowHandle))
                    rivm.UpdateProcess(apps[rivm.RunningProcess.ProcessName + rivm.RunningProcess.MainWindowHandle]);
                rivm.UpdateName();
            }
        }
    }

    public static bool IsBrowser(string name)
    {
        return name.StartsWith("chrome") || name.StartsWith("msedge") || name.StartsWith("firefox");
    }
}
