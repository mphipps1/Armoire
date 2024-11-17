using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public partial class RunningItemViewModel : ItemViewModel
    {

        private Process Process { get; set; }
        public RunningItemViewModel(string parentID, int drawerHierarchy, ContainerViewModel? container, Process process)
            : base(parentID, drawerHierarchy, container)
        {
            Process = process;
            ExecutablePath = "";
            Name = process.ProcessName;
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
        }

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;


        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("User32")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static string GetMainModuleFileName(Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                null;
        }

        public static IntPtr FindWindow(string titleName)
        {
            Process[] pros = Process.GetProcesses(".");
            foreach (Process p in pros)
                if (p.MainWindowTitle.ToUpper().Contains(titleName.ToUpper()))
                    return p.MainWindowHandle;
            return new IntPtr();
        }

        public override void HandleContentsClick()
        {
            IntPtr hWnd = FindWindow(Process.ProcessName);
            if (!hWnd.Equals(IntPtr.Zero))
            {
                // SW_SHOWMAXIMIZED to maximize the window
                // SW_SHOWMINIMIZED to minimize the window
                // SW_SHOWNORMAL to make the window be normal size
                ShowWindowAsync(hWnd, SW_SHOWMAXIMIZED);
                SetForegroundWindow(hWnd);
            }
        }
    }
}
