using System;
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
        public static DockViewModel DockViewModel
        {
            get =>
                _dockViewModel
                ?? throw new InvalidOperationException("Invalid access: before MwVm init");
            private set => _dockViewModel = value;
        }

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

            // Create DockViewModel for the dock and save its corresponding Drawer model to the
            // database.
            DockViewModel = new DockViewModel() { Name = "dock" };
            DockViewModel.SaveToDb();

            // Create a sample drawer for the dock.
            var d1 = new DrawerAsContentsViewModel(DockViewModel, "apple");
            d1.InnerContainer = new DrawerViewModel(1, d1);
            d1.DrawerHierarchy = 0;

            // Create another sample drawer for the dock.
            var d2 = new DrawerAsContentsViewModel(DockViewModel, "orange");
            d2.InnerContainer = new DrawerViewModel(2, d2);
            d2.DrawerHierarchy = 0;

            // Add to the dock (this triggers dc_OnAdd).
            DockViewModel.InnerContents.Add(d1);
            DockViewModel.InnerContents.Add(new ItemViewModel());
            DockViewModel.InnerContents.Add(d2);
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
        public void HandleWrenchClick()
        {
            // TODO: This approach is Anti-MVVM. The ViewModel should be unaware of the View.
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

        [RelayCommand]
        public void AddItemClick()
        {
            DialogHost.Show(new NewItemViewModel("CONTENTS_1", 0, true));
        }

        [RelayCommand]
        public void AddDrawerClick()
        {
            if (DockViewModel.InnerContents.Count < 10)
                DockViewModel.InnerContents.Add(new DrawerAsContentsViewModel());
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
