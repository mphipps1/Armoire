using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Armoire.Views;

public partial class NewItemView : UserControl
{
    private Popup popup;
    private StackPanel backAndSubmit;
    private StackPanel stack;
    public NewItemView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("Popup");
        backAndSubmit = this.Find<StackPanel>("BackAndSubmit");
        stack = this.Find<StackPanel>("Stack");


    }

    private void OnPopupButton_Click(object sender, RoutedEventArgs e)
    {
        if (popup.IsOpen)
        {
            popup.IsOpen = false;
            backAndSubmit.Margin = Thickness.Parse("-8 10 10 10");
            stack.Height = 200;
        }
        else
        {
            popup.IsOpen = true;
            backAndSubmit.Margin = Thickness.Parse("-8 90 10 10");
            stack.Height = 270;

        }
    }
}