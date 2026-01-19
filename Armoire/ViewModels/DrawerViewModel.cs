/* This class inherets from Container, its mostly empty for now but splitting these 
 * Container and Drawer might make future development easier.
 * 
 */

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
