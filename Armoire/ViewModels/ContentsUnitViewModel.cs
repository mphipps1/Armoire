using System;
using System.Collections.ObjectModel;
using Armoire.Models;
using Armoire.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase
{
    protected const string IdBase = "CONTENTS_";

    public ContainerViewModel? Container { get; set; }
    public ContentsUnitViewModel? Parent { get; set; }
    public int? LoadPosition { get; set; }
    private string? IconPath { get; set; }

    // Expression-bodied property.
    public int? Position => Container?.Contents.IndexOf(this);

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _deleteMe;

    [ObservableProperty]
    protected int? _drawerHierarchy;

    [ObservableProperty]
    private string _moveBackDirection;

    [ObservableProperty]
    private string _moveForwardDirection;

    public Item? Model { get; set; }

    public string? ParentId { get; set; }

    public ContentsUnitViewModel()
    {
        var idSuffix = Guid.NewGuid().ToString();
        Id = IdBase + idSuffix;
        Name = "unit_" + idSuffix;
        MoveBackDirection = "Up";
        MoveForwardDirection = "Down";
    }

    //public ContentsUnitViewModel(Drawer _) { }

    //public ContentsUnitViewModel(Item _) { }

    [RelayCommand]
    public virtual void HandleContentsClick() { }

    [RelayCommand]
    public virtual void HandleDeleteClick()
    {
        Container?.RegisterEventHandlers();
        DeleteMe = true;
        MainWindowViewModel.DeletedUnits.Push(this);
    }

    public string Id { get; set; }
    public string ContainerId { get; set; } = "CONTAINER_NULL";

    [RelayCommand]
    public void MoveUp()
    {
        //var drawer = findParentDrawer(MainWindowViewModel.ActiveDockViewModel, this);
        if (Container == null)
            return;
        Container.RegisterEventHandlers();
        var indexOfMe = Container.Contents.IndexOf(this);

        if (indexOfMe <= 0)
            return;
        Container.RegisterEventHandlers();
        //(drawer.Contents[indexOfMe - 1], drawer.Contents[indexOfMe]) = (
        //    drawer.Contents[indexOfMe],
        //    drawer.Contents[indexOfMe - 1]
        //);
        Container.Contents.Move(indexOfMe, indexOfMe - 1);
    }

    [RelayCommand]
    public void MoveDown()
    {
        //var drawer = findParentDrawer(MainWindowViewModel.ActiveDockViewModel, this);
        if (Container == null)
            return;

        var indexOfMe = Container.Contents.IndexOf(this);
        if (indexOfMe >= Container.Contents.Count - 1)
            return;
        Container.RegisterEventHandlers();
        //(drawer.Contents[indexOfMe + 1], drawer.Contents[indexOfMe]) = (
        //    drawer.Contents[indexOfMe],
        //    drawer.Contents[indexOfMe + 1]
        //);
        Container.Contents.Move(indexOfMe, indexOfMe + 1);
    }

    public static ContainerViewModel? findParentDrawer(
        ContainerViewModel contentsIn,
        ContentsUnitViewModel target
    )
    {
        if (contentsIn.Contents.Contains(target))
            return contentsIn;
        foreach (var unit in contentsIn.Contents)
        {
            if (
                unit is DrawerAsContentsViewModel dac
                && dac.GeneratedDrawer.Contents.Contains(target)
            )
            {
                return dac.GeneratedDrawer;
            }
        }
        foreach (var unit in contentsIn.Contents)
        {
            if (unit is DrawerAsContentsViewModel dac)
            {
                return findParentDrawer(dac.GeneratedDrawer, target);
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
            DbHelper.SaveRecursive(target);
            return;
        }
        var ret = FindParentDrawerByID(MainWindowViewModel.ActiveDockViewModel.Contents, target);
        if (ret != null)
            ret.Add(target);
        DbHelper.SaveRecursive(target);
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
