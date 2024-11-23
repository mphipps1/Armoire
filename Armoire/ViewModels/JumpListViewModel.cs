using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Armoire.ViewModels
{
    public static class JumpListViewModel
    {
        private static List<FileInfo> fileInfos = new List<FileInfo>();

        public static List<ItemViewModel> Items = new List<ItemViewModel>();

        public static List<string> lnkpaths = new List<string>();   








        public static void GetJumpList()
        {
            string userName = Environment.UserName;
            string directloc = $"C:\\Users\\{userName}\\AppData\\Roaming\\Microsoft\\Windows\\Recent";

            // files list from the root directory and prints it
            string[] fyles = Directory.GetFiles(directloc);
            string file;
            for (int i = 0; i < fyles.Length; i++)
            {

                fileInfos.Add(new FileInfo(fyles[i]));
                Type shellObjectType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic windowsShell = Activator.CreateInstance(shellObjectType);
                if (fileInfos.ElementAt(i).Extension.Equals(".lnk"))
                {
                    dynamic shortcut = windowsShell.CreateShortcut(fileInfos.ElementAt(i).FullName);
                    lnkpaths.Add(fyles[i]);
                }

                //   var file = shortcut.TargetPath;
                string applicationName = Path.GetFileNameWithoutExtension(fileInfos.ElementAt(i).FullName);
            }

         }




        public static string GetOpenWithAppName()
        {

            string autoDestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                            @"Microsoft\Windows\Recent\AutomaticDestinations");

            foreach (var file in Directory.GetFiles(autoDestPath, "*.automaticDestinations-ms"))
            {
                Console.WriteLine($"Jump List File: {Path.GetFileName(file)}");
            }


            return "";
        }
               
    }

        /**

        public static uint AccessJumpList(string appId)
        {
            // Create a CustomDestinationList instance
            var destinationList = (ICustomDestinationList)new CustomDestinationList();

            try
            {
                Guid emptyGuid = Guid.Empty;
                // Set the AppID for the jump list
                destinationList.SetAppID(appId);

                // Begin the jump list and retrieve the number of slots
                destinationList.BeginList(out uint availableSlots, ref emptyGuid, out object collection);

                Console.WriteLine($"Jump List accessed successfully. Available slots: {availableSlots}");

                // You can process the 'collection' object here for advanced scenarios (e.g., IObjectArray)
                return availableSlots;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing jump list: {ex.Message}");
                throw;
            }
        }
        // Retrieves the AppID from a shortcut
        public static string GetAppIDFromShortcut(string shortcutPath)
        {
            if (!System.IO.File.Exists(shortcutPath))
                throw new System.IO.FileNotFoundException("Shortcut not found", shortcutPath);

            var shellLink = (IShellLinkW)new ShellLink();
            var persistFile = (System.Runtime.InteropServices.ComTypes.IPersistFile)shellLink;

            // Load the shortcut
            persistFile.Load(shortcutPath, 0);

            // Access the property store
            const int STGM_READ = 0;
            Guid guidPropertyStore = new Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
            var propertyStore = (IPropertyStore)shellLink;
            PropertyKey appIdKey = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5); // PKEY_AppUserModel_ID

            // Query the AppID property
            PropVariant value;
            propertyStore.GetValue(ref appIdKey, out value);

            return value.Value as string;
        }
    }


        [ComImport]
    [Guid("77f10cf0-3db5-4966-b520-b7c54fd35ed6")]
    internal class CustomDestinationList
    {
    }

    // ICustomDestinationList interface
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6332debf-87b5-4670-90c0-5e57b408a49e")]
    internal interface ICustomDestinationList
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        void BeginList(out uint cMinSlots, ref Guid riid, out object ppvObject);
        void AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, object poa);
        void AppendKnownCategory(KnownDestCategory category);
        void AddUserTasks(object poa);
        void CommitList();
        void GetRemovedDestinations(ref Guid riid, out object ppvObject);
        void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        void AbortList();
    }

    // KnownDestCategory enumeration
    internal enum KnownDestCategory
    {
        Frequent = 1,
        Recent
    }







    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink { }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLinkW
    {
        void GetPath([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, ref IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }


    [StructLayout(LayoutKind.Sequential)]
    internal struct PropertyKey
    {
        public PropertyKey(Guid fmtid, uint pid)
        {
            this.fmtid = fmtid;
            this.pid = pid;
        }

        public Guid fmtid;
        public uint pid;
    }

    // Helper struct for PropVariant
    [StructLayout(LayoutKind.Sequential)]
    internal struct PropVariant
    {
        public short vt;
        public IntPtr wReserved1;
        public IntPtr wReserved2;
        public IntPtr wReserved3;
        public IntPtr pointerValue;

        public object Value
        {
            get
            {
                if (vt == 31) // VT_LPWSTR
                {
                    return Marshal.PtrToStringUni(pointerValue);
                }

                return null;
            }
        }
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
     internal interface IPropertyStore
    {
        void GetCount(out uint propertyCount);
        void GetAt(uint propertyIndex, out PropertyKey key);
        void GetValue(ref PropertyKey key, out PropVariant pv);
        void SetValue(ref PropertyKey key, ref PropVariant pv);
        void Commit();
    }

        */


    }




