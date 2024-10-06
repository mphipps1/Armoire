using System.ComponentModel;
using Armoire.Interfaces;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class ContentsUnitViewModel : ViewModelBase, IHasId
{
    private static int _count;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _iconKind;

    [ObservableProperty]
    private string? _iconPath;

    [ObservableProperty]
    private bool _deleteMe;

    public IContentsUnit? Model { get; set; }

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
}
