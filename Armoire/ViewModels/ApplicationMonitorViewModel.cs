/* ApplicationMonitor is the drawer responsible for holding a collection of RunningItems
 * It inherets from DrawerAsContents
 * 
 * It loops over the list of all processes running on this computer, then narrows down this
 * list based on its MainWindowHandle and MainWindowTitle to find processes that should be displayed
 * for the user to maximize or close.
 * 
 * Browsers are difficult to handle. Say there are two groups of tabs open. For chrome, only one process would have a MainWindowHandle,
 * and that is whichever group of tabs was most recently clicked on. Swapping between chrome windows causes the one that is no longer clicked
 * on to be deleted. To solve this, browers are handled in their own collection, and arent deleted at the moment.
 */



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
    //The names of the currently running apps, used to store the list of apps that are currently added in this drawer
    //Used so that we dont need to manually check each RunningItems Processes MainWindowTitle every time
    public static List<string> RunningAppNames { get; set; } = new List<string>();

    //Boolean used to infinately loop
    private static bool isMonitoring = true;

    public ApplicationMonitorViewModel(string? parentID, int drawerHierarchy)
        : base(parentID, drawerHierarchy, true)
    {
        Name = "Running Applications";
        //This Id is special and serves as a flag so this drawer isnt added to the database
        Id = "MONITOR";
    }

    // This function uses async and await to start the infinate loop of checking processes
    public async void GetInitialRunningApps()
    {
        var processes = Process.GetProcesses();
        var checkingApps = CheckRunningApplication(this);
        await checkingApps;
    }

    // CheckRunningApplication infinately loops to check for changes in the currentl running processes
    public static async Task CheckRunningApplication(DrawerAsContentsViewModel dac)
    {
        ArrayList browserWindows = new ArrayList();
        while (isMonitoring)
        {
            await Task.Delay(300);
            var processes = Process.GetProcesses();

            // Creating a dictionary to hold apps who have a MainWindowHandle (apps that can be maximized)
            Dictionary<string, Process> apps = new Dictionary<string, Process>();
            int i = 0;
            foreach (Process process in processes)
            {
                if (process.MainWindowHandle.ToInt32() > 0)
                {
                    apps.Add(process.ProcessName + process.MainWindowHandle, process);
                }
                i++;
                // delaying here to prevent all the delaying being done at once
                // without this, dragging the dock around would lag every 1 seconds or so
                // but now we're spreading the work out
                if (i == 15)
                {
                    await Task.Delay(35);
                    i = 0;
                }
            }

            // A badProcess is defined as a process that has a MainWindowTitle that we dont want to display
            // Any processes that shouldn't be added to the list of running apps should be added here
            ArrayList badProcesses = new ArrayList();
            badProcesses.Add("TextInputHost");
            badProcesses.Add("svchost");
            badProcesses.Add("ApplicationFrameHost");
            bool foundApp = false;

            // This loop checks to see if any apps in our drawer have been closed
            foreach (string appName in RunningAppNames.ToList())
            {
                // Using a loop and flag to search with StartsWith as some MainWindowHandles were truncated occassionally
                // This mainly occured with FileExplorer, so it might not still be neccissary 
                foundApp = false;
                foreach (var n in apps.Keys)
                {
                    if (appName.StartsWith(n))
                    {
                        foundApp = true;
                        break;
                    }
                }
                if (foundApp)
                    continue;

                //Check to see if its a brower window
                if (browserWindows.Contains(appName))
                    continue;

                //If we make it this far, delete the app
                RunningAppNames.Remove(appName);
                foreach (var cuvm in dac.GeneratedDrawer.Contents.ToList())
                {
                    if (cuvm is RunningItemViewModel rivm)
                    {
                        if (
                            appName.StartsWith(rivm.ProcessName)
                            || rivm.RunningProcess.MainWindowTitle == ""
                        )
                        {
                            rivm.DeleteMe = true;
                            Debug.WriteLine("Deleting " + appName);
                        }
                    }
                }
            }

            //Adding any new apps
            foreach (string appName in apps.Keys)
            {
                // Handling browsers in a seperate collection. Browsers can have multiple windows, each has a different
                // MainWindowHandle, but one process manages both of these, meaning its MainWindowHandle switches between each window
                if (IsBrowser(appName) && !browserWindows.Contains(appName))
                    browserWindows.Add(appName + apps[appName].MainWindowHandle);

                // Checking with a flag and loop as some names were truncated occassionally
                // The logic here could be improved
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
                dac.GeneratedDrawer.Contents.Add(
                    new RunningItemViewModel(
                        dac.Id,
                        dac.DrawerHierarchy,
                        dac.GeneratedDrawer,
                        apps[appName]
                    )
                );
            }

            // Updating the processes and names in each RunningItem
            foreach (var cuvm in dac.GeneratedDrawer.Contents.ToList())
            {
                var rivm = cuvm as RunningItemViewModel;
                if (rivm == null)
                    continue;
                if (rivm.RunningProcess.HasExited)
                {
                    dac.GeneratedDrawer.Contents.Remove(cuvm);
                    continue;
                }

                if (
                    apps.ContainsKey(
                        rivm.RunningProcess.ProcessName + rivm.RunningProcess.MainWindowHandle
                    )
                )
                    rivm.UpdateProcess(
                        apps[rivm.RunningProcess.ProcessName + rivm.RunningProcess.MainWindowHandle]
                    );
                rivm.UpdateName();
            }
        }
    }

    // Check to see if the current name is a browser
    public static bool IsBrowser(string name)
    {
        return name.StartsWith("chrome") || name.StartsWith("msedge") || name.StartsWith("firefox");
    }
}
