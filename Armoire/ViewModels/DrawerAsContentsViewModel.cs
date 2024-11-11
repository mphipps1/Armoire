using System.Data;
using System.Data.Common;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using Armoire.Models;
using Armoire.Utils;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace Armoire.ViewModels;

public partial class DrawerAsContentsViewModel : ContentsUnitViewModel
{
    private const int MAXNESTERDRAWERS = 4;
    private static int _count = 1;

    // The "drawer as container" that issues from this drawer button when clicked.
    public DrawerViewModel GeneratedDrawer { get; set; }

    [ObservableProperty]
    private PlacementMode _flyoutPlacement = PlacementMode.Right;

    [ObservableProperty]
    private Avalonia.Media.Imaging.Bitmap _iconBmp;

    public DrawerAsContentsViewModel(
        DrawerViewModel container,
        string name,
        string? parentID,
        int drawerHierarchy
    )
    {
        Name = name;
        FileStream fs = new FileStream("./../../../Assets/tempDrawer.jpg", FileMode.Open, FileAccess.Read);
        System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image, 60, 60);
        using (MemoryStream memory = new MemoryStream())
        {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
        }
        GeneratedDrawer = new DrawerViewModel(this);
        Container = container;
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        SetMoveDirections(this);
        _count++;
    }

    public DrawerAsContentsViewModel(string? parentID, int drawerHierarchy)
    {
        Name = "drawer " + _count++;
        FileStream fs = new FileStream("./../../../Assets/tempDrawer.jpg", FileMode.Open, FileAccess.Read);
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

    public DrawerAsContentsViewModel(
        string name,
        System.Drawing.Bitmap bmp,
        string parentID,
        int drawerHierarchy
        )
    {
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
    }

    public DrawerAsContentsViewModel(
        string name,
        string? iconPath,
        string? parentID,
        int drawerHierarchy
    )
    {
        GeneratedDrawer = new DrawerViewModel(this);
        Name = name;
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        if (iconPath == null || iconPath == "")
            IconPath = "/../Assets/closedGradientDrawer.svg";
        else
            IconPath = iconPath;
        _count++;
        SetMoveDirections(this);
    }

    [RelayCommand]
    public void CheckDraweModel(object typeviewModel)
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
        DialogHost.Show(new NewItemViewModel(Id, DrawerHierarchy));
    }

    [RelayCommand]
    public void AddDrawerClick()
    {
        if (GeneratedDrawer.InnerContents.Count < 10)
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
            DialogHost.Show(new NewDrawerViewModel(Id, DrawerHierarchy));
        }
        else
            DialogHost.Show(
                new ErrorMessageViewModel(
                    $"Drawer '{Name}' is full.\nDrawers can only hold 10 items."
                )
            );
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
        OutputHelper.DebugPrintJson(this, "DeVm_CreateDrawer_this");
        OutputHelper.DebugPrintJson(Container, "DeVm_CreateDrawer_OuterContainer");
        return new Drawer()
        {
            Id = Id,
            Name = Name,
            ParentId = ParentId
        };
    }
}
