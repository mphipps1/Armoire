﻿using System.Collections.ObjectModel;
using Armoire.Interfaces;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase
{
    protected static int IdCount = 1;
    protected const string IdBase = "CONTENTS_";

    public ContainerViewModel? Container { get; set; }
    public ContentsUnitViewModel? Parent { get; set; }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string? _iconPath;

    [ObservableProperty]
    private bool _deleteMe;

    [ObservableProperty]
    private int _drawerHierarchy;

    // TODO: This is a "memory leak" because the models are part of the database.
    // They should only be used in the context of a "session with the database".
    public Item? Model { get; set; }

    public string ParentID { get; set; }

    public ContentsUnitViewModel()
    {
        Name = "unit " + ++IdCount;
    }

    [RelayCommand]
    public virtual void HandleContentsClick() { }

    [RelayCommand]
    public virtual void HandleDeleteClick()
    {
        DeleteMe = true;
        MainWindowViewModel.DeletedUnits.Push(this);
    }

    public string Id { get; set; } = "CONTENTS_NULL";
    public string ContainerId { get; set; } = "CONTAINER_NULL";
    public string ParentId { get; set; } = "CONTENTS_NULL";

    [RelayCommand]
    public void MoveUp()
    {
        var drawer = findParentDrawer(MainWindowViewModel.DockViewModel.InnerContents, this);
        if (drawer == null)
            return;
        int indexOfMe = drawer.IndexOf(this);

        //using c# tuple to swap the drawerAsContents this function was called from and the DrawerAsConetnts before it
        if (indexOfMe > 0)
            (drawer[indexOfMe - 1], drawer[indexOfMe]) = (drawer[indexOfMe], drawer[indexOfMe - 1]);
    }

    [RelayCommand]
    public void MoveDown()
    {
        var drawer = findParentDrawer(MainWindowViewModel.DockViewModel.InnerContents, this);
        if (drawer == null)
            return;
        //using c# tuple to swap the drawerAsContents this function was called from and the DrawerAsConetnts before it

        int indexOfMe = drawer.IndexOf(this);
        if (indexOfMe < drawer.Count - 1)
            (drawer[indexOfMe + 1], drawer[indexOfMe]) = (drawer[indexOfMe], drawer[indexOfMe + 1]);
    }

    public static ObservableCollection<ContentsUnitViewModel>? findParentDrawer(
        ObservableCollection<ContentsUnitViewModel> contentsIn,
        ContentsUnitViewModel target
    )
    {
        if (contentsIn.Contains(target))
            return contentsIn;
        foreach (var unit in contentsIn)
        {
            if (
                unit is DrawerAsContentsViewModel dac
                && dac.GeneratedDrawer.InnerContents.Contains(target)
            )
            {
                return dac.GeneratedDrawer.InnerContents;
            }
        }
        foreach (var unit in contentsIn)
        {
            if (unit is DrawerAsContentsViewModel dac)
            {
                return findParentDrawer(dac.GeneratedDrawer.InnerContents, target);
            }
        }
        return null;
    }

    public static void Undo()
    {
        if (MainWindowViewModel.DeletedUnits.Count == 0)
            return;
        ContentsUnitViewModel target = MainWindowViewModel.DeletedUnits.Pop();
        if (target.ParentID == "CONTENT_0")
        {
            MainWindowViewModel.DockViewModel.InnerContents.Add(target);
            return;
        }
        var ret = FindParentDrawerByID(MainWindowViewModel.DockViewModel.InnerContents, target);
        if (ret != null)
            ret.Add(target);
    }

    public static ObservableCollection<ContentsUnitViewModel>? FindParentDrawerByID(
        ObservableCollection<ContentsUnitViewModel> contentsIn,
        ContentsUnitViewModel target
    )
    {
        foreach (var unit in contentsIn)
        {
            if (unit is DrawerAsContentsViewModel dac && dac.Id == target.ParentID.ToString())
            {
                return dac.GeneratedDrawer.InnerContents;
            }
        }
        foreach (var unit in contentsIn)
        {
            if (unit is DrawerAsContentsViewModel dac)
            {
                return FindParentDrawerByID(dac.GeneratedDrawer.InnerContents, target);
            }
        }
        return null;
    }
}
