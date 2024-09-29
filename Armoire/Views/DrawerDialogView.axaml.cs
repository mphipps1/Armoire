using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Armoire.Views;

public partial class DrawerDialogView : UserControl
{
    private bool isDrawerOpen = false;
    public DrawerDialogView()
    {
        InitializeComponent();
    }


    private void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            var button = sender as Button;
            button?.ContextMenu?.Open(button);
        }
    }

    public void ToggleDrawer()
    {
        var drawer = this.FindControl<Border>("Drawer");
        drawer.IsVisible = true;
        drawer.Height = 100;

        if (isDrawerOpen)
        {
            drawer.IsVisible = false;
            drawer.Height = 0;
            drawer.Width = 0;
        }
        else
        {
            drawer.IsVisible = true;
            drawer.Height = 300;
            drawer.Width = 500;
        }


        isDrawerOpen = !isDrawerOpen;
    }
}
