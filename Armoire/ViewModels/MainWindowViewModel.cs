using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using Armoire.Views;
using Avalonia.VisualTree;
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

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public ObservableCollection<ContentsUnitViewModel> DockContents { get; set; } = [];

        private bool CanAddContentsUnit() => true;

        [RelayCommand(CanExecute = nameof(CanAddContentsUnit))]
        private void HandleDrawerAddClick()
        {
            currentEntry = new NewItemViewModel(DockContents);
            DialogHost.Show(currentEntry);
            //DockContents.Add(new DrawerAsContentsViewModel());
        }

        public MainWindowViewModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            UpdateTime();

            DockContents.CollectionChanged += dc_CollectionChanged;
            var d1 = new DrawerAsContentsViewModel
            {
                DrawerAsContainer = new DrawerViewModel()
            };
            var d2 = new DrawerAsContentsViewModel() { DrawerAsContainer = new DrawerViewModel(1, 0) };
            DockContents.Add(d1);
            DockContents.Add(new ItemViewModel());
            DockContents.Add(d2);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateTime();
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
    }
}
