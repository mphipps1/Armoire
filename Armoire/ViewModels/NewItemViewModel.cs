//using Windows.Management;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using DialogHostAvalonia;

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
using Microsoft.Win32;
using System.Drawing;

namespace Armoire.ViewModels
{
    public partial class NewItemViewModel : ViewModelBase
    {
        public static Dictionary<string, string> Executables { get; set; }
        public static ObservableCollection<string> ExecutableNames { get; set; }
        public static Dictionary<string, Icon> Icons { get; set; }

        private ObservableCollection<ContentsUnitViewModel> Dock { get; set; }

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string _iconPath;

        [ObservableProperty]
        public string _newExe;

        [ObservableProperty]
        public bool _isItem;

        private int TargetDrawerID;

        private int TargetDrawerHeirarchy;

        public NewItemViewModel(int targetDrawerID, int targetDrawerHeirarchy)
        {
            Dock = MainWindowViewModel.DockContents;
            Executables = new Dictionary<string, string>();
            ExecutableNames = new ObservableCollection<string>();
            Icons = new Dictionary<string, Icon>();
            TargetDrawerID = targetDrawerID;
            if (!Executables.Any())
                GetExecutables();
            IsItem = false;
            TargetDrawerHeirarchy = targetDrawerHeirarchy;
        }

        [RelayCommand]
        public void Update()
        {
            var targetDrawer = GetTargetDrawer(Dock);
            if (targetDrawer != null)

            {
                if (IsItem)
                {
                    targetDrawer.Add(new ItemViewModel(Name, Executables[NewExe], Icons[NewExe].ToBitmap(), TargetDrawerID.ToString()));
                }
                else
                {
                    var newDrawer = new DrawerAsContentsViewModel(Name, IconPath);
                    newDrawer.DrawerHierarchy = TargetDrawerHeirarchy + 1;
                    targetDrawer.Add(newDrawer);
                }
            }

            //MainWindowViewModel._dialogIsOpen = false;
            MainWindowViewModel.CloseDialog();
        }

        private ObservableCollection<ContentsUnitViewModel>? GetTargetDrawer(
            ObservableCollection<ContentsUnitViewModel> currentDrawer
        )
        {
            if (TargetDrawerID == 0)
                return Dock;
            foreach (var unit in currentDrawer)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Id == TargetDrawerID)
                    {
                        return dacvm.InnerContainer.InnerContents;
                    }
                }
            }
            foreach (var unit in currentDrawer)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    var ret = GetTargetDrawer(dacvm.InnerContainer.InnerContents);
                    return ret;
                }
            }
            return null;
        }

        public void GetExecutables()
        {
            string directoryPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";
           
            if (Directory.Exists(directoryPath))
            {
                foreach (string file in Directory.EnumerateFiles(directoryPath, "*.lnk", SearchOption.AllDirectories))
                {
                    if (!Executables.ContainsKey(System.IO.Path.GetFileNameWithoutExtension(file)))
                    {
                        string shortcut = System.IO.Path.GetFileNameWithoutExtension(file);
                        Executables.Add(shortcut, file);
                        ExecutableNames.Add(shortcut);
                        Icons.Add(shortcut, Icon.ExtractAssociatedIcon(file));
                    }
                }
            }
        }

        [RelayCommand]
        public void IsItemClicked()
        {
            IsItem = !IsItem;
        }

        [RelayCommand]
        public void CloseDialog()
        {
            MainWindowViewModel.CloseDialog();
        }
    }
}
