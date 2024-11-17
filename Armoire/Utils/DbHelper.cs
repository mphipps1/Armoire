using System;
using Armoire.ViewModels;

namespace Armoire.Utils;

public class DbHelper
{
    public static void SaveDrawer(DrawerAsContentsViewModel dacVm)
    {
        using var context = new AppDbContext();
        var drawerToAdd = dacVm.CreateDrawer();
        OutputHelper.DebugPrintJson(
            drawerToAdd,
            $"DbHelper-SaveDrawer-drawerToAdd-{drawerToAdd.Id}"
        );
        context.TryAddDrawer(drawerToAdd);
        context.SaveChanges();
    }

    public static void SaveItem(ItemViewModel iVm)
    {
        using var context = new AppDbContext();
        var itemToAdd = iVm.CreateItem();
        OutputHelper.DebugPrintJson(itemToAdd, $"DbHelper-SaveItem-itemToAdd-{itemToAdd.Id}");
        context.TryAddItem(itemToAdd);
        context.SaveChanges();
    }

    public static DrawerAsContentsViewModel LoadDrawerOrThrow(
        string id,
        ContainerViewModel container
    )
    {
        using var context = new AppDbContext();
        var drawerToLoad = context.Drawers.Find(id);
        if (drawerToLoad is null)
            throw new InvalidOperationException($"Drawer with Id {id} not found.");
        OutputHelper.DebugPrintJson(
            drawerToLoad,
            $"DbHelper-LoadDrawerOrThrow-drawerToLoad-{drawerToLoad.Id}"
        );
        return new DrawerAsContentsViewModel(drawerToLoad, container);
    }

    public static bool LoadDockOrCreate(out DrawerAsContentsViewModel dock)
    {
        using var context = new AppDbContext();
        var dockModel = context.Drawers.Find("CONTENTS_1");
        if (dockModel != null)
        {
            dock = new DrawerAsContentsViewModel(dockModel, null);
            OutputHelper.DebugPrintJson(dock, $"DbHelper-LoadDockOrCreate-dock-{dock.Id}");
            return true;
        }
        dock = new DrawerAsContentsViewModel(null, -1);
        return false;
    }
}
