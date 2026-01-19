/*  DrawerAsContents holds the logic for the button functionalities of a drawer
 *  These functions include moving, deleting, renaming etc.
 *
 *
 */

using System;
using System.Drawing.Imaging;
using System.IO;
using Armoire.Models;
using Armoire.Utils;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace Armoire.ViewModels;

public partial class DrawerAsContentsViewModel : ContentsUnitViewModel
{
    private const int MAXNESTERDRAWERS = 4;
    private static int _count = 1;

    // The "drawer as container" that generates from this drawer button when clicked.
    public DrawerViewModel GeneratedDrawer { get; set; }

    [ObservableProperty]
    private PlacementMode _flyoutPlacement = PlacementMode.Right;

    [ObservableProperty]
    private Avalonia.Media.Imaging.Bitmap _iconBmp;

    // Dock source constructor (leaves some properties null on purpose).
    public DrawerAsContentsViewModel(string? parentID, int? drawerHierarchy)
    {
        Name = "dock";
        Id = "CONTENTS_1";
        IconBmp = MiscHelper.GetAvaBmpFromAssets("tempDrawer.jpg");

        // Converting the .jpg image into a bitmap
        FileStream fs = new FileStream(
            "./../../../Assets/tempDrawer.jpg",
            FileMode.Open,
            FileAccess.Read
        );
        System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image, 60, 60);
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }
        GeneratedDrawer = new DrawerViewModel(this);
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        SetMoveDirections(this);
    }

    public DrawerAsContentsViewModel(string? parentID, int? drawerHierarchy, bool notDock)
    {
        Name = "drawer " + _count++;
        IconBmp = MiscHelper.GetAvaBmpFromAssets("table.png");
        GeneratedDrawer = new DrawerViewModel(this);
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        SetMoveDirections(this);
    }

    public DrawerAsContentsViewModel(
        string name,
        System.Drawing.Bitmap bmp,
        string parentID,
        int? drawerHierarchy,
        ContainerViewModel? cvm,
        string iconPath
    )
    {
        IconPath = iconPath;
        GeneratedDrawer = new DrawerViewModel(this);
        Name = name;

        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        SetMoveDirections(this);
        //the following converts the System.Drawing.Bitmap which cannot
        //be displayed in a view to an Avalonia.Media.Imaging.Bitmap type
        //which can be displayed by loading it into a memory stream to mimic downloading it
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }

        Container = cvm;
    }

    public DrawerAsContentsViewModel(
        string name,
        string? iconPath,
        string? parentID,
        int? drawerHierarchy,
        ContainerViewModel? cvm = null
    )
    {
        GeneratedDrawer = new DrawerViewModel(this);
        Name = name;
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        IconBmp = string.IsNullOrEmpty(iconPath) ? MiscHelper.GetAvaBmpFromAssets("table.png") : new Bitmap(iconPath);

        // Converting the .jpg image into a bitmap
        if (iconPath == null || iconPath == "")
        {
            FileStream fs = new FileStream(
                "./../../../Assets/table.png",
                FileMode.Open,
                FileAccess.Read
            );
            System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image, 50, 50);
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
            }
        }
        else
        {
            if (iconPath is not null)
            {
                FileStream fs = new FileStream(iconPath, FileMode.Open, FileAccess.Read);
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image, 50, 50);
                using (MemoryStream memory = new MemoryStream())
                {
                    bmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
                }
            }
        }
        _count++;
        SetMoveDirections(this);
        Container = cvm;
    }

    // Load-from-db constructor.
    public DrawerAsContentsViewModel(Drawer model, ContainerViewModel? container)
    {
        Id = model.Id;
        Name = model.Name;
        LoadPosition = model.Position;
        var defaultIcon = MiscHelper.GetAvaBmpFromAssets("table.png");
        IconBmp =
            model.IconPath == null
                ? defaultIcon
                : MiscHelper.GetAvaBmpFromImgPath(model.IconPath) ?? defaultIcon;
        GeneratedDrawer = new DrawerViewModel(this);
        Container = container;
        ParentId = model.ParentId;
        DrawerHierarchy = model.DrawerHierarchy;
        SetMoveDirections(this);
        _count++;
    }

    public DrawerAsContentsViewModel(DrawerAsContentsViewModel orig)
    {
        Id = orig.Id;
        Name = orig.Name;
        GeneratedDrawer = new DrawerViewModel(this);
        foreach (var cuVm in orig.GeneratedDrawer.Contents)
        {
            switch (cuVm)
            {
                case DrawerAsContentsViewModel dacVm:
                    GeneratedDrawer.Contents.Add(new DrawerAsContentsViewModel(dacVm));
                    break;
                case ItemViewModel iVm:
                    GeneratedDrawer.Contents.Add(new ItemViewModel(iVm));
                    break;
                default:
                    throw new NotImplementedException(
                        $"Copying a ${cuVm.GetType()} not supported yet."
                    );
            }
        }
        Container = orig.Container;
        ParentId = orig.ParentId;
        DrawerHierarchy = orig.DrawerHierarchy;
        SetMoveDirections(this);
    }

    // CheckDrawerModel is used to check which direction a drawer should open
    [RelayCommand]
    public void CheckDrawerModel(object typeviewModel)
    {
        if (typeviewModel is DrawerAsContentsViewModel viewModel)
        {
            var drawerviewmodel = viewModel.GeneratedDrawer;

            if (viewModel.DrawerHierarchy == 0)
            {
                FlyoutPlacement = PlacementMode.Right;
                drawerviewmodel.WrapPanelOrientation = Avalonia.Layout.Orientation.Horizontal;
            }
            else if (viewModel.DrawerHierarchy % 2 == 1)
            {
                FlyoutPlacement = PlacementMode.Bottom;
                drawerviewmodel.WrapPanelOrientation = Avalonia.Layout.Orientation.Vertical;
            }
            else
            {
                FlyoutPlacement = PlacementMode.Right;
                drawerviewmodel.WrapPanelOrientation = Avalonia.Layout.Orientation.Horizontal;
            }

            OnPropertyChanged(nameof(FlyoutPlacement));
        }
    }

    [RelayCommand]
    public void AddItemClick()
    {
        CloseDrawers();
        DialogHost.Show(new NewItemViewModel(Id, DrawerHierarchy, GeneratedDrawer));
    }

    [RelayCommand]
    public void AddDrawerClick()
    {
        CloseDrawers();
        if (GeneratedDrawer.Contents.Count < 10)
        {
            //var newDrawer = new DrawerAsContentsViewModel();
            //newDrawer.DrawerHierarchy = DrawerHierarchy + 1;
            //if (newDrawer.DrawerHierarchy <= MAXNESTERDRAWERS)
            //{
            //    GeneratedDrawer.InnerContents.Add(newDrawer);
            //}
            //else
            //{
            //    DialogHost.Show(
            //        new ErrorMessageViewModel(
            //            $"Maximum drawer nesting level reached. You can only open up to 4 nested drawers."
            //        )
            //    );
            //}
            DialogHost.Show(
                new NewDrawerViewModel(
                    Id,
                    DrawerHierarchy,
                    GeneratedDrawer,
                    App.ActiveMainWindowViewModel?.ViewHeight ?? 400
                )
            );
        }
        else
            DialogHost.Show(
                new ErrorMessageViewModel(
                    $"Drawer '{Name}' is full.\nDrawers can only hold 10 items."
                )
            );
    }

    public void CloseDrawers()
    {
        foreach (var drawer in MainWindowViewModel.DrawerAsContentsViews)
        {
            drawer.CloseFlyout();
        }
    }

    [RelayCommand]
    public void ChangeDrawerName()
    {
        var view = new EditDrawerViewModel(Name);
        DialogHost.Show(view);
        //var window = new ChangeDrawerNameView();
        //window.Show();
    }

    public Drawer CreateDrawer()
    {
        return new Drawer(Id, Name, ParentId, Position, DrawerHierarchy, IconPath);
    }
}
