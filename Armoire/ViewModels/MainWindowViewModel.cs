using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Armoire.Utils;
using Armoire.Views;
using Avalonia;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public static Task TaskCheck;
        private int _contentsUnitCount;
        private int _drawerCount;
        private int _itemCount;
        private DevDrawerView? _devDrawerView;
        private NewItemViewModel? currentEntry;
        private System.Timers.Timer _timer;
        private string _currentTime;
        private static DockViewModel? _dockViewModel;
        public static Stack<ContentsUnitViewModel> DeletedUnits;

        //temp
        //dockSource is the DrawerAsContentsViewModel that holds the ActiveDockViewModel
        public static DrawerViewModel ActiveDockViewModel { get; set; }

        //public static DockViewModel ActiveDockViewModel
        //{
        //    get =>
        //        _dockViewModel
        //        ?? throw new InvalidOperationException("Invalid access: before MwVm init");
        //    private set => _dockViewModel = value;
        //}

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        [ObservableProperty]
        public static bool _dialogIsOpen;

        private bool CanAddContentsUnit() => true;

        public MainWindowViewModel()
        {
            // Clock setup.
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
            UpdateTime();

            // Dock setup.
            var dockSource = DbHelper.LoadDockOrCreate();
            DbHelper.SaveDrawer(dockSource);
            ActiveDockViewModel = dockSource.GeneratedDrawer;

            // Initialize DeletedUnits; used for undo.
            if (DeletedUnits == null)
                DeletedUnits = new Stack<ContentsUnitViewModel>();

            // Get the list of apps in the start menu here instead of in the NewItemViewModel constructor to avoid lag.
            NewItemViewModel.GetExecutables();

            var notif = new NotificationAreaViewModel(dockSource.Id, 0);
            ActiveDockViewModel.Contents.Add(notif);

            var start = new StartMenuItemViewModel(notif.Id, 1, notif.GeneratedDrawer);
            var bat = new BatteryPercentageViewModel(dockSource.Id, 1, ActiveDockViewModel);
            notif.GeneratedDrawer.Contents.Add(bat);

            ActiveDockViewModel.Contents.Add(start);

            var apps = new ApplicationMonitorViewModel(dockSource.Id, 0);
            ActiveDockViewModel.Contents.Add(apps);
            apps.GetInitialRunningApps();
            //apps.DisplayProcess();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            CurrentTime = DateTime.Now.ToString("%h:mm tt");
        }

        [RelayCommand]
        public void AddItemClick()
        {
            DialogHost.Show(
                new NewItemViewModel(
                    "CONTENTS_1",
                    ActiveDockViewModel.SourceDrawer.DrawerHierarchy,
                    ActiveDockViewModel
                )
            );
        }

        [RelayCommand]
        public void AddDrawerClick()
        {
            if (ActiveDockViewModel.Contents.Count < 10)
                DialogHost.Show(
                    new NewDrawerViewModel(
                        "CONTENTS_1",
                        ActiveDockViewModel.SourceDrawer.DrawerHierarchy,
                        ActiveDockViewModel
                    )
                );
            else
                DialogHost.Show(
                    new ErrorMessageViewModel($"The dock is full, it can\n only hold 10 items.")
                );
        }

        [RelayCommand]
        public void Logoff()
        {
            WindowsFunctionalities.LogOff();
        }

        [RelayCommand]
        public void Restart()
        {
            WindowsFunctionalities.Restart();
        }

        [RelayCommand]
        public void Shutdown()
        {
            WindowsFunctionalities.Shutdown();
        }

        public static void CloseDialog()
        {
            DialogHost.GetDialogSession("MainDialogHost")?.Close(false);
        }

        [RelayCommand]
        public void OpenWindowsSetting()
        {
            Process.Start(new ProcessStartInfo("ms-settings:") { UseShellExecute = true });
        }
    }
}
