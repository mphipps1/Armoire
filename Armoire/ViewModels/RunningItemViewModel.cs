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
        Name = process.MainWindowTitle;
        ProcessName = process.ProcessName + process.MainWindowHandle;
        Icon icon;

        // Getting the icon of this app
        icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
        Avalonia.Controls.Image image = new Avalonia.Controls.Image();

        var bmp = icon.ToBitmap();
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }

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


