using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DrawerViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _content;

    public DrawerViewModel(Drawer drawer)
    {
        Content = drawer.Content;
    }

    public DrawerViewModel() { }
}
