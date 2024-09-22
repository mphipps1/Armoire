using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<DrawerViewModel> Drawers { get; } = [];

        [ObservableProperty]
        private string _headingMain = "Welcome to Armoire!";

        [ObservableProperty]
        private string _headingDock = "Dock";

        [ObservableProperty]
        private string _headingDockAlt = "Dock Alternate";

        private bool CanAddDrawer() => true;

        [RelayCommand(CanExecute = nameof(CanAddDrawer))]
        private async Task HandleAddClick()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }

        public MainWindowViewModel()
        {
            Drawers.Add(new DrawerViewModel() { Content = "hi" });
            Drawers.Add(new DrawerViewModel() { Content = "hi2" });
        }
    }
}
