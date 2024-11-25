using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Armoire.Utils;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class ContainerViewModel : ViewModelBase
{
    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;

    private const string IdBase = "CONTAINER_";
    private static long _id = 1;

    public string Id { get; set; }
    public string Name { get; set; } = "default";

    public ContainerViewModel() { }

    public ContainerViewModel(DrawerAsContentsViewModel sourceDrawer)
    {
        Id = IdBase + _id++;
        SourceDrawer = sourceDrawer;
        SourceDrawerId = sourceDrawer.Id;

        RegisterEventHandlers();
    }

    public void RegisterEventHandlers()
    {
        Contents.CollectionChanged -= contents_CollectionChanged;
        Contents.CollectionChanged -= contents_OnAdd;
        Contents.CollectionChanged += contents_CollectionChanged;
        Contents.CollectionChanged += contents_OnAdd;
    }

    // The drawer button that this container generates from.
    public DrawerAsContentsViewModel SourceDrawer { get; set; }

    public string? SourceDrawerId { get; set; }

    public ObservableCollection<ContentsUnitViewModel> Contents { get; set; } = [];

    private void contents_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // https://stackoverflow.com/a/8490996/16458003
        if (e.NewItems != null)
            foreach (ContentsUnitViewModel item in e.NewItems)
                item.PropertyChanged += contents_PropertyChanged;

        if (e.OldItems == null)
            return;

        foreach (ContentsUnitViewModel item in e.OldItems)
            item.PropertyChanged -= contents_PropertyChanged;
    }

    private static void contents_OnAdd(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not { } newContentsUnits)
            return;
        foreach (var contentsUnit in newContentsUnits)
        {
            switch (contentsUnit)
            {
                case DrawerAsContentsViewModel dacVm:
                    OutputHelper.DebugPrintJson(dacVm, $"CVM-contentsOnAdd-dacVm-{dacVm.Id}");
                    DbHelper.SaveDrawer(dacVm);
                    break;
                case RunningItemViewModel:
                    break;
                case ItemViewModel iVm:
                    OutputHelper.DebugPrintJson(iVm, $"CVM-contentsOnAdd-iVm-{iVm.Id}");
                    DbHelper.SaveItem(iVm);
                    break;
                default:
                    Debug.WriteLine(
                        $"Method `contents_OnAdd` encountered unknown contents type: `{contentsUnit.GetType()}`."
                    );
                    break;
            }
        }
    }

    private void contents_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "DeleteMe":
                if (sender is ContentsUnitViewModel cu)
                {
                    Contents.Remove(cu);
                    DbHelper.DeleteContentsUnitViewModelFromDb(cu);
                }
                break;
            default:
                Debug.WriteLine("Different property changed.");
                break;
        }
    }
}
