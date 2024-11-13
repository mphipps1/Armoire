using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Armoire.Utils;
using Armoire.Views;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private int _contentsUnitCount;
        private int _drawerCount;
        private int _itemCount;
        private DevDrawerView? _devDrawerView;
        private NewItemViewModel? currentEntry;
        private Timer _timer;
        private string _currentTime;
        private static DockViewModel? _dockViewModel;
        public static Stack<ContentsUnitViewModel> DeletedUnits;

        //temp
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
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            UpdateTime();
            var dockSource = new DrawerAsContentsViewModel(null, 0);
            //ActiveDockViewModel = (DockViewModel)dockSource.GeneratedDrawer;
            ActiveDockViewModel = dockSource.GeneratedDrawer;
            DbHelper.SaveDrawer(dockSource);

            // Create a sample drawer for the dock.
            var d1 = new DrawerAsContentsViewModel(
                dockSource.GeneratedDrawer,
                "sample 1",
                dockSource.Id,
                0
            );

            // Create another sample drawer for the dock.
            var d2 = new DrawerAsContentsViewModel(
                dockSource.GeneratedDrawer,
                "sample 2",
                dockSource.Id,
                0
            );

            // Add sample drawer 1 to the dock.
            ActiveDockViewModel.Contents.Add(d1);

            if (DeletedUnits == null)
                DeletedUnits = new Stack<ContentsUnitViewModel>();

            //getting the list of apps in the start menu here instead of in the NewItemViewModel contructor to avoid lag
            NewItemViewModel.GetExecutables();

            // Add sample item to the dock.
            if (
                NewItemViewModel.ExecutableNames is { } exeNames
                && NewItemViewModel.Executables is { } exes
                && NewItemViewModel.Icons is { } icons
            )
            {
                var rnd = new Random();
                var sampleItemIdx = rnd.Next(exeNames.Count);
                var sampleItemName = exeNames[sampleItemIdx];
                ActiveDockViewModel.Contents.Add(
                    new ItemViewModel(
                        sampleItemName,
                        exes[sampleItemName],
                        icons[sampleItemName].ToBitmap(),
                        dockSource.Id,
                        0
                    )
                );
            }

            // Add sample drawer 2 to the dock.
            ActiveDockViewModel.Contents.Add(d2);
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
            DialogHost.Show(new NewItemViewModel("CONTENTS_1", 0));
        }

        [RelayCommand]
        public void AddDrawerClick()
        {
            if (ActiveDockViewModel.Contents.Count < 10)
                DialogHost.Show(new NewDrawerViewModel("CONTENTS_1", 0));
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
