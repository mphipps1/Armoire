using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using Armoire.Models;
using Armoire.Utils;
using Armoire.Utils;
using Armoire.Views;
using Armoire.Views;
using Avalonia.VisualTree;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using DialogHostAvalonia;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private int _onAddCount = 0;
        private int _contentsUnitCount;
        private int _drawerCount;
        private int _itemCount;
        private DevDrawerView? _devDrawerView;
        private NewItemViewModel? currentEntry;
        private Timer _timer;
        private string _currentTime;

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        [ObservableProperty]
        public static bool _dialogIsOpen;

        public static ObservableCollection<ContentsUnitViewModel> DockContents { get; set; } = [];

        private bool CanAddContentsUnit() => true;

        public MainWindowViewModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            UpdateTime();

            // Register event handlers.
            DockContents.CollectionChanged += dc_CollectionChanged;
            DockContents.CollectionChanged += dc_OnAdd;

            // Create DACVM for the dock and add it to the DB.
            var dockDac = new DrawerAsContentsViewModel(true);
            SaveDrawerToDb(dockDac);

            OutputHelper.DebugPrintJson(dockDac, "dockDac");

            var d1 = new DrawerAsContentsViewModel(dockDac, "apple");
            d1.DrawerAsContainer = new DrawerViewModel(1, d1);
            d1.DrawerHierarchy = 0;

            var d2 = new DrawerAsContentsViewModel(dockDac, "orange");
            d2.DrawerAsContainer = new DrawerViewModel(2, d2);
            d2.DrawerHierarchy = 0;

            DockContents.Add(d1);
            DockContents.Add(new ItemViewModel());
            DockContents.Add(d2);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateTime();
        }

        private void dc_OnAdd(object? sender, NotifyCollectionChangedEventArgs e)
        {
            using var context = new AppDbContext();
            if (e.NewItems is not { } ni)
                return;
            var i = 0;
            foreach (var item in ni)
            {
                if (item is DrawerAsContentsViewModel dacvm)
                {
                    var newContentsUnit = dacvm.GetDrawer();
                    context.TryAddDrawer(newContentsUnit);
                    OutputHelper.DebugPrintJson(dacvm, $"dacvm{i++}");
                }
            }
            OutputHelper.DebugPrintJson(context.Drawers, "drawers");

            _onAddCount++;
            // This line produces `'FOREIGN KEY constraint failed.'` exception
            context.SaveChanges();
        }

        private void SaveDrawerToDb(DrawerAsContentsViewModel dac)
        {
            using var context = new AppDbContext();
            var drawerToAdd = dac.GetDrawer();
            OutputHelper.DebugPrintJson(drawerToAdd, "dockDrawer");
            context.TryAddDrawer(drawerToAdd);
            OutputHelper.DebugPrintJson(context.Drawers, "drawersAfterDockAdd");
            context.SaveChanges();
        }

        private void UpdateTime()
        {
            CurrentTime = DateTime.Now.ToString("%h:mm tt");
        }

        private void dc_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // https://stackoverflow.com/a/8490996/16458003
            if (e.NewItems != null)
                foreach (ContentsUnitViewModel item in e.NewItems)
                    item.PropertyChanged += dc_PropertyChanged;

            if (e.OldItems != null)
                foreach (ContentsUnitViewModel item in e.OldItems)
                    item.PropertyChanged -= dc_PropertyChanged;
        }

        private void dc_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DeleteMe":
                    if (sender is ContentsUnitViewModel cu)
                        DockContents.Remove(cu);
                    break;
                default:
                    Debug.WriteLine("Different property changed.");
                    break;
            }
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
            DialogHost.Show(new NewItemViewModel(0, 0));
        }

        [RelayCommand]
        public void AddDrawerClick()
        {
            if (DockContents.Count < 10)
                DockContents.Add(new DrawerAsContentsViewModel());
            else
                DialogHost.Show(
                    new ErrorMessageViewModel($"The dock is full, it can\n only hold 10 items.")
                );
        }

        public static void CloseDialog()
        {
            DialogHost.GetDialogSession("MainDialogHost")?.Close(false);
        }
    }
}
