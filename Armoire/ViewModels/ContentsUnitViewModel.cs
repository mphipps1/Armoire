﻿using System.Collections.ObjectModel;
using Armoire.Interfaces;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase, IHasId
{
    private static int _count;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string? _iconKind;

    [ObservableProperty]
    private string? _iconPath;

    [ObservableProperty]
    private bool _deleteMe;

    [ObservableProperty]
    private int _drawerHierarchy;

    public Item? Model { get; set; }

    public ContentsUnitViewModel()
    {
        Name = "unit " + ++_count;
    }

    [RelayCommand]
    public virtual void HandleContentsClick() { }

    [RelayCommand]
    public virtual void HandleDeleteClick()
    {
        DeleteMe = true;
    }

    public int Id { get; set; }

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
            if (unit is DrawerAsContentsViewModel dac)
            {
                if (dac.InnerContainer.InnerContents.Contains(target))
                {
                    return dac.InnerContainer.InnerContents;
                }
            }
        }
        foreach (var unit in contentsIn)
        {
            if (unit is DrawerAsContentsViewModel dac)
            {
                return findParentDrawer(dac.InnerContainer.InnerContents, target);
            }
        }
        return null;
    }
}
