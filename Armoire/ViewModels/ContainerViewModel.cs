﻿/* Container holds a collection of ContentsUnits along with information about this collection
 * This class holds functions that are called when the collection of ContentsUnits is changed
 * 
 */


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
    // This is the direction that this collection will be displayed
    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;

    private const string IdBase = "CONTAINER_";
    private static long _id = 1;
    public string Id { get; set; }
    public string Name { get; set; } = "default";
    // The drawer button that this container generates from.
    public DrawerAsContentsViewModel SourceDrawer { get; set; }
    public string? SourceDrawerId { get; set; }
    public ObservableCollection<ContentsUnitViewModel> Contents { get; set; } = [];

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
        Contents.CollectionChanged -= contents_OnMove;
        Contents.CollectionChanged += contents_CollectionChanged;
        Contents.CollectionChanged += contents_OnAdd;
        Contents.CollectionChanged += contents_OnMove;
        foreach (ContentsUnitViewModel cuVm in Contents)
        {
            cuVm.PropertyChanged -= contents_PropertyChanged;
            cuVm.PropertyChanged += contents_PropertyChanged;
        }
    }

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
                    DbHelper.SaveDrawer(dacVm);
                    break;
                case RunningItemViewModel:
                    break;
                case ItemViewModel iVm:
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

    private static void contents_OnMove(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Move)
            return;
        if (sender is not ObservableCollection<ContentsUnitViewModel> s)
            return;
        var t = s[e.OldStartingIndex];
        var u = s[e.NewStartingIndex];
        DbHelper.MoveRecord(t);
        DbHelper.MoveRecord(u);
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
            case "Name":
                if (sender is ContentsUnitViewModel cu2)
                {
                    DbHelper.RenameRecord(cu2);
                }
                break;
            default:
                Debug.WriteLine("Different property changed.");
                break;
        }
    }
}
