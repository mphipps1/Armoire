using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
//using Windows.Management;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Armoire.Interfaces;
using Armoire.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using Microsoft.Win32;

namespace Armoire.ViewModels
{
    public partial class NewItemViewModel : ViewModelBase
    {
        public static Dictionary<string, string> Executables { get; set; }
        public static ObservableCollection<string> ExecutableNames { get; set; }


        private ObservableCollection<ContentsUnitViewModel> Dock { get; set; }

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string _iconPath;

        [ObservableProperty]
        public string _newExe;

        private int TargetDrawerID;

        public NewItemViewModel(int targetDrawerID)
        {
            Dock = MainWindowViewModel.DockContents;
            Executables = new Dictionary<string, string>();
            ExecutableNames = new ObservableCollection<string>();
            TargetDrawerID = targetDrawerID;
            if (!Executables.Any())
                GetExecutables();
        }

        [RelayCommand]
        public void Update()
        {
            var targetDrawer = GetTargetDrawer(Dock);
            if (targetDrawer != null) 
                targetDrawer.Add(new ItemViewModel(Name, Executables[NewExe], TargetDrawerID.ToString()));
        }

        private ObservableCollection<ContentsUnitViewModel>? GetTargetDrawer(ObservableCollection<ContentsUnitViewModel> currentDrawer)
        {
            foreach(var unit in currentDrawer)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if(dacvm.Id == TargetDrawerID)
                    {
                         return dacvm.DrawerAsContainer.Contents;
                    }
                }
            }
            foreach(var unit in currentDrawer)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    var ret = GetTargetDrawer(dacvm.DrawerAsContainer.Contents);
                    return ret;
                }
            }
            return null;
        }

        public void GetExecutables()
        {
            //the following only works on windows
            string registry_key_32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            string registry_key_64 =
                @"SOFTWARE\WoW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

            Microsoft.Win32.RegistryKey key_32 = Registry.LocalMachine.OpenSubKey(registry_key_32);
            Microsoft.Win32.RegistryKey key_64 = Registry.LocalMachine.OpenSubKey(registry_key_64);

            foreach (string subkey_name in key_32.GetSubKeyNames())
            {
                using (RegistryKey subkey = key_32.OpenSubKey(subkey_name))
                {
                    if (!subkey.Name.Contains("{") && !Executables.ContainsKey(subkey_name))
                    {
                        string path = (string)subkey.GetValue("InstallLocation");
                        if (path != null && path != "")
                            Executables.Add(
                                subkey_name,
                                path + "\\" + MatchFileName(path, subkey_name)
                            );
                    }
                }
            }

            foreach (string subkey_name in key_64.GetSubKeyNames())
            {
                using (RegistryKey subkey = key_64.OpenSubKey(subkey_name))
                {
                    if (!subkey.Name.Contains("{") && !Executables.ContainsKey(subkey_name))
                    {
                        string path = (string)subkey.GetValue("InstallLocation");
                        if (path != null)
                        {
                            path = path.Replace("\"", "");
                            Executables.Add(
                                subkey_name,
                                path + "\\" + MatchFileName(path, subkey_name)
                            );
                        }
                    }
                }
            }

            foreach (string subkey_name in key_32.GetSubKeyNames())
            {
                using (RegistryKey subkey = key_32.OpenSubKey(subkey_name))
                {
                    if (!subkey.Name.Contains("{") && !Executables.ContainsKey(subkey_name))
                    {
                        string path = (string)subkey.GetValue("InstallLocation");
                        if (path != null)
                            Executables.Add(
                                subkey_name,
                                path + "\\" + MatchFileName(path, subkey_name)
                            );
                    }
                }
            }

            foreach (string subkey_name in key_64.GetSubKeyNames())
            {
                using (RegistryKey subkey = key_64.OpenSubKey(subkey_name))
                {
                    if (!subkey.Name.Contains("{") && !Executables.ContainsKey(subkey_name))
                    {
                        string path = (string)subkey.GetValue("InstallLocation");
                        if (path != null)
                            Executables.Add(
                                subkey_name,
                                path + "\\" + MatchFileName(path, subkey_name)
                            );
                    }
                }
            }
            foreach (KeyValuePair<string, string> kv in Executables)
            {
                ExecutableNames.Add(kv.Key);
            }

            //Windows.Management.Deployment.PackageManager packageManager = new Windows.Management.Deployment.PackageManager();
            // IEnumerable<Windows.ApplicationModel.Package> packages = (IEnumerable<Windows.ApplicationModel.Package>)packageManager.FindPackages();
        }

        //this file takes the full path of the application folder and checks the last folder matches any executable names
        //an example of this is as follows:
        // path = "C:\Program Files\Mozilla Firefox", subkey_name =

        public string MatchFileName(string path, string subkey_name)
        {
            if (path != null)
            {
                string exeName = "";
                if (path.AsSpan() == "")
                    return "";
                var files = Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    //gets the filename and the .exe from file, which is a full
                    string fileAndExtension;
                    var ret = System.IO.Path.GetFullPath(file);
                    if (ret.AsSpan() == "")
                        continue;
                    fileAndExtension = System.IO.Path.GetFileName(ret);
                    //checks if the subkey_name contains the name of the executable file without the extension
                    if (
                        subkey_name
                            .ToLower()
                            .Contains(
                                fileAndExtension.Substring(0, fileAndExtension.Length - 4).ToLower()
                            )
                    )
                    {
                        exeName = fileAndExtension;
                        return exeName;
                    }
                }
            }
            return "";
        }
    }
}
