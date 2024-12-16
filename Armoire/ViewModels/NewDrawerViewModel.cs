/*  This class holds the logic for the view displayed when a user wants to make a new drawer
 *  A new drawer must have a name, but can also accept an image to display
 * 
 */

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Avalonia.Media;
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
            Name = "";
            IconPath = "";
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

        // Update is called when "save" is clicked and checks to see if it should have a custom icon or not
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
                            TargetDrawerHeirarchy + 1,
                            ActiveContainerViewModel,
                            fileInfo.FullName
                        )
                    );
                    // Moving items in the dock so that they arent below the custom drawers/items
                    if (targetDrawer.SourceDrawer.DrawerHierarchy == -1)
                        targetDrawer.Contents.Move(
                            targetDrawer.Contents.Count - 1,
                            targetDrawer.Contents.Count - 4
                        );
                }
                else
                {
                    var newDrawer = new DrawerAsContentsViewModel(
                        Name,
                        "./../../../Assets/table.png",
                        TargetDrawerID,
                        TargetDrawerHeirarchy + 1,
                        ActiveContainerViewModel
                    );
                    targetDrawer.RegisterEventHandlers();
                    targetDrawer.Contents.Add(newDrawer);
                    // Moving items in the dock so that they arent below the custom drawers/items
                    if (targetDrawer.SourceDrawer.DrawerHierarchy == -1)
                        targetDrawer.Contents.Move(
                            targetDrawer.Contents.Count - 1,
                            targetDrawer.Contents.Count - 4
                        );
                }
            }

            //MainWindowViewModel._dialogIsOpen = false;
            MainWindowViewModel.CloseDialog();
        }

        // OpenFileDialogClick is used to allow users to select an image to upload
        [RelayCommand]
        public void OnOpenFileDialogClick()
        {
            //var window = TopLevel.GetTopLevel(this) as Window;

            var dialog = new OpenFileDialog();
            dialog.Filter = "*.png | *.jpg";
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
            for (int i = 0; i < fileNames.Length; i++)
            {
                ImageDropCollection.Add(filePaths[i]);
            }

            Name = Path.GetFileNameWithoutExtension(fileNames[0]);

            //if (result != null && result.Length != 0)
            //    NewItemViewModel.NewExe = result[0];
        }

        // GetTargetDrawer searches for the drawer to which we want to add the new drawer
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

        //Removes any uploaded files
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
