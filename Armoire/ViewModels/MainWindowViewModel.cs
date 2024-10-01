using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Armoire.Models;
using Armoire.Utils;
using Armoire.Views;
using Avalonia.VisualTree;
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
        private DevDrawerView? _devDrawerView;

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
            var win11Path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft\\WindowsApps\\mspaint.exe"
            );
            DockContents.Add(new ContentsUnitViewModel(new Widget("database", null)));
            DockContents.Add(new ContentsUnitViewModel(new DrawerAsContents("drawer 0")));
            if (isWindows11())
                DockContents.Add(new ContentsUnitViewModel(new Item("Paint", win11Path, "0")));
            else
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
        private void DeleteDrawer(ContentsUnitViewModel drawer)
        {
            DockContents.Remove(drawer);
        }

        [RelayCommand]
        public void HandleContentsClick(ContentsUnitViewModel vm)
        {
            switch (vm.Model)
            {
                case DrawerAsContents:
                    //DialogHost.Show(new DrawerDialogViewModel());
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
            if (_devDrawerView is null)
            {
                _devDrawerView = new DevDrawerView();
                var vm = new DevDrawerViewModel();
                _devDrawerView.DataContext = vm;
                //w.Position = vm.Point;
                _devDrawerView.Show();
                foreach (var vc in _devDrawerView.GetVisualChildren())
                {
                    Debug.WriteLine("Reporting from HandleWrenchClick depth 1: " + vc);
                    foreach (var vc2 in vc.GetVisualChildren())
                    {
                        Debug.WriteLine("Reporting from HandleWrenchClick depth 2: " + vc2);
                        foreach (var vc3 in vc2.GetVisualChildren())
                        {
                            Debug.WriteLine("Reporting from HandleWrenchClick depth 3: " + vc3);
                            foreach (var vc4 in vc3.GetVisualChildren())
                            {
                                // One of these is the Canvas
                                Debug.WriteLine("Reporting from HandleWrenchClick depth 4: " + vc4);
                            }
                        }
                    }
                }
            }
            else
            {
                _devDrawerView.Close();
                _devDrawerView = null;
            }
            //DialogHost.Show(new DevDialogViewModel());
            //DialogHost.Show(new SqlDialogViewModel(new SqlDialog()));
        }

        public bool isWindows11()
        {
            //windows 11 machines will return a verion build greater that 22000,
            //probably not the best long term solution as this will return true when windows 12 releases
            //https://stackoverflow.com/questions/69038560/detect-windows-11-with-net-framework-or-windows-api
            return Environment.OSVersion.Version.Build >= 22000;
        }
    }
}
