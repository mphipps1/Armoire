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

public partial class NewItemView : UserControl
{
    private Popup popup;
    private StackPanel backAndSubmit;
    private StackPanel stack;
    private Popup dragAndDropPopup;
    private TextBox typedAppName;
    private string currentTypedAppName;
    private Controls originalList;
    public NewItemView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("Popup");
        backAndSubmit = this.Find<StackPanel>("BackAndSubmit");
        stack = this.Find<StackPanel>("Stack");
        dragAndDropPopup = this.FindControl<Popup>("DragDropArea");
        DropBorder.AddHandler(DragDrop.DropEvent, OnDrop);
        typedAppName = this.Find<TextBox>("TypedAppName");
        currentTypedAppName = "";
        //originalList = new Controls();
    }

    //makes buttons out of the list of executables
    public void PopulateExecutableList(object source, RoutedEventArgs args)
    {
        StackPanel? exeList = this.Find<StackPanel>("ExecutableList");
        if (exeList == null || exeList.Children.Count > 0)
            return;
        foreach (var name in NewItemViewModel.ExecutableNames)
        {
            Button button = new Button();
            button.Content = name;
            //set NewExe and close the popup
            button.Click += (s, e) =>
            {
                NewItemViewModel.NewExe = (s as Button).Content as string;
                this.Find<Popup>("Popup").IsOpen = false;
            };
            button.Height = 25;
            button.Padding = Thickness.Parse("10 0 0 0");
            button.BorderThickness = Thickness.Parse("0");
            button.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            button.FontWeight = Avalonia.Media.FontWeight.Normal;
            button.FontSize = 14;
            button.Foreground = Avalonia.Media.Brush.Parse("Black");
            button.Background = Avalonia.Media.Brush.Parse("LightGray");
            button.CornerRadius = Avalonia.CornerRadius.Parse("0");
            RowDefinition rowDefinition = new RowDefinition();
            exeList.Children.Add(button);
        }
        originalList = new Controls(exeList.Children);
    }

    public void KeyUp(object sender, KeyEventArgs e)
    {
        if (typedAppName.Text == currentTypedAppName || typedAppName.Text == null)
            return;
        if (!popup.IsOpen) {
            PopulateExecutableList(null, null);
            popup.IsOpen = true;
        }
        StackPanel? exeList = this.Find<StackPanel>("ExecutableList");
        //if (exeList == null || exeList.Children.Count == 0)
        //    return;
        currentTypedAppName = typedAppName.Text;

        if(e.Key == Key.Back) {
            exeList.Children.Clear();
            foreach(var button in originalList)
            {
                exeList.Children.Add((Button)button);
            }
        }

        if (typedAppName.Text == "")
            return;

        foreach (var entry in exeList.Children.ToList<Control>())
        {
            Button button = entry as Button;
            if (!button.Content.ToString().ToLower().Contains(currentTypedAppName.ToLower()))
                exeList.Children.Remove(button);
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        //https://github.com/AvaloniaUI/Avalonia.Xaml.Behaviors/discussions/118

        var items = e.Data.GetFileNames();
        string fileExtension = "";
          fileExtension =  items.ElementAt(0).Substring(items.ElementAt(0).IndexOf('.')+1);

        if (fileExtension.Equals("jpg") || fileExtension.Equals("png"))
        {
            FileInfo fileInfo = new FileInfo(items.ElementAt(0));

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
        else if (fileExtension.Equals("lnk"))
        {
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
                    if (viewModel.lnkDropCollection.Count < 1)
                    {
                        viewModel.lnkDropCollection.Add(shortcut);
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


   



}

