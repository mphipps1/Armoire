using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
            var dock = MainWindowViewModel.ActiveDockViewModel.InnerContents;
            foreach (var unit in dock)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Name == TargetName)
                    {
                        dacvm.Name = NewName;
                    }
                    UpdateName(dacvm.GeneratedDrawer.InnerContents);
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
                    UpdateName(dacvm.GeneratedDrawer.InnerContents);
                }
            }
            MainWindowViewModel.CloseDialog();
        }
    }
}
