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
        //using parameter as ParentId in undo button, fix this ID later
       // DataContext = new DrawerAsContentsViewModel("CONTENTS_-1");  
       MainWindowViewModel.DrawerAsContentsViews.AddLast(this);
    }

    public void CloseFlyout()
    {
        var drawerButton = this.Find<Button>("DrawerButton");
        if(drawerButton != null ) 
            if(drawerButton.Flyout != null ) 
                drawerButton.Flyout.Hide();
    }

    public void Flyout_Opening(object? sender, EventArgs e)
    {

        if (DataContext is DrawerAsContentsViewModel viewModel)
        {
            viewModel.CheckDrawerModel(viewModel);
        }
    }
}
