using Armoire.Interfaces;
using Armoire.Models;
using Armoire.Views;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShimSkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Armoire.ViewModels;

public partial class DrawerDialogViewModel : DrawerViewModel
{

    public ObservableCollection<ContentsUnitViewModel> drawerContents { get; } = [];


    [ObservableProperty]
    private DrawerDialog drawerDialog;


    private static int _count = 0;

    [ObservableProperty]
    private string _drawerName;
    public DrawerDialogViewModel()
    {
        drawerContents.Add(new DrawerAsContentsViewModel(1, DrawerName, "/Assets/closedGradientDrawer.svg"));
    
    }

    /**
    public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        
           drawerDialog.Btn_PointerReleased(sender, e);
    }

    public void ToggleDrawer()
    {
        
        drawerDialog.ToggleDrawer();
    }


    */


    

    [RelayCommand]
    public async Task AddDrawer()
    {
        _drawerName = "Drawer" + _count++;
        await Task.Delay(TimeSpan.FromSeconds(1));
        drawerContents.Add(
            new DrawerAsContentsViewModel()
        ) ;
    }
    /**
    [RelayCommand]
    public async Task addItemClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        drawerContents.Add(new ItemViewModel());

    }
    */
}
