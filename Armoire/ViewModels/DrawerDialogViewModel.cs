using Armoire.Interfaces;
using Armoire.Models;
using Armoire.Views;
using Avalonia.Input;
using Avalonia.Interactivity;
using ShimSkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Armoire.ViewModels;

public class DrawerDialogViewModel : ViewModelBase {

    public ObservableCollection<ContentsUnitViewModel> drawerContents { get; } = [];

    private string Name1 = "Drawer";

    private DrawerDialog drawerDialog = new DrawerDialog();

    public DrawerDialogViewModel()
    {
        drawerContents.Add(new ContentsUnitViewModel(new DrawerAsContents("drawer 0")));
        drawerContents.Add(new ContentsUnitViewModel(new DrawerAsContents("drawer 1")));
        drawerContents.Add(new ContentsUnitViewModel(new DrawerAsContents("drawer 1")));

    }

    public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        drawerDialog.Btn_PointerReleased(sender, e);
    }

    public void ToggleDrawer()
    {
        drawerDialog.ToggleDrawer();
    }


    private async Task addContentsToDrawer(object? sender, RoutedEventArgs arg)
    {


    }



    private async Task AddDrawerClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        drawerContents.Add(
            new ContentsUnitViewModel(new DrawerAsContents("Drawer"))
        );
    }

    private async Task addItemClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        drawerContents.Add(new ContentsUnitViewModel(new Item("Item","path","1")));

    }
}
