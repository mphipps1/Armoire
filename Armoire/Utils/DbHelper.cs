using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Armoire.Models;
using Armoire.ViewModels;

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
        }
        catch (Exception ex)
        {
            throw new Exception("IVM ID: " + iVm.Id + "\n" + ex.Message);
        }
    }

    public static DrawerAsContentsViewModel LoadDockOrCreate()
    {
        using var context = new AppDbContext();
        var dockModel = context.Drawers.Find("CONTENTS_1");
        if (dockModel != null)
        {
            var x = LoadRecurse(dockModel, null, context);
            return x;
        }
        else
        {
            var y = new DrawerAsContentsViewModel(null, -1);
            return y;
        }

        //return dockModel != null
        //    ? LoadRecurse(dockModel, null, context)
        //    : new DrawerAsContentsViewModel(null, -1);
    }

    public static DrawerAsContentsViewModel LoadRecurse(
        Drawer drawer,
        ContainerViewModel? container,
        AppDbContext context
    )
    {
        var ret = new DrawerAsContentsViewModel(drawer, container);
        List<ContentsUnitViewModel> tempContents = [];
        foreach (var innerDrawer in drawer.Drawers)
        {
            var innerDacVm = LoadRecurse(innerDrawer, ret.GeneratedDrawer, context);
            tempContents.Add(innerDacVm);
        }
        foreach (var item in drawer.Items)
        {
            var itemViewModel = new ItemViewModel(item, ret.GeneratedDrawer);
            tempContents.Add(itemViewModel);
        }

        var x = tempContents.OrderBy(cuVm => cuVm.LoadPosition).ToList();
        ret.GeneratedDrawer.Contents = new ObservableCollection<ContentsUnitViewModel>(x);
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

    public static void SaveRecursive(ContentsUnitViewModel target)
    {
        switch (target)
        {
            case DrawerAsContentsViewModel dacVm:
                SaveDrawer(dacVm);
                foreach (var inner in dacVm.GeneratedDrawer.Contents)
                    SaveRecursive(inner);
                break;
            case ItemViewModel iVm:
                SaveItem(iVm);
                break;
            default:
                Debug.WriteLine(
                    "Unrecognized view model type encountered in `DbHelper.SaveRecursive`."
                );
                break;
        }
    }

    public static void RenameRecord(ContentsUnitViewModel viewModel)
    {
        using var context = new AppDbContext();
        switch (viewModel)
        {
            case DrawerAsContentsViewModel:
                var drawerToEdit = context.Drawers.Find(viewModel.Id);
                if (drawerToEdit != null)
                    drawerToEdit.Name = viewModel.Name;
                break;
            case ItemViewModel:
                var itemToEdit = context.Items.Find(viewModel.Id);
                if (itemToEdit != null)
                    itemToEdit.Name = viewModel.Name;
                break;
            default:
                Debug.WriteLine("Fell thru switch in RenameRecord");
                break;
        }
        context.SaveChanges();
    }

    public static void MoveRecord(ContentsUnitViewModel viewModel)
    {
        using var context = new AppDbContext();
        switch (viewModel)
        {
            case DrawerAsContentsViewModel:
                var drawerToEdit = context.Drawers.Find(viewModel.Id);
                if (drawerToEdit != null)
                    drawerToEdit.Position = viewModel.Position;
                break;
            case ItemViewModel:
                var itemToEdit = context.Items.Find(viewModel.Id);
                if (itemToEdit != null)
                    itemToEdit.Position = viewModel.Position;
                break;
            default:
                Debug.WriteLine("Fell thru switch in MoveRecord");
                break;
        }
        context.SaveChanges();
    }
}
