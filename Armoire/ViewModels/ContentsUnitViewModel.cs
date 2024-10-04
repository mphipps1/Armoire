using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase
{
    private static int _count;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _iconKind;

    [ObservableProperty]
    private string? _iconPath;

    public IContentsUnit? Model { get; set; }

    public ContentsUnitViewModel(IContentsUnit contentsUnit)
    {
        Name = contentsUnit.Name;
        IconKind = contentsUnit switch
        {
            DrawerAsContents => "PackageVariantClosed",
            Widget => "Octagram",
            Item => "Star",
            _ => "Rectangle"
        };
        //IconPath uses svgs as images
        IconPath = contentsUnit switch
        {
            DrawerAsContents => "/Assets/closedGradientDrawer.svg",
            Widget => "/Assets/databaseIcon.svg",
            Item => "/Assets/mspaintLogo.svg",
            _ => "/Assets/closedDrawer.svg"
        };
        Model = contentsUnit;
    }

    public ContentsUnitViewModel()
    {
        Name = "unit " + ++_count;
    }

    [RelayCommand]
    public virtual void HandleContentsClick() { }

    [RelayCommand]
    public virtual void HandleDeleteClick() { }
}
