using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public partial class StartMenuItemViewModel : ItemViewModel
    {

        [ObservableProperty]
        public bool _startMenuTextBoxOpen;

        [ObservableProperty]
        public string _clickedApp;

        public StartMenuItemViewModel() 
        {
            Name = "Start";
        }

        [RelayCommand]
        public void OpenStartMenu()
        {
            StartMenuTextBoxOpen = !StartMenuTextBoxOpen;
        }


    }
}
