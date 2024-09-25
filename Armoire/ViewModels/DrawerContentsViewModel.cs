using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DrawerContentsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _content;

    public DrawerContentsViewModel(DrawerButton drawerButton)
    {
        Content = drawerButton.Content;
    }

    public DrawerContentsViewModel() { }
}
