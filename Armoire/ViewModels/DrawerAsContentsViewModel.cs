using System.Collections.ObjectModel;
using Armoire.Models;
using Armoire.Views;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace Armoire.ViewModels;

public partial class DrawerAsContentsViewModel : ContentsUnitViewModel
{
    private const int MAXNESTERDRAWERS = 4;
    private static int _count;
    private static int _id2 = 2;
    public DrawerAsContentsViewModel? ParentDrawer { get; set; }
    public DrawerViewModel DrawerAsContainer { get; set; }
    public int ID2 { get; set; }

    [ObservableProperty]
    private PlacementMode _flyoutPlacement = PlacementMode.Right;

    public DrawerAsContentsViewModel(DrawerAsContentsViewModel parent, string name)
    {
        Name = name;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        DrawerAsContainer = new DrawerViewModel();
        ID2 = _id2++;
        ParentDrawer = parent;
    }

    public DrawerAsContentsViewModel()
    {
        Name = "drawer " + ++_count;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        DrawerAsContainer = new DrawerViewModel();
        ID2 = _id2++;
    }

    public DrawerAsContentsViewModel(bool isDock)
    {
        Name = "dock";
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        DrawerAsContainer = new DrawerViewModel();
        ID2 = 1;
    }

    public DrawerAsContentsViewModel(int id, int drawerHierarchy)
    {
        DrawerHierarchy = drawerHierarchy;
        Name = "drawer " + ++_count;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        ID2 = _id2++;
    }

    public DrawerAsContentsViewModel(int id, string name, string iconPath)
    {
        DrawerAsContainer = new DrawerViewModel(id);
        Name = name;
        IconPath = iconPath;
        ID2 = _id2++;
    }

    public DrawerAsContentsViewModel(string name, string? iconPath)
    {
        DrawerAsContainer = new DrawerViewModel(_count++);
        Name = name;
        if (iconPath == null || iconPath == "")
            IconPath = "/../Assets/closedGradientDrawer.svg";
        else
            IconPath = iconPath;
        ID2 = _id2++;
    }

    [RelayCommand]
    public void CheckDraweModel(object parameter)
    {
        if (parameter is DrawerAsContentsViewModel viewModel)
        {
            var drawerviewmodel = viewModel.DrawerAsContainer;

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
        if (DrawerAsContainer.Contents.Count < 10)
        {
            var newDrawer = new DrawerAsContentsViewModel();
            newDrawer.DrawerHierarchy = DrawerHierarchy + 1;
            if (newDrawer.DrawerHierarchy <= MAXNESTERDRAWERS)
            {
                DrawerAsContainer.Contents.Add(newDrawer);
            }
            else
            {
                DialogHost.Show(
                    new ErrorMessageViewModel(
                        $"Maximum drawer nesting level reached. You can only open up to 4 nested drawers."
                    )
                );
            }
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
        var view = new ChangeDrawerNameViewModel(Name);
        DialogHost.Show(view);
        //var window = new ChangeDrawerNameView();
        //window.Show();
    }

    public Drawer GetDrawer()
    {
        return new Drawer() { DrawerId = ID2, ParentDrawerId = ParentDrawer?.ID2 ?? 1 };
    }
}
