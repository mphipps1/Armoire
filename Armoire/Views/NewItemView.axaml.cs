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

    private void OnFileDrop()
    {
        backAndSubmit.Margin = Thickness.Parse("-8 70 10 10");
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
                    border.Background = Brushes.DarkViolet;
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


   



}

