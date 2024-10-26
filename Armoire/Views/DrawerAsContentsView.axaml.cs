using Armoire.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace Armoire.Views;

public partial class DrawerAsContentsView : UserControl
{
    public DrawerAsContentsView()
    {
        InitializeComponent();
        DataContext = new DrawerAsContentsViewModel();  

     
    }


    private void Flyout_Opening(object? sender, EventArgs e)
    {
      
            if (DataContext is DrawerAsContentsViewModel viewModel)
            {
                viewModel.CheckDraweModel(DataContext);
            }
    }
}
