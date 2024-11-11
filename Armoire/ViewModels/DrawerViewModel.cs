using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Armoire.Interfaces;
using Armoire.Models;
using Armoire.Utils;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class DrawerViewModel : ContainerViewModel
{
    private int _onAddCount = 0;

    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;

    // The contents of this "drawer as container".
    public ObservableCollection<ContentsUnitViewModel> InnerContents { get; set; } = [];

    public int Id { get; set; }
    public string Name { get; set; }

    [ObservableProperty]
    private int _ParentdrawerhierarchyPosition;

    public DrawerViewModel(DrawerAsContentsViewModel dacVm)
        : base(dacVm) // This calls the ContainerViewModel constructor.
    {
        // Register event handlers.
        InnerContents.CollectionChanged += dc_CollectionChanged;
        InnerContents.CollectionChanged += dc_OnAdd;
    }

    public Drawer CreateDrawer()
    {
        return new Drawer() { Name = Name, ParentId = "default" };
    }

    public void SaveToDb()
    {
        using var context = new AppDbContext();
        var drawerToAdd = CreateDrawer();
        OutputHelper.DebugPrintJson(drawerToAdd, "DVM_SaveToDb_drawer");
        context.TryAddDrawer(drawerToAdd);
        context.SaveChanges();
    }

    private void dc_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // https://stackoverflow.com/a/8490996/16458003
        if (e.NewItems != null)
            foreach (ContentsUnitViewModel item in e.NewItems)
                item.PropertyChanged += dc_PropertyChanged;

        if (e.OldItems != null)
            foreach (ContentsUnitViewModel item in e.OldItems)
                item.PropertyChanged -= dc_PropertyChanged;
    }

    private void dc_OnAdd(object? sender, NotifyCollectionChangedEventArgs e)
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
        context.SaveChanges();
        _onAddCount++;
    }

    private void dc_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "DeleteMe":
                if (sender is ContentsUnitViewModel cu)
                    InnerContents.Remove(cu);
                break;
            default:
                Debug.WriteLine("Different property changed.");
                break;
        }
    }
}
