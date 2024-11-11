using Armoire.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Metadata;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Armoire.Views;

public partial class NewDrawerView : UserControl
{
    public NewDrawerView()
    {
        InitializeComponent();
        DropBorder.AddHandler(DragDrop.DropEvent, OnDrop);

    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        //https://github.com/AvaloniaUI/Avalonia.Xaml.Behaviors/discussions/118

        var items = e.Data.GetFileNames();
        string fileExtension = "";
        fileExtension = items.ElementAt(0).Substring(items.ElementAt(0).IndexOf('.') + 1);

        if (fileExtension.Equals("jpg") || fileExtension.Equals("png"))
        {
            FileInfo fileInfo = new FileInfo(items.ElementAt(0));

            if (sender is Border border)
            {
                if (DataContext is NewDrawerViewModel drawerviewmodel)
                {
                    drawerviewmodel.IsPopupRemoveButton = true;
                    var dotIndex = Path.GetFileName(fileInfo.Name).IndexOf('.');
                    border.Background = Brushes.DarkViolet;
                    drawerviewmodel.FileDropText = $"{Path.GetFileName(fileInfo.Name).Substring(0, dotIndex)} dropped";
                }
            }

            if (DataContext is NewDrawerViewModel viewModel)
            {
                if (viewModel.ImageDropCollection.Count < 1)
                {
                    viewModel.ImageDropCollection.Add(items.ElementAt(0));
                    viewModel.BorderBackground = Brushes.Violet;
                }
                else
                {

                    if (sender is Border dropareaborder)
                    {
                        dropareaborder.Background = Brushes.Transparent;
                        viewModel.FileDropText = "Remove the dropped file first";
                    }
                }
            }


        }
    }
}