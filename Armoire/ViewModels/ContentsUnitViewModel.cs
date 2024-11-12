using System.Collections.ObjectModel;
using Armoire.Interfaces;
using Armoire.Models;
using Avalonia.Controls;
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
    protected int _drawerHierarchy;

    [ObservableProperty]
    private string _moveBackDirection;

    [ObservableProperty]
    private string _moveForwardDirection;

    // TODO: This is a "memory leak" because the models are part of the database.
    // They should only be used in the context of a "session with the database".
    public Item? Model { get; set; }

    public string? ParentId { get; set; }

    public ContentsUnitViewModel()
    {
        Id = IdBase + IdCount++;
        Name = "unit " + IdCount;
    }

    [RelayCommand]
    public virtual void HandleContentsClick() { }

    [RelayCommand]
    public virtual void HandleDeleteClick()
    {
        DeleteMe = true;
        MainWindowViewModel.DeletedUnits.Push(this);
    }

    public string Id { get; set; }
    public string ContainerId { get; set; } = "CONTAINER_NULL";

    [RelayCommand]
    public void MoveUp()
    {
        var drawer = findParentDrawer(MainWindowViewModel.ActiveDockViewModel.Contents, this);
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
        var drawer = findParentDrawer(MainWindowViewModel.ActiveDockViewModel.Contents, this);
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
                && dac.GeneratedDrawer.Contents.Contains(target)
            )
            {
                return dac.GeneratedDrawer.Contents;
            }
        }
        foreach (var unit in contentsIn)
        {
            if (unit is DrawerAsContentsViewModel dac)
            {
                return findParentDrawer(dac.GeneratedDrawer.Contents, target);
            }
        }
        return null;
    }

    public static void Undo()
    {
        if (MainWindowViewModel.DeletedUnits.Count == 0)
            return;
        ContentsUnitViewModel target = MainWindowViewModel.DeletedUnits.Pop();
        target.DeleteMe = false;
        if (target.ParentId == "CONTENTS_1")
        {
            MainWindowViewModel.ActiveDockViewModel.Contents.Add(target);
            return;
        }
        var ret = FindParentDrawerByID(
            MainWindowViewModel.ActiveDockViewModel.Contents,
            target
        );
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
            if (unit is DrawerAsContentsViewModel dac && dac.Id == target.ParentId)
            {
                return dac.GeneratedDrawer.Contents;
            }
        }
        foreach (var unit in contentsIn)
        {
            if (unit is DrawerAsContentsViewModel dac)
            {
                return FindParentDrawerByID(dac.GeneratedDrawer.Contents, target);
            }
        }
        return null;
    }

    [RelayCommand]
    public void SetMoveDirections(object parameter)
    {
        if (parameter is not ContentsUnitViewModel)
            return;
        if (parameter is ContentsUnitViewModel unit)
        {
            if (unit.DrawerHierarchy == 0)
            {
                MoveForwardDirection = "Move Down";
                MoveBackDirection = "Move Up";
            }
            else if (unit.DrawerHierarchy % 2 == 1)
            {
                MoveForwardDirection = "Move Right";
                MoveBackDirection = "Move Left";
            }
            else
            {
                MoveForwardDirection = "Move Down";
                MoveBackDirection = "Move Up";
            }
        }
    }
}
