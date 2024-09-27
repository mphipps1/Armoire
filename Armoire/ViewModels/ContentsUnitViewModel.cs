using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _iconKind;

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
    }

    public ContentsUnitViewModel() { }
}
