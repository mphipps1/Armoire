/*  This class manages the Armoire startmenu. Much of the logic for creating the UI is handled in StartMenuView.axaml.cs
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels
{
    public partial class StartMenuItemViewModel : ItemViewModel
    {
        [ObservableProperty]
        public bool _startMenuTextBoxOpen;


        [ObservableProperty]
        public string _clickedApp;

        public StartMenuItemViewModel(
            string parentID,
            int drawerHeirarchy,
            ContainerViewModel? container
        )
            : base(parentID, drawerHeirarchy, container)
        {
            Name = "Start";
            ExecutablePath = "";
            //Container = container;
            Model = new Item(Name, "", parentID.ToString(), Position);

            //Special ID so this item isnt added to the database
            Id = "START_MENU";
        }

        [RelayCommand]
        public void OpenStartMenu()
        {
            StartMenuTextBoxOpen = !StartMenuTextBoxOpen;
        }
    }
}
