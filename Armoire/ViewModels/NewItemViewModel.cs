//using Windows.Management;

//using Windows.Management;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Armoire.ViewModels
{
    public partial class NewItemViewModel : ViewModelBase
    {
        public ObservableCollection<dynamic> DropCollection { get; set; } = new ObservableCollection<dynamic>();
        [ObservableProperty]
        public IBrush _borderBackground = Avalonia.Media.Brushes.Transparent;
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

        [ObservableProperty]
        public bool _isDrawer;

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

        private int TargetDrawerID;

        private int TargetDrawerHeirarchy;

        public NewItemViewModel(int targetDrawerID, int targetDrawerHeirarchy, bool isItem)
        {
            Dock = MainWindowViewModel.DockViewModel.InnerContents;
            Executables = new Dictionary<string, string>();
            ExecutableNames = new ObservableCollection<string>();
            Icons = new Dictionary<string, Icon>();
            TargetDrawerID = targetDrawerID;
            if (!Executables.Any())
            {
                GetExecutables();
                ExecutableNames = new ObservableCollection<string>(ExecutableNames.OrderBy(i => i));
            }
            IsItem = isItem;
            IsDrawer = !isItem;
            if (IsItem)
                PanelHeight = 400;
            else
                PanelHeight = 200;
            PanelWidth = 400;

            //setting the backgrounds to light gray
            BackgroundColor = "#c5c7c6";
            TargetDrawerHeirarchy = targetDrawerHeirarchy;
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
                            TargetDrawerID.ToString()
                        )
                    );
                }
                else if (DropCollection.Count > 0)
                {
                    var droppedFile = DropCollection.ElementAt(0);
                    var ExeFilePath = droppedFile.TargetPath;
                    var IconLocation = droppedFile.IconLocation;
                    var IconPath = ExeFilePath + IconLocation;
                    var name = Path.GetFileName(droppedFile.FullName).Substring(0, Path.GetFileName(droppedFile.FullName).IndexOf('.'));
                    var icon = Icon.ExtractAssociatedIcon(ExeFilePath);

                    Bitmap bitmap = icon.ToBitmap();

                    targetDrawer.Add(new ItemViewModel(name ,  ExeFilePath, bitmap ,TargetDrawerID.ToString()));

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
                    if(ret != null)
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
        }

        [RelayCommand]
        public void IsItemClicked()
        {
            IsItem = !IsItem;
        }


        [RelayCommand]
        public void RemoveFile()
        {
            DropCollection.RemoveAt(0);
            if (DropCollection.Count == 0)
            {
                FileDropText = "Drop lnk file here";
                IsPopupRemoveButton = false;
                BorderBackground = Avalonia.Media.Brushes.Transparent;
            }
        }

        public bool CheckIsItem()
        {
            return IsItem;
        }

        [RelayCommand]
        public void CloseDialog()
        {
            MainWindowViewModel.CloseDialog();
        }
    }
}
