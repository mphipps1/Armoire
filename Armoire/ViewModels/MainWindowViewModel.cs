﻿/*  MainWindow holds a the logic that should applied to the whole app or the main window
 *  It has a couple of static members such as DeletedUnits or DockViewModel which can be used by the whole app
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Armoire.Utils;
using Armoire.Views;
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
        public double ViewHeight { get; set; }

        //temp
        //dockSource is the DrawerAsContentsViewModel that holds the ActiveDockViewModel
        public static DrawerViewModel ActiveDockViewModel { get; set; }

        //List of DrawerAsContentsViews, used to close their flyouts when a rightclick occurs
        public static LinkedList<DrawerAsContentsView> DrawerAsContentsViews;

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

            DrawerAsContentsViews = new LinkedList<DrawerAsContentsView>();

            // Dock setup.
            var dockSource = DbHelper.LoadDockOrCreate();
            DbHelper.SaveDrawer(dockSource);
            ActiveDockViewModel = dockSource.GeneratedDrawer;

            // Initialize DeletedUnits; used for undo.
            if (DeletedUnits == null)
                DeletedUnits = new Stack<ContentsUnitViewModel>();

            // Get the list of apps in the start menu here instead of in the NewItemViewModel constructor to avoid lag.
            NewItemViewModel.GetExecutables();

            //Setting up the custom drawers/items like the NotificationArea and its contents
            var notif = new NotificationAreaViewModel(dockSource.Id, 0);
            ActiveDockViewModel.Contents.Add(notif);

            var bat = new BatteryPercentageViewModel(notif.Id, 1, notif.GeneratedDrawer);
            var sound = new SoundItemViewModel(notif.Id, 1, notif.GeneratedDrawer);
            var wifi = new WifiItemViewModel(notif.Id, 1, notif.GeneratedDrawer);
            notif.GeneratedDrawer.Contents.Add(bat);
            notif.GeneratedDrawer.Contents.Add(sound);
            notif.GeneratedDrawer.Contents.Add(wifi);

            try
            {
                var weather = new WeatherViewModel(notif.Id, 1, notif.GeneratedDrawer);
                notif.GeneratedDrawer.Contents.Add(weather);
                //weather.UpdateWeather();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not get weather. See the following\n" + ex);
            }

            var start = new StartMenuItemViewModel(dockSource.Id, 0, ActiveDockViewModel);
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
            CurrentTime = DateTime.Now.ToString("%H:mm");
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
                        ActiveDockViewModel,
                        ViewHeight
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
