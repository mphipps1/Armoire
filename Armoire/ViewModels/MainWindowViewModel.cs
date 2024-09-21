using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _headingMain = "Welcome to Armoire!";

        [ObservableProperty]
        private string _headingDock = "Dock";

        [ObservableProperty]
        private string _headingDockAlt = "Dock Alternate";
    }
}
