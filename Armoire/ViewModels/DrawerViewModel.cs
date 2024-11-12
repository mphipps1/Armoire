using System.Collections.ObjectModel;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DrawerViewModel : ContainerViewModel
{
    private int _onAddCount = 0;

    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;

    // The contents of this "drawer as container".
    public ObservableCollection<ContentsUnitViewModel> InnerContents { get; set; } = [];

    [ObservableProperty]
    private int _ParentdrawerhierarchyPosition;

    public DrawerViewModel(DrawerAsContentsViewModel dacVm)
        : base(dacVm) // This calls the ContainerViewModel constructor.
    { }
}
