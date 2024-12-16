/*  This class handles the UI of the start menu
 *  It constructs the list of executables base off the same list of executables in the NewItem drop down menu
 * 
 */

using Armoire.ViewModels;
using Armoire.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Armoire.Views;

public partial class StartMenuItemView : ItemView
{

    private TextBox? typedAppName;
    private string? currentTypedAppName;
    public static Controls? originalList;
    public static Popup? popup;

    public StartMenuItemView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("Popup");
        typedAppName = this.Find<TextBox>("TypedAppName");
        currentTypedAppName = "";
        PopulateStartMenuList();
    }

    public void PopulateStartMenuList()
    {
        StackPanel? startMenuList = this.Find<StackPanel>("StartMenuList");
        if (startMenuList == null || startMenuList.Children.Count > 0)
            return;

        if (NewItemViewModel.Icons == null || NewItemViewModel.ExecutableNames == null || NewItemViewModel.Executables == null)
            return;

        var Icons = NewItemViewModel.Icons;
        foreach (var name in NewItemViewModel.ExecutableNames)
        {
            // Making icons to place next to executable names
            Bitmap bmp = Icons[name].ToBitmap();
            Avalonia.Media.Imaging.Bitmap IconBmp;
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                IconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
            }

            Button button = new Button();
            StackPanel buttonContent = new StackPanel();
            buttonContent.Orientation = Avalonia.Layout.Orientation.Horizontal;
            buttonContent.Spacing = 10;

            Avalonia.Controls.Image icon = new Avalonia.Controls.Image();
            icon.Source = IconBmp;
            icon.Width = 32;
            icon.Height = 32;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = name;

            buttonContent.Children.Add(icon);
            buttonContent.Children.Add(textBlock);

            button.Content = buttonContent;
            button.Background = (IBrush)new BrushConverter().ConvertFrom("#222222");
            
            //set NewExe and close the popup
            button.Click += (s, e) =>
            {
                //NewItemViewModel.NewExe = (s as Button).Content as string;
                Process p = new Process();
                p.StartInfo.FileName = NewItemViewModel.Executables[name];
                p.StartInfo.UseShellExecute = true;
                p.Start();
                var popupControl = this.Find<Popup>("Popup");
                if (popupControl == null)
                    return;
                popupControl.IsOpen = false;
            };
            button.Width = 220;
            button.Height = 50;
            button.Padding = Thickness.Parse("10");
            button.BorderThickness = Thickness.Parse("0");
            button.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            button.FontWeight = Avalonia.Media.FontWeight.Normal;
            button.FontSize = 14;
            button.CornerRadius = Avalonia.CornerRadius.Parse("0");
            RowDefinition rowDefinition = new RowDefinition();
            startMenuList.Children.Add(button);
        }
        originalList = new Controls(startMenuList.Children);
    }

    // Narrowing the list of executables based off what the user has typed
    public void StartMenuKeyUp(object sender, KeyEventArgs e)
    {
        if (typedAppName is null || popup is null)
            return;

        if (typedAppName.Text == currentTypedAppName || typedAppName.Text == null)
            return;
        if (!popup.IsOpen)
        {
            PopulateStartMenuList();
            popup.IsOpen = true;
        }
        StackPanel? exeList = this.Find<StackPanel>("StartMenuList");
        if(exeList is null || originalList is null)
            return ;
        //if (exeList == null || exeList.Children.Count == 0)
        //    return;
        currentTypedAppName = typedAppName.Text;

        if (e.Key == Key.Back)
        {
           exeList.Children.Clear();
            foreach (var button in originalList)
            {
                exeList.Children.Add((Button)button);
            }
        }

        if (typedAppName.Text == "")
            return;

        foreach (var entry in exeList.Children.ToList<Control>())
        {
            Button? button = entry as Button;

            if (button is null || button.Content is null)
                return;
            if(button.Content is not StackPanel ||  ((StackPanel)button.Content).Children is null || ((StackPanel)button.Content).Children.Count < 2 )
                return;
            if(((StackPanel)button.Content).Children[1] is null || ((StackPanel)button.Content).Children[1].ToString() is null)
                return;

            string? s = ((TextBlock)((StackPanel)button.Content).Children[1]).Text;
            if (s is null)
                return;

            if (!s.Contains(currentTypedAppName, System.StringComparison.CurrentCultureIgnoreCase))
                exeList.Children.Remove(button);
            //if (!button.Content.ToString().Contains(currentTypedAppName, System.StringComparison.CurrentCultureIgnoreCase))
            //    exeList.Children.Remove(button);

        }
    }

    //An attempt at setting the focus to the textbox of the start menu when opened, doesnt currently work
    public void SetFocus(object sender, RoutedEventArgs args)
    {
        var textBox = this.Find<TextBox>("TypedAppName");
        if (textBox != null) 
            textBox.Focus();
    }

    //resetting the state of the start menu
    public void PopupCloseEvent(object? sender, System.EventArgs args)
    {
        if(DataContext == null)
            return;
        var vm = (StartMenuItemViewModel)this.DataContext;
        var textBox = this.Find<TextBox>("TypedAppName");
        if (textBox != null)
            textBox.Clear();

        StackPanel? exeList = this.Find<StackPanel>("StartMenuList");
        if (exeList is null || originalList is null)
            return;
        exeList.Children.Clear();
        foreach (var button in originalList)
        {
            exeList.Children.Add((Button)button);
        }
        vm.StartMenuTextBoxOpen = false;
    }
}