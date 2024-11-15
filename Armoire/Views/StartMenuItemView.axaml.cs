using Armoire.ViewModels;
using Armoire.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using System.Linq;

namespace Armoire.Views;

public partial class StartMenuItemView : ItemView
{

    private TextBox typedAppName;
    private string currentTypedAppName;
    public static Controls originalList;
    public static Popup popup;

    public StartMenuItemView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("Popup");
        typedAppName = this.Find<TextBox>("TypedAppName");
        currentTypedAppName = "";
    }

    public void PopulateStartMenuList(object source, RoutedEventArgs args)
    {
        StackPanel? startMenuList = this.Find<StackPanel>("StartMenuList");
        if (startMenuList == null || startMenuList.Children.Count > 0)
            return;
        foreach (var name in NewItemViewModel.ExecutableNames)
        {
            Button button = new Button();
            //StackPanel buttonContent = new StackPanel();
            //buttonContent.Orientation = Avalonia.Layout.Orientation.Horizontal;
            //buttonContent.Children.Add
            button.Content =name;
            
            //set NewExe and close the popup
            button.Click += (s, e) =>
            {
                //NewItemViewModel.NewExe = (s as Button).Content as string;
                Process p = new Process();
                p.StartInfo.FileName = NewItemViewModel.Executables[name];
                p.StartInfo.UseShellExecute = true;
                p.Start();
                this.Find<Popup>("Popup").IsOpen = false;
                var vm = (StartMenuItemViewModel)this.DataContext;
                vm.OpenStartMenu();
            };
            button.Width = 220;
            button.Height = 50;
            button.Padding = Thickness.Parse("10");
            button.BorderThickness = Thickness.Parse("0");
            button.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            button.FontWeight = Avalonia.Media.FontWeight.Normal;
            button.FontSize = 14;
            button.Foreground = Avalonia.Media.Brush.Parse("Black");
            button.Background = Avalonia.Media.Brush.Parse("LightGray");
            button.CornerRadius = Avalonia.CornerRadius.Parse("0");
            RowDefinition rowDefinition = new RowDefinition();
            startMenuList.Children.Add(button);
        }
        originalList = new Controls(startMenuList.Children);
    }

    public void KeyUp(object sender, KeyEventArgs e)
    {
        if (typedAppName.Text == currentTypedAppName || typedAppName.Text == null)
            return;
        if (!popup.IsOpen)
        {
            PopulateStartMenuList(null, null);
            popup.IsOpen = true;
        }
        StackPanel? exeList = this.Find<StackPanel>("StartMenuList");
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
            Button button = entry as Button;
            if (!button.Content.ToString().ToLower().Contains(currentTypedAppName.ToLower()))
                exeList.Children.Remove(button);
        }
    }
}