using System;
using System.Diagnostics;
using System.Linq;
using Armoire.Models;
using Armoire.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Armoire.Utils;

public class DbHelper
{
    public static void SaveDrawer(DrawerAsContentsViewModel dacVm)
    {
        if (dacVm.Id is "NOTIFICATIONS" or "MONITOR")
            return;
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
        if (iVm.Id is "BATTERY" or "START_MENU" or "RUNNING" or "SOUND" or "WIFI" or "WEATHER")
            return;
        using var context = new AppDbContext();
        var itemToAdd = iVm.CreateItem();
        OutputHelper.DebugPrintJson(itemToAdd, $"DbHelper-SaveItem-itemToAdd-{itemToAdd.Id}");
        context.TryAddItem(itemToAdd);
        try
        {
            context.SaveChanges();
        } catch (Exception ex)
        {
            throw new Exception("IVM ID: " + iVm.Id + "\n" + ex.Message);            
        }
    }

    public static DrawerAsContentsViewModel LoadDockOrCreate()
    {
        using var context = new AppDbContext();
        var dockModel = context.Drawers.Find("CONTENTS_1");
        return dockModel != null
            ? LoadRecurse(dockModel, null, context)
            : new DrawerAsContentsViewModel(null, -1);
    }

    public static DrawerAsContentsViewModel LoadRecurse(
        Drawer drawer,
        ContainerViewModel? container,
        AppDbContext context
    )
    {
        var ret = new DrawerAsContentsViewModel(drawer, container);
        foreach (var innerDrawer in drawer.Drawers)
        {
            var innerDacVm = LoadRecurse(innerDrawer, ret.GeneratedDrawer, context);
            ret.GeneratedDrawer.Contents.Add(innerDacVm);
        }
        foreach (var item in drawer.Items)
        {
            var itemViewModel = new ItemViewModel(item, ret.GeneratedDrawer);
            ret.GeneratedDrawer.Contents.Add(itemViewModel);
        }
        return ret;
    }

    public static void DeleteContentsUnitViewModelFromDb(ContentsUnitViewModel viewModel)
    {
        using var context = new AppDbContext();
        var drawerToDelete = context.Drawers.Find(viewModel.Id);
        if (drawerToDelete != null)
            context.Drawers.Remove(drawerToDelete);
        else
        {
            var itemToDelete = context.Items.Find(viewModel.Id);
            if (itemToDelete != null)
                context.Items.Remove(itemToDelete);
        }
        context.SaveChanges();
    }
}
