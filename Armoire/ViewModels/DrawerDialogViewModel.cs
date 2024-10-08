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

    [ObservableProperty]
    private DrawerViewModel drawerViewModel;

    [ObservableProperty]
    private ItemViewModel itemViewModel;

    public DrawerDialogViewModel()
    {
       
    
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
    public async Task AddDrawerClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        drawerContents.Add(
            new DrawerAsContentsViewModel()
        ) ;
    }

    [RelayCommand]
    public async Task addItemClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        drawerContents.Add(new ItemViewModel());

    }
}
