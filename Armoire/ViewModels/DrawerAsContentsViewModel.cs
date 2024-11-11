using System.Data;
using System.Data.Common;
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

    public DrawerAsContentsViewModel(
        DrawerViewModel container,
        string name,
        string? parentID,
        int drawerHierarchy
    )
    {
        Name = name;
        IconPath = "/Assets/closedGradientDrawer.svg";
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
        IconPath = "/Assets/closedGradientDrawer.svg";
        GeneratedDrawer = new DrawerViewModel(this);
        ParentId = parentID;
        DrawerHierarchy = drawerHierarchy;
        SetMoveDirections(this);
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
        DialogHost.Show(new NewItemViewModel(Id, DrawerHierarchy, true));
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
            DialogHost.Show(new NewItemViewModel(Id, DrawerHierarchy, false));
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
        var view = new EditViewModel(Name);
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
