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
        //using parameter as ParentID in undo button, fix this ID later
        DataContext = new DrawerAsContentsViewModel("CONTENTS_-1");  

     
    }


    private void Flyout_Opening(object? sender, EventArgs e)
    {
      
            if (DataContext is DrawerAsContentsViewModel viewModel)
            {
                viewModel.CheckDraweModel(DataContext);
            }
    }
}
