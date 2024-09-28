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
        private int _drawerCount;

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
            //await DialogHost.Show(new NewEntryPopUpViewModel());
            DrawerContents.Add(
                new ContentsUnitViewModel(new DrawerAsContents($"drawer {++_drawerCount}"))
            );
        }

        public MainWindowViewModel()
        {
            DrawerContents.Add(new ContentsUnitViewModel(new Widget("database", null)));
            DrawerContents.Add(new ContentsUnitViewModel(new DrawerAsContents("drawer 0")));
            DrawerContents.Add(new ContentsUnitViewModel(new Item("Paint", "C:\\WINDOWS\\system32\\mspaint.exe", "0")));
        }

        [RelayCommand]
        public void OpenSqlDialog()
        {
            DialogHost.Show(new SqlDialogViewModel(new SqlDialog()), dialogIdentifier: "Sql");
        }

        [RelayCommand]
        public void OpenDialog(ContentsUnitViewModel vm)
        {
            switch (vm.Model)
            {
                case DrawerAsContents:
                    DialogHost.Show(new DrawerDialogViewModel(), dialogIdentifier: "Drawer");
                    break;
                case Widget:
                    DialogHost.Show(
                        new SqlDialogViewModel(new SqlDialog()),
                        dialogIdentifier: "Sql"
                    );
                    break;
                case Item:
                    ((Item)vm.Model).Execute();
                    break;
            }
        }

        [RelayCommand]
        public void OpenDrawerDialog()
        {
            DialogHost.Show(new SqlDialogViewModel(new SqlDialog()), dialogIdentifier: "Drawer");
        }
    }
}
