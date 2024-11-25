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
    public partial class NewDrawerViewModel : ViewModelBase
    {
        public ObservableCollection<dynamic> ImageDropCollection { get; set; } =
            new ObservableCollection<dynamic>();

        [ObservableProperty]
        public IBrush _borderBackground = Avalonia.Media.Brushes.Transparent;

        private static ObservableCollection<ContentsUnitViewModel>? Dock { get; set; }

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string _iconPath;

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

        public NewDrawerViewModel(
            string targetDrawerID,
            int? targetDrawerHeirarchy,
            ContainerViewModel? cvm = null
        )
        {
            TargetDrawerID = targetDrawerID;
            PanelHeight = 400;
            PanelWidth = 400;

            //setting the backgrounds to light gray
            BackgroundColor = "#c5c7c6";
            TargetDrawerHeirarchy = targetDrawerHeirarchy;

            Dock = MainWindowViewModel.ActiveDockViewModel.Contents;
            DropDownIcon = "ArrowBottomDropCircleOutline";
            ActiveContainerViewModel = cvm;
        }

        [RelayCommand]
        public void Update()
        {
            var targetDrawer = GetTargetDrawer(MainWindowViewModel.ActiveDockViewModel);
            if (targetDrawer != null)
            {
                if (ImageDropCollection.Count > 0)
                {
                    var droppedFile = ImageDropCollection.ElementAt(0);
                    string fileExtension = droppedFile.Substring(droppedFile.IndexOf('.') + 1);
                    FileInfo fileInfo = new FileInfo(droppedFile);
                    if (Name == null || Name == "")
                        Name = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));
                    //var icon = Icon.ExtractAssociatedIcon(droppedFile);
                    //System.Drawing.Bitmap bitmap = icon.ToBitmap();

                    Image image = Image.FromFile(fileInfo.FullName);
                    targetDrawer.RegisterEventHandlers();
                    targetDrawer.Contents.Add(
                        new DrawerAsContentsViewModel(
                            Name,
                            new System.Drawing.Bitmap(image, 60, 60),
                            TargetDrawerID.ToString(),
                            TargetDrawerHeirarchy,
                            ActiveContainerViewModel
                        )
                    );
                }
                else
                {
                    var newDrawer = new DrawerAsContentsViewModel(
                        Name,
                        "./../../../Assets/tempDrawer.jpg",
                        TargetDrawerID,
                        TargetDrawerHeirarchy + 1,
                        ActiveContainerViewModel
                    );
                    targetDrawer.RegisterEventHandlers();
                    targetDrawer.Contents.Add(newDrawer);
                }
            }

            //MainWindowViewModel._dialogIsOpen = false;
            MainWindowViewModel.CloseDialog();
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
        public void RemoveFile()
        {
            if (ImageDropCollection.Count > 0)
            {
                ImageDropCollection.RemoveAt(0);
            }
            if (ImageDropCollection.Count == 0)
            {
                FileDropText = "Drop lnk file here";
                IsPopupRemoveButton = false;
                BorderBackground = Avalonia.Media.Brushes.Transparent;
            }
        }

        [RelayCommand]
        public void CloseDialog()
        {
            MainWindowViewModel.CloseDialog();
        }
    }
}
