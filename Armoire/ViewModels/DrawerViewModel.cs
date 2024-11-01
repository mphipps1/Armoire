using System;
using System.Collections.ObjectModel;
using Armoire.Interfaces;
using Armoire.Models;
using Armoire.Utils;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DrawerViewModel : ViewModelBase, IHasId
{
    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;

    // The contents of this "drawer as container".
    public ObservableCollection<ContentsUnitViewModel> InnerContents { get; set; } = [];

    // The drawer button that this "drawer as container" issues from.
    public DrawerAsContentsViewModel? OuterContents { get; set; }
    public int Id { get; set; }
    public long Id3 { get; set; }
    public string Name { get; set; }

    [ObservableProperty]
    private int _ParentdrawerhierarchyPosition;

    public DrawerAsContentsViewModel drawerAsContentsViewModel { get; set; }

    public DrawerViewModel() { }

    public DrawerViewModel(int id)
    {
        Id = id;
    }

    public DrawerViewModel(int id, DrawerAsContentsViewModel drawerascontentsviewmodel)
    {
        drawerAsContentsViewModel = drawerascontentsviewmodel;
        Id = id;
    }

    public Drawer FindDrawer(AppDbContext context)
    {
        return context.Drawers.Find(Id3)
            ?? throw new InvalidOperationException($"Drawer with ID {Id3} not found.");
    }

    public Drawer CreateDrawer()
    {
        return new Drawer()
        {
            Name = Name,
            ParentDrawerId = OuterContents?.OuterContainer.Id3 ?? 1
        };
    }

    public void SaveToDb()
    {
        using var context = new AppDbContext();
        var drawerToAdd = CreateDrawer();
        OutputHelper.DebugPrintJson(drawerToAdd, "DVM_SaveToDb_drawer");
        context.TryAddDrawer(drawerToAdd);
        context.SaveChanges();
        Id3 = drawerToAdd.DrawerId;
    }
}
