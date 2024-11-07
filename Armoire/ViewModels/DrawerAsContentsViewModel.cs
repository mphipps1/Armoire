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
    private static int _count;

    // The "drawer as container" that contains this drawer button.
    public DrawerViewModel OuterContainer { get; set; }

    // The "drawer as container" that issues from this drawer button when clicked.
    public DrawerViewModel InnerContainer { get; set; }

    [ObservableProperty]
    private PlacementMode _flyoutPlacement = PlacementMode.Right;

    public DrawerAsContentsViewModel(DrawerViewModel outerContainer, string name)
    {
        Name = name;
        Id = ++_count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        InnerContainer = new DrawerViewModel();
        OuterContainer = outerContainer;
    }

    public DrawerAsContentsViewModel()
    {
        Name = "drawer " + ++_count;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        InnerContainer = new DrawerViewModel();
    }

    public DrawerAsContentsViewModel(int id, int drawerHierarchy)
    {
        DrawerHierarchy = drawerHierarchy;
        Name = "drawer " + ++_count;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
    }

    public DrawerAsContentsViewModel(string name, string? iconPath)
    {
        InnerContainer = new DrawerViewModel(_count++);
        Id = _count;
        Name = name;
        if (iconPath == null || iconPath == "")
            IconPath = "/../Assets/closedGradientDrawer.svg";
        else
            IconPath = iconPath;
    }

    [RelayCommand]
    public void CheckDraweModel(object parameter)
    {
        if (parameter is DrawerAsContentsViewModel viewModel)
        {
            var drawerviewmodel = viewModel.InnerContainer;

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
        if (InnerContainer.InnerContents.Count < 10)
        {
            //var newDrawer = new DrawerAsContentsViewModel();
            //newDrawer.DrawerHierarchy = DrawerHierarchy + 1;
            //if (newDrawer.DrawerHierarchy <= MAXNESTERDRAWERS)
            //{
            //    InnerContainer.InnerContents.Add(newDrawer);
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
        var view = new ChangeDrawerNameViewModel(Name);
        DialogHost.Show(view);
        //var window = new ChangeDrawerNameView();
        //window.Show();
    }

    public Drawer CreateDrawer(AppDbContext context)
    {
        OutputHelper.DebugPrintJson(this, "DeVm_CreateDrawer_this");
        OutputHelper.DebugPrintJson(OuterContainer, "DeVm_CreateDrawer_OuterContainer");
        return new Drawer() { Name = Name, ParentId = "default" };
    }
}
