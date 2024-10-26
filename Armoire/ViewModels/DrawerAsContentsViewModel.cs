using Armoire.Models;
using Armoire.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;


namespace Armoire.ViewModels;

public partial class DrawerAsContentsViewModel : ContentsUnitViewModel
{
    private static int _count;
    public DrawerViewModel DrawerAsContainer { get; set; }

    [ObservableProperty]
    private PlacementMode _flyoutPlacement = PlacementMode.Right;



    public DrawerAsContentsViewModel()
    {
        Name = "drawer " + ++_count;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
        DrawerAsContainer = new DrawerViewModel();
    }

    public DrawerAsContentsViewModel(int id,int drawerHierarchy)
    {

        DrawerHierarchy = drawerHierarchy;  
        Name = "drawer " + ++_count;
        Id = _count;
        IconPath = "/Assets/closedGradientDrawer.svg";
    }

    public DrawerAsContentsViewModel(int id, string name, string iconPath)
    {
        DrawerAsContainer = new DrawerViewModel(id);
        Name = name;
        IconPath = iconPath;
    }

    private DrawerAsContentsViewModel(DrawerAsContentsViewModel copyMe)
    {
        Id = _count++;
        Name = copyMe.Name;
        IconPath = copyMe.IconPath;
        IconKind = copyMe.IconKind;
        DeleteMe = copyMe.DeleteMe;
    }

    [RelayCommand]
    private void CheckDraweModel(object parameter)
    {

        if (parameter is DrawerAsContentsViewModel viewModel)
        {
            var drawerviewmodel = viewModel.DrawerAsContainer;


            if(viewModel.DrawerHierarchy == 0)
            {
            //    FlyoutPlacement = PlacementMode.Right;
                drawerviewmodel.WrapPanelOrientation = Avalonia.Layout.Orientation.Horizontal;

            }else if(viewModel.DrawerHierarchy % 2 == 1)
            {     //FlyoutPlacement= PlacementMode.Bottom;
                drawerviewmodel.WrapPanelOrientation = Avalonia.Layout.Orientation.Vertical;

            }else
            {
               // FlyoutPlacement = PlacementMode.Bottom;
                drawerviewmodel.WrapPanelOrientation = Avalonia.Layout.Orientation.Horizontal;
            }
           
        }
    }

    [RelayCommand]
    public void AddItemClick()
    {
        DialogHost.Show(new NewItemViewModel(Id));
    }

    [RelayCommand]
    public void AddDrawerClick()
    {
        if (DrawerAsContainer.Contents.Count < 10)
        {
            var newDrawer = new DrawerAsContentsViewModel();
            newDrawer.DrawerHierarchy = DrawerHierarchy + 1;
            DrawerAsContainer.Contents.Add(newDrawer);
        }
        else
            DialogHost.Show(new ErrorMessageViewModel($"Drawer '{Name}' is full.\nDrawers can only hold 10 items."));
    }

    [RelayCommand]
    public void ChangeDrawerName()
    {
        var view = new ChangeDrawerNameViewModel(Name);
        DialogHost.Show(view);
        //var window = new ChangeDrawerNameView();
        //window.Show();
    }

}
