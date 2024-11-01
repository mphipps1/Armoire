using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public partial class ChangeDrawerNameViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _newName;

        private string TargetName;
        private string DialogHostName;

        public ChangeDrawerNameViewModel(string targetName)
        {
            TargetName = targetName;
           
        }



        [RelayCommand]
        //used for the base dock to get the recursive step set up
        public void UpdateName()
        {
            var dock = MainWindowViewModel.DockContents;
            foreach (var unit in dock)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Name == TargetName)
                    {
                        dacvm.Name = NewName;
                    }
                    UpdateName(dacvm.InnerContainer.InnerContents);
                }
            }
        }
        //recursive calls to all nested drawers
        public void UpdateName(ObservableCollection<ContentsUnitViewModel> Contents)
        {
            foreach (var unit in Contents)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Name == TargetName)
                    {
                        dacvm.Name = NewName;
                    }
                    UpdateName(dacvm.InnerContainer.InnerContents);
                }
            }
            MainWindowViewModel.CloseDialog();
        }

    }
}
