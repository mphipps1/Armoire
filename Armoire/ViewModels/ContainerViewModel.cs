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
    private int _onAddCount;

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

        // Register event handlers.
        Contents.CollectionChanged += contents_CollectionChanged;
        Contents.CollectionChanged += contents_OnAdd;
    }

    // The drawer button that this container generates from.
    public DrawerAsContentsViewModel SourceDrawer { get; set; }

    public string SourceDrawerId { get; set; } = "default";

    public ObservableCollection<ContentsUnitViewModel> Contents { get; } = [];

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

    private void contents_OnAdd(object? sender, NotifyCollectionChangedEventArgs e)
    {
        using var context = new AppDbContext();
        if (e.NewItems is not { } newItems)
            return;
        var i = 0;
        foreach (var item in newItems)
        {
            switch (item)
            {
                case DrawerAsContentsViewModel deVm:
                    OutputHelper.DebugPrintJson(deVm, $"dc_OnAdd-deVm{i++}");
                    var newContentsUnit = deVm.CreateDrawer();
                    context.TryAddDrawer(newContentsUnit);
                    break;
                case ItemViewModel iVm:
                    Debug.WriteLine("dc_OnAdd ItemViewModel case placeholder");
                    break;
                default:
                    Debug.WriteLine("dc_OnAdd encountered unknown contents type");
                    break;
            }
        }
        OutputHelper.DebugPrintJson(context.Drawers, $"dc_OnAdd-drawersBeforeSave{_onAddCount}");
        context.SaveChanges();
        OutputHelper.DebugPrintJson(context.Drawers, $"dc_OnAdd-drawersAfterSave{_onAddCount}");
        _onAddCount++;
    }

    private void contents_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "DeleteMe":
                if (sender is ContentsUnitViewModel cu)
                    Contents.Remove(cu);
                break;
            default:
                Debug.WriteLine("Different property changed.");
                break;
        }
    }
}
