using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Armoire.Views;

public partial class NewItemView : UserControl
{
    private Popup popup;
    private Button submit;
    private StackPanel stack;
    public NewItemView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("Popup");
        submit = this.Find<Button>("Submit");
        stack = this.Find<StackPanel>("Stack");


    }

    private void OnPopupButton_Click(object sender, RoutedEventArgs e)
    {
        if (popup.IsOpen)
        {
            popup.IsOpen = false;
            submit.Margin = Thickness.Parse("10");
            stack.Height = 200;
        }
        else
        {
            popup.IsOpen = true;
            submit.Margin = Thickness.Parse("10 90 10 10");
            stack.Height = 270;

        }
    }
}