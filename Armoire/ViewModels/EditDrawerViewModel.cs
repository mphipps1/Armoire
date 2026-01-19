/*  This class is where the logic for editing a drawer goes
 *  As of writing this comment, the only function is renaming the drawer
 * 
 */

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels
{
    public partial class EditDrawerViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _newName;

        private string TargetName;
        private string DialogHostName;

        public EditDrawerViewModel(string targetName)
        {
            TargetName = targetName;
        }

        [RelayCommand]
        //used for the base dock to get the recursive step set up
        public void UpdateName()
        {
            var dock = MainWindowViewModel.ActiveDockViewModel;
            foreach (var unit in dock.Contents)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Name == TargetName)
                    {
                        dock.RegisterEventHandlers();
                        dacvm.Name = NewName;
                    }
                    UpdateName(dacvm.GeneratedDrawer);
                }
            }
        }

        //recursive calls to all nested drawers
        public void UpdateName(ContainerViewModel container)
        {
            foreach (var unit in container.Contents)
            {
                if (unit is DrawerAsContentsViewModel dacvm)
                {
                    if (dacvm.Name == TargetName)
                    {
                        container.RegisterEventHandlers();
                        dacvm.Name = NewName;
                    }
                    UpdateName(dacvm.GeneratedDrawer);
                }
            }
            MainWindowViewModel.CloseDialog();
        }

        [RelayCommand]
        public void Cancel()
        {
            MainWindowViewModel.CloseDialog();
        }
    }
}
