using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Armoire.Models;
using Armoire.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using Microsoft.Data.Sqlite;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private int _drawerCount = 0;

        public ObservableCollection<ContentsUnitViewModel> DrawerContents { get; } = [];

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
            await Task.Delay(TimeSpan.FromSeconds(1));
            DrawerContents.Add(
                new ContentsUnitViewModel(new DrawerAsContents($"{_drawerCount++}"))
            );
        }

        public MainWindowViewModel()
        {
            DrawerContents.Add(new ContentsUnitViewModel(new DrawerAsContents("0")));
            DrawerContents.Add(new ContentsUnitViewModel(new Widget("database", null)));
        }

        [RelayCommand]
        public void OpenSqlDialog()
        {
            DialogHost.Show(new SqlDialogViewModel(new SqlDialog()));
        }
    }
}
