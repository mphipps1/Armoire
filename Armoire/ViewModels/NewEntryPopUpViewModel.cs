using Armoire.Interfaces;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Armoire.ViewModels
{
    public partial class NewEntryPopUpViewModel : ViewModelBase
    {
        public NewEntryPopUpViewModel(ObservableCollection<ContentsUnitViewModel> dock) {
            Dock = dock;
        }
        public NewEntryPopUpViewModel(Item item)
        {
            Unit = item;
        }

        public NewEntryPopUpViewModel(Drawer drawer)
        {
            Drawer = drawer;
        }

        public IContentsUnit Unit { get; set; }
        public IDrawer Drawer { get; set; }

        public bool IsDrawer = false;

        private ObservableCollection<ContentsUnitViewModel> Dock { get; set; }

        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        public string _iconPath;

        [ObservableProperty]
        public string _path;

        [RelayCommand]
        public void UpdateEntryType()
        {
            IsDrawer = !IsDrawer;
        }

        public bool isDrawer()
        {
            return IsDrawer;
        }
        [RelayCommand]
        public void Update()
        {
            if (IsDrawer){
                int.TryParse(Path, out int i);
                //this DrawerAsContentsViewModel makes a DrawerViewModwl in its constructor
                Dock.Add(new DrawerAsContentsViewModel(i, Name, IconPath));
            } else {
                //this ItemViewModel constructor makes an Item model from the parameters
                Dock.Add(new ItemViewModel(Name, Path, "0"));
            }

        }

    }

}
