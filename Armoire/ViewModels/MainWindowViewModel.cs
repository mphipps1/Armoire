using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Armoire.Models;
using Armoire.Utils;
using Armoire.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using Microsoft.Data.Sqlite;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private int _contentsUnitCount;
        private int _drawerCount;
        private int _itemCount;

        public ObservableCollection<ContentsUnitViewModel> DockContents { get; } = [];

        private bool CanAddContentsUnit() => true;

        [RelayCommand(CanExecute = nameof(CanAddContentsUnit))]
        private async Task HandleDrawerAddClick()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            //await DialogHost.Show(new NewEntryPopUpViewModel());
            _contentsUnitCount++;
            _drawerCount++;
            DockContents.Add(
                new ContentsUnitViewModel(new DrawerAsContents($"drawer {_drawerCount}"))
            );
        }

        public MainWindowViewModel()
        {
            DockContents.Add(new ContentsUnitViewModel(new Widget("database", null)));
            DockContents.Add(new ContentsUnitViewModel(new DrawerAsContents("drawer 0")));
            DockContents.Add(
                new ContentsUnitViewModel(
                    new Item("Paint", "C:\\WINDOWS\\system32\\mspaint.exe", "0")
                )
            );
        }

        [RelayCommand]
        public void OpenSqlDialog()
        {
            DialogHost.Show(new SqlDialogViewModel(new SqlDialog()));
        }

        [RelayCommand]
        public void HandleContentsClick(ContentsUnitViewModel vm)
        {
            switch (vm.Model)
            {
                case DrawerAsContents:
                    DialogHost.Show(new DrawerDialogViewModel());
                    break;
                case Widget:
                    DialogHost.Show(new SqlDialogViewModel(new SqlDialog()));
                    break;
                case Item item:
                    item.Execute();
                    break;
            }
        }

        [RelayCommand]
        public void HandleDatabaseClick(string dbType)
        {
            switch (dbType)
            {
                case "fake":
                    DialogHost.Show(new SqlDialogViewModel(new SqlDialog()));
                    break;
            }
        }

        [RelayCommand]
        public void HandleWrenchClick()
        {
            var w = new DevDrawerView();
            var vm = new DevDrawerViewModel();
            w.DataContext = vm;
            w.Position = vm.Point;
            w.Show();
            //DialogHost.Show(new DevDialogViewModel());
            //DialogHost.Show(new SqlDialogViewModel(new SqlDialog()));
        }
    }
}
