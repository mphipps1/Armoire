//using Windows.Management;

//using Windows.Management;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels
{
    public partial class NewItemViewModel : ViewModelBase
    {
        public ObservableCollection<dynamic> lnkDropCollection { get; set; } =
            new ObservableCollection<dynamic>();


        public ObservableCollection<dynamic> ImageDropCollection { get; set; } =
            new ObservableCollection<dynamic>();


        [ObservableProperty]
        public IBrush _borderBackground = Avalonia.Media.Brushes.Transparent;

        //the following are declared in GetExecutables() so they're populated before a user goes to add a new unit
        public static Dictionary<string, string>? Executables { get; set; }
        public static ObservableCollection<string>? ExecutableNames { get; set; }
        public static Dictionary<string, Icon>? Icons { get; set; }

        private static ObservableCollection<ContentsUnitViewModel>? Dock { get; set; }

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string _iconPath;

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

        private int TargetDrawerHeirarchy;

        public NewItemViewModel(string targetDrawerID, int targetDrawerHeirarchy)
        {
            TargetDrawerID = targetDrawerID;
 
            PanelHeight = 400;
            PanelWidth = 400;

            //setting the backgrounds to light gray
            BackgroundColor = "#c5c7c6";
            TargetDrawerHeirarchy = targetDrawerHeirarchy;

            DropDownIcon = "ArrowBottomDropCircleOutline";
        }

        [RelayCommand]
        public void Update()
        {
            var targetDrawer = GetTargetDrawer(Dock);
            if (targetDrawer != null)
            {
                if (NewExe != null)
                {
                    targetDrawer.Add(
                        new ItemViewModel(
                            Name ?? NewExe,
                            Executables[NewExe],
                            Icons[NewExe].ToBitmap(),
                            TargetDrawerID,
                            TargetDrawerHeirarchy + 1
                        )
                    );
                }
                else if (lnkDropCollection.Count > 0 || ImageDropCollection.Count > 0)
                {

                    if (ImageDropCollection.Count > 0)
                    {
                        var droppedFile = ImageDropCollection.ElementAt(0);
                        string fileExtension = droppedFile.Substring(droppedFile.IndexOf('.') + 1);

                  

                            FileInfo fileInfo = new FileInfo(droppedFile);

                            var name = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));

                            var icon = Icon.ExtractAssociatedIcon(droppedFile);

                            System.Drawing.Bitmap bitmap = icon.ToBitmap();

                        var b = ApplicationMonitorViewModel.isRunning;
                        var x = ApplicationMonitorViewModel.runningApplications;

                        targetDrawer.Add(
                               new ItemViewModel(name, droppedFile, bitmap, TargetDrawerID.ToString(), TargetDrawerHeirarchy)
                           );


                        



                    }
                    else if (lnkDropCollection.Count > 0)
                    {
                        var droppedFile = lnkDropCollection.ElementAt(0);
                        var ExeFilePath = droppedFile.TargetPath;
                        var IconLocation = droppedFile.IconLocation;
                        var IconPath = ExeFilePath + IconLocation;
                        var name = Path.GetFileName(droppedFile.FullName)
                            .Substring(0, Path.GetFileName(droppedFile.FullName).IndexOf('.'));
                        var icon = Icon.ExtractAssociatedIcon(ExeFilePath);

                       System.Drawing.Bitmap bitmap = icon.ToBitmap();

                        targetDrawer.Add(
                            new ItemViewModel(name, ExeFilePath, bitmap, TargetDrawerID, TargetDrawerHeirarchy)
                        );
                    }
                }
                else
                {
                    var newDrawer = new DrawerAsContentsViewModel(Name, IconPath, TargetDrawerID, TargetDrawerHeirarchy + 1);
                    targetDrawer.Add(newDrawer);
                }
            }

            //MainWindowViewModel._dialogIsOpen = false;
            MainWindowViewModel.CloseDialog();
            NewExe = null;
        }

        private ObservableCollection<ContentsUnitViewModel>? GetTargetDrawer(
            ObservableCollection<ContentsUnitViewModel> currentDrawer
        )
        {
            if (TargetDrawerID == "CONTENTS_1")
                return Dock;
            foreach (var unit in currentDrawer)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Id == TargetDrawerID)
                    {
                        return dacvm.GeneratedDrawer.Contents;
                    }
                }
            }
            foreach (var unit in currentDrawer)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    var ret = GetTargetDrawer(dacvm.GeneratedDrawer.Contents);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
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
            ExecutableNames = new ObservableCollection<string>(ExecutableNames.OrderBy(i => i));
        }


        [RelayCommand]
        public void RemoveFile()
        {
            if (ImageDropCollection.Count > 0)
            {
                ImageDropCollection.RemoveAt(0);
            }
            else if (lnkDropCollection.Count > 0)
            {
                lnkDropCollection.RemoveAt(0);
            }

            if (lnkDropCollection.Count == 0 && ImageDropCollection.Count == 0 )
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
