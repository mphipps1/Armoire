using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Armoire.Interfaces;
using Armoire.Models;
using Avalonia.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Styles.Controls;
using Microsoft.VisualBasic;

namespace Armoire.ViewModels;

public partial class DrawerViewModel : ViewModelBase, IHasId
{
    [ObservableProperty]
    private Orientation _wrapPanelOrientation = Orientation.Horizontal;
    public ObservableCollection<ContentsUnitViewModel> Contents { get; set; } = [];
    public int Id { get; set; }

    [ObservableProperty]
    private int drawerHierarchy;

  
    private static int counter;



    

    public DrawerViewModel()
    {
    }
    public DrawerViewModel(int id)
    {
        Id = id;
    }

    public DrawerViewModel(int id, int drawerhierarchy)
    {
        Id = id;
        drawerHierarchy = drawerhierarchy;
    }

    [RelayCommand]
    public async Task addDrawerClick()
    {
        if (Contents.Count == 0)
        {
            counter += 1;
        }


      
        DrawerViewModel ViewModel = new DrawerViewModel(1,counter);

        await Task.Delay(TimeSpan.FromSeconds(1));
        Contents.Add(
            new DrawerAsContentsViewModel() { DrawerAsContainer = ViewModel} 
        );

        var drawerAsContentsViewModel = (DrawerAsContentsViewModel)Contents[0];
        
        if(drawerAsContentsViewModel.DrawerAsContainer.drawerHierarchy % 2 == 1)
        {
            _wrapPanelOrientation = Orientation.Horizontal;
        } else if (drawerAsContentsViewModel.DrawerAsContainer.drawerHierarchy % 2 == 0)
        {
            _wrapPanelOrientation = Orientation.Vertical;
        }

     
            
        
        
    }

    [RelayCommand]
    public async Task addItemClick()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        Contents.Add(new ItemViewModel());

    }
}
