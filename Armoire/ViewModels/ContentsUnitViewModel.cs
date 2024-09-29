using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _iconKind;

    [ObservableProperty]
    private string? _iconPath;

    public IContentsUnit Model { get; set; }

    public ContentsUnitViewModel(IContentsUnit contentsUnit)
    {
        Name = contentsUnit.Name;
        IconKind = contentsUnit switch
        {
            DrawerAsContents => "Tray",
            Widget => "Database",
            Item => "Pyramid",
            _ => "Rectangle"
        };
        IconPath = contentsUnit switch
        {
            DrawerAsContents => "/Assets/closedGradientDrawer.svg",
            Widget => "/Assets/databaseIcon.svg",
            Item => "/Assets/mspaintLogo.svg",
            _ => "/Assets/closedDrawer.svg"
        };
        Model = contentsUnit;
    }

    public ContentsUnitViewModel() { }

    public void OnItemClick()
    {

    }
}
