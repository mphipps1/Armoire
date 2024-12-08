//using Windows.Management;

//using Windows.Management;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Armoire.ViewModels
{
    public partial class NewItemViewModel : ViewModelBase
    {
        public ObservableCollection<string> lnkDropCollection { get; set; } =
            new ObservableCollection<string>();

        [ObservableProperty]
        public IBrush _borderBackground = Avalonia.Media.Brushes.Transparent;

        //the following are declared in GetExecutables() so they're populated before a user goes to add a new unit
        public static Dictionary<string, string>? Executables { get; set; }
        public static ObservableCollection<string>? ExecutableNames { get; set; }
        public static Dictionary<string, Icon>? Icons { get; set; }

        private static ObservableCollection<ContentsUnitViewModel>? Dock { get; set; }

        [ObservableProperty]
        public string _name;

        public static string? NewExe;

        [ObservableProperty]
        public int _panelHeight;

        [ObservableProperty]
        public int _panelWidth;

        [ObservableProperty]
        public string _backgroundColor;

        [ObservableProperty]
        public string _fileDropText = "Drop lnk file here";

        [ObservableProperty]
        public bool _isPopupRemoveButton;

        [ObservableProperty]
        public bool _exePopUpOpen;

        [ObservableProperty]
        public string _dropDownIcon;

        private string TargetDrawerID;

        private int? TargetDrawerHeirarchy;

        private ContainerViewModel? ActiveContainerViewModel { get; }

        public NewItemViewModel(
            string targetDrawerID,
            int? targetDrawerHeirarchy,
            ContainerViewModel? cvm = null
        )
        {
            Name = "";
            TargetDrawerID = targetDrawerID;

            PanelHeight = 345;
            PanelWidth = 400;

            //setting the backgrounds to light gray
            BackgroundColor = "#c5c7c6";
            TargetDrawerHeirarchy = targetDrawerHeirarchy;

            DropDownIcon = "ArrowBottomDropCircleOutline";

            ActiveContainerViewModel = cvm;
        }

        [RelayCommand]
        public void Update()
        {
            var targetDrawer = GetTargetDrawer(MainWindowViewModel.ActiveDockViewModel);
            if (targetDrawer != null)
            {
                if (NewExe != null && Executables.ContainsKey(NewExe))
                {
                    targetDrawer.RegisterEventHandlers();
                    targetDrawer.Contents.Add(
                        new ItemViewModel(
                            Name ?? NewExe,
                            Executables[NewExe],
                            Icons[NewExe].ToBitmap(),
                            TargetDrawerID,
                            TargetDrawerHeirarchy + 1,
                            ActiveContainerViewModel,
                            NewExe
                        )
                    );

                    if (targetDrawer.SourceDrawer.DrawerHierarchy == -1)
                        targetDrawer.Contents.Move(
                            targetDrawer.Contents.Count - 1,
                            targetDrawer.Contents.Count - 4
                        );
                    Name = NewExe;
                }
                else if (NewExe != null)
                {
                    targetDrawer.RegisterEventHandlers();
                    targetDrawer.Contents.Add(
                        new ItemViewModel(
                            Name ?? NewExe,
                            NewExe,
                            Icon.ExtractAssociatedIcon(NewExe).ToBitmap(),
                            TargetDrawerID,
                            TargetDrawerHeirarchy + 1,
                            ActiveContainerViewModel,
                            NewExe
                        )
                    );
                    Name = NewExe;
                }
                else if (lnkDropCollection.Count > 0)
                {
                    if (lnkDropCollection.Count > 0)
                    {
                        var droppedFile = lnkDropCollection.ElementAt(0);
                        var ExeFilePath = droppedFile;
                        var IconLocation = droppedFile;
                        var IconPath = ExeFilePath + IconLocation;
                        var name = Path.GetFileName(droppedFile)
                            .Substring(0, Path.GetFileName(droppedFile).IndexOf('.'));
                        var icon = Icon.ExtractAssociatedIcon(ExeFilePath);

                        System.Drawing.Bitmap bitmap = icon.ToBitmap();

                        targetDrawer.RegisterEventHandlers();
                        targetDrawer.Contents.Add(
                            new ItemViewModel(
                                name,
                                ExeFilePath,
                                bitmap,
                                TargetDrawerID,
                                TargetDrawerHeirarchy,
                                ActiveContainerViewModel
                            )
                        );
                    }
                }
            }

            //MainWindowViewModel._dialogIsOpen = false;
            MainWindowViewModel.CloseDialog();
            NewExe = null;
        }

        private ContainerViewModel? GetTargetDrawer(ContainerViewModel currentDrawer)
        {
            if (TargetDrawerID == "CONTENTS_1")
                return MainWindowViewModel.ActiveDockViewModel;
            foreach (var unit in currentDrawer.Contents)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Id == TargetDrawerID)
                    {
                        return dacvm.GeneratedDrawer;
                    }
                }
            }
            foreach (var unit in currentDrawer.Contents)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    var ret = GetTargetDrawer(dacvm.GeneratedDrawer);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
        }

        [RelayCommand]
        public void OnOpenFileDialogClick()
        {
            //var window = TopLevel.GetTopLevel(this) as Window;

            var dialog = new OpenFileDialog();
            dialog.Filter = "*.lnk | *.exe";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.Multiselect = true;
            dialog.Title = "Select application(s) or drag and drop...";

            var result = dialog.ShowDialog();

            string[] filePaths = dialog.FileNames;
            string[] fileNames = dialog.SafeFileNames;
            var targetDrawer = GetTargetDrawer(MainWindowViewModel.ActiveDockViewModel);
            if (filePaths.Length == 0)
                return;

            if (targetDrawer == null)
                return;
            for (int i = 0; i < filePaths.Length; i++)
            {
                targetDrawer.RegisterEventHandlers();
                lnkDropCollection.Add(filePaths[i]);
            }

            Name = Path.GetFileNameWithoutExtension(fileNames[0]);

            //if (result != null && result.Length != 0)
            //    NewItemViewModel.NewExe = result[0];
        }

        public static void GetExecutables()
        {
            Dock = MainWindowViewModel.ActiveDockViewModel.Contents;
            Executables = new Dictionary<string, string>();
            ExecutableNames = new ObservableCollection<string>();
            Icons = new Dictionary<string, Icon>();
            string directoryPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs";

            if (Directory.Exists(directoryPath))
            {
                foreach (
                    string file in Directory.EnumerateFiles(
                        directoryPath,
                        "*.lnk",
                        SearchOption.AllDirectories
                    )
                )
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

            directoryPath =
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\Microsoft\Windows\Start Menu\Programs";
            if (Directory.Exists(directoryPath))
            {
                foreach (
                    string file in Directory.EnumerateFiles(
                        directoryPath,
                        "*.lnk",
                        SearchOption.AllDirectories
                    )
                )
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

            ExecutableNames = new ObservableCollection<string>(ExecutableNames.OrderBy(i => i));
        }

        [RelayCommand]
        public void RemoveFile()
        {
            if (lnkDropCollection.Count > 0)
            {
                lnkDropCollection.RemoveAt(0);
            }

            if (lnkDropCollection.Count == 0)
            {
                FileDropText = "Drop lnk file here";
                IsPopupRemoveButton = false;
                BorderBackground = Avalonia.Media.Brushes.Transparent;
            }
        }

        [RelayCommand]
        public void ToggleExeDropDown()
        {
            ExePopUpOpen = !ExePopUpOpen;
            if (ExePopUpOpen)
                DropDownIcon = "ArrowTopDropCircleOutline";
            else
                DropDownIcon = "ArrowBottomDropCircleOutline";
        }

        [RelayCommand]
        public void CloseDialog()
        {
            MainWindowViewModel.CloseDialog();
        }
    }
}
