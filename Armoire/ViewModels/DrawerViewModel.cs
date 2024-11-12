using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DrawerViewModel : ContainerViewModel
{
    private int _onAddCount = 0;

    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;

    [ObservableProperty]
    private int _ParentdrawerhierarchyPosition;

    public DrawerViewModel(DrawerAsContentsViewModel dacVm)
        : base(dacVm) // This calls the ContainerViewModel constructor.
    { }
}
