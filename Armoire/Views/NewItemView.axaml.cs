using Armoire.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Armoire.Views;

public partial class NewItemView : UserControl
{
    private Popup popup;
    private StackPanel backAndSubmit;
    private StackPanel stack;
    private Popup dragAndDropPopup;
    public NewItemView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("Popup");
        backAndSubmit = this.Find<StackPanel>("BackAndSubmit");
        stack = this.Find<StackPanel>("Stack");
        dragAndDropPopup = this.FindControl<Popup>("DragDropArea");
        DropBorder.AddHandler(DragDrop.DropEvent, OnDrop);


    }

    private void OnPopupButton_Click(object sender, RoutedEventArgs e)
    {
        if (popup.IsOpen)
        {
            popup.IsOpen = false;
            backAndSubmit.Margin = Thickness.Parse("-8 10 10 10");
            stack.Height = 250;
        }
        else
        {
            popup.IsOpen = true;
            backAndSubmit.Margin = Thickness.Parse("0 90 10 10");
            stack.Height = 300;

        }
    }

    private void OnPopup_Click(object sender, RoutedEventArgs e)
    {
        if (dragAndDropPopup.IsOpen)
        {
            dragAndDropPopup.IsOpen = false;
            backAndSubmit.Margin = Thickness.Parse("-8 10 10 10");
            stack.Height = 250;
            stack.Width = 150;
        }
        else
        {
            dragAndDropPopup.IsOpen = true;
            backAndSubmit.Margin = Thickness.Parse("50 400 10 10");
            stack.Height = 600;
            stack.Width = 300;
        }
    }


    private void OnDrop(object? sender, DragEventArgs e)
    {
        //https://github.com/AvaloniaUI/Avalonia.Xaml.Behaviors/discussions/118

        var items = e.Data.GetFileNames();
        if (items != null)
        {
            Type shellObjectType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic windowsShell = Activator.CreateInstance(shellObjectType);
            dynamic shortcut = windowsShell.CreateShortcut(items.ElementAt(0));
            
            var file = shortcut.TargetPath;

            FileInfo fileInfo = new FileInfo(file);
          
            if (sender is Border border)
            {
                if (DataContext is NewItemViewModel itemviewmodel)
                {
                    itemviewmodel.IsPopupRemoveButton = true;
                    var dotIndex = Path.GetFileName(fileInfo.Name).IndexOf('.');
                    border.Background = Brushes.Violet;
                    itemviewmodel.FileDropText = $"{Path.GetFileName(fileInfo.Name).Substring(0, dotIndex)} dropped";
                }
            }

            if (DataContext is NewItemViewModel viewModel)
            {
                if (viewModel.DropCollection.Count < 1)
                {
                    viewModel.DropCollection.Add(shortcut);
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


    private async void OnOpenFileDialogClick(object sender, RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;

        var dialog = new OpenFileDialog
        {
            AllowMultiple = true
        };

        var result = await dialog.ShowAsync(window);

    }



}

