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
        Name = process.MainWindowTitle;
        ProcessName = process.ProcessName + process.MainWindowHandle;
        Icon icon;
        string s;
        try
        {
            s = GetMainModuleFileName(process, 1024);
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            return;
        }
        if (!String.IsNullOrEmpty(s))
        {
            icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            Avalonia.Controls.Image image = new Avalonia.Controls.Image();

            var bmp = icon.ToBitmap();
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
            }
        }

        Id = "RUNNING";
    }

    //showWindow documentation: https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_SHOW = 5;

    [DllImport("Kernel32.dll")]
    private static extern bool QueryFullProcessImageName(
        [In] IntPtr hProcess,
        [In] uint dwFlags,
        [Out] StringBuilder lpExeName,
        [In, Out] ref uint lpdwSize
    );

    public static string? GetMainModuleFileName(Process process, int buffer = 1024)
    {
        var fileNameBuilder = new StringBuilder(buffer);
        uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
        return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength)
            ? fileNameBuilder.ToString()
            : null;
    }

    //[DllImport("user32.dll", SetLastError = true)]
    //static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    //// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.

    //[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    //static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

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


