/*  A RunningItem represents an application that is currently running on this device
 *  This class contains info about the process that it represents along with the ability to close it.
 *  
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels;

public partial class RunningItemViewModel : ItemViewModel
{
    public Process RunningProcess { get; set; }
    public string ProcessName { get; set; }

    public RunningItemViewModel(
        string parentID,
        int? drawerHierarchy,
        ContainerViewModel? container,
        Process process
    )
        : base(parentID, drawerHierarchy, container)
    {
        RunningProcess = process;
        ExecutablePath = "";

        // The name is what is displayed as a tool tip to the user,
        // the process name is used to identify unique processes such as two sepereate command prompts
        Name = process.ProcessName;
        ProcessName = process.ProcessName + process.MainWindowHandle.ToString();
        
        // Getting the icon of this app
        Avalonia.Controls.Image image = new Avalonia.Controls.Image();
        var iconBitmap = GetCurrentProcessIcon(process);
        image.Source = iconBitmap;

        //Special ID to prevent being added to the database
        Id = "RUNNING";
    }

    // pinvoke function that brings the process that has the MainWindowHandle hWnd to the top of the screen
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool BringWindowToTop(IntPtr hWnd);

    //Constant values used ub showing windows
    //showWindow documentation: https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_SHOW = 5;

    // pinvoke function to show the window if it was minized
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    //Show the window and bring to to the top if this RunningItem is clicked
    public override void HandleContentsClick()
    {
        //IntPtr hWnd = FindWindow(null, RunningProcess);
        Debug.WriteLine(RunningProcess.ProcessName + " " + RunningProcess.MainWindowHandle);

        // SW_SHOWMAXIMIZED to maximize the window
        // SW_SHOWMINIMIZED to minimize the window
        // SW_SHOWNORMAL to make the window be normal size

        if (RunningProcess.ProcessName.Equals("Armoire"))
            ShowWindow(RunningProcess.MainWindowHandle, SW_SHOWNORMAL);
        else
            ShowWindow(RunningProcess.MainWindowHandle, SW_SHOW);
        BringWindowToTop(RunningProcess.MainWindowHandle);
        //SetForegroundWindow(RunningProcess.MainWindowHandle);
    }
    public void UpdateName()
    {
        Name = RunningProcess.MainWindowTitle;
    }
    
    /*
     * This function gets the icons of the currently running item the cross platform way.
     */
    public Avalonia.Media.Imaging.Bitmap? GetCurrentProcessIcon(Process process)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                var fileName = process.MainModule?.FileName;
                if (fileName == null) return null;

                var icon = Icon.ExtractAssociatedIcon(fileName);
                if (icon == null) return null;

                using var bitmap = icon.ToBitmap();
                using var memory = new MemoryStream();
                bitmap.Save(memory, ImageFormat.Png);
                memory.Seek(0, SeekOrigin.Begin);
                return new Avalonia.Media.Imaging.Bitmap(memory);
            }
            catch
            {
                return null;
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            try
            {
                var fileName = process.MainModule?.FileName;
                if (fileName == null) return null;

                /*
                 * macOS .app bundles its icons within the .app file. So in order to get the icons we do the following:
                 * We get the app bundle location then we can grab the icons out of the path that they live in which is nested in contens/resources/
                 * from there we can just search for all .icns files to find one that matches the current fileName
                 */
                var appBundle = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(fileName)));
                var resourcesPath = Path.Combine(appBundle ?? "", "Contents", "Resources");
                var icons = Directory.Exists(resourcesPath)
                    ? Directory.GetFiles(resourcesPath, "*.icns").FirstOrDefault()
                    : null;

                return icons != null ? new Avalonia.Media.Imaging.Bitmap(icons) : null;
            }
            catch { return null; }
        }
        //If OS is Linux we have to try different paths for where applications live on linux until we find the currently running one.
        else
        {
            try
            {
                var processName = process.ProcessName;
                if (processName == null) return null;

                // I found these filepaths on Ubuntu 24.04 I used the 3 most common icon sizes that I found on my system.
                // add new filepaths for linux icons here if needed.
                var possibleIconFilePaths = new[]
                {
                    $"/usr/share/icons/hicolor/256x256/apps/{processName}.png",
                    $"/usr/share/icons/hicolor/128x128/apps/{processName}.png",
                    $"/usr/share/icons/hicolor/48x48/apps/{processName}.png",
                };

                var iconFilePath = possibleIconFilePaths.FirstOrDefault(File.Exists);
                return iconFilePath != null ? new Avalonia.Media.Imaging.Bitmap(iconFilePath) : null;
            }
            catch { return null; }
        }
    }
    
    public void UpdateProcess(Process p)
    {
        RunningProcess = p;
    }

    [RelayCommand]
    public void EndProcess()
    {
        RunningProcess.Kill();
    }
}


