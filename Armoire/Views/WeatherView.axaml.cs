using Armoire.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Armoire.Views;

public partial class WeatherView : ItemView
{
    public WeatherView()
    {
        InitializeComponent();
    }

    public void ShowPopup(object? sender, PointerEventArgs args)
    {
        this.FindControl<Popup>("Popup").IsOpen = true;
    }

    public void ClosePopup(object? sender, PointerEventArgs args)
    {
        this.FindControl<Popup>("Popup").IsOpen = false;
    }
}