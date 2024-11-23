using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Armoire.Views;

public partial class ItemView : UserControl
{
    public ItemView()
    {
        InitializeComponent();
    }


    public void ButtonPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            if (sender is Button button)
            {

                if (button.ContextMenu != null)
                {
                    button.ContextMenu.Open(button);
                }
            }
        }
    }
}
