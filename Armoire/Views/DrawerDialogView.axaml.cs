using Armoire.Interfaces;
using Armoire.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.Views;

public partial class DrawerDialogView : UserControl, IDrawerDialogService
{
    
   

    public DrawerDialogView()
    {
        InitializeComponent();
       DataContext = new DrawerDialogViewModel();
        
        
    }
    
   
 
    public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
    {

        if (e.Source is Border border)
        {
           
            var pointerPoint = e.GetCurrentPoint(border);
            var properties = pointerPoint.Properties;

            
            if (properties.IsRightButtonPressed)
            {
                
                if (border.ContextMenu != null)
                {
                    // Open the context menu
                    border.ContextMenu.Open(border);
                }
            }
        }
    }

    public void ToggleDrawer()
    {
         bool isDrawerOpen = false;
    var drawer = this.FindControl<Border>("Drawer");
        drawer.IsVisible = true;
        drawer.Height = 100;

        if (isDrawerOpen)
        {
            drawer.IsVisible = false;
            drawer.Height = 0;
            drawer.Width = 0;
        }
        else
        {
            drawer.IsVisible = true;
            drawer.Height = 300;
            drawer.Width = 500;
        }


        isDrawerOpen = !isDrawerOpen;
    }

    
}
