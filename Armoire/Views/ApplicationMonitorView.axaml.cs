using Armoire.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Armoire.Views;

public partial class ApplicationMonitorView : UserControl
{
    public ApplicationMonitorView()
    {
        InitializeComponent();
        DataContext = new ApplicationMonitorViewModel();
    }
}