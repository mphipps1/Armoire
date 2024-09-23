using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DrawerButtonViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _content;

    public DrawerButtonViewModel(DrawerButton drawerButton)
    {
        Content = drawerButton.Content;
    }

    public DrawerButtonViewModel() { }
}
