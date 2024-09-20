using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _heading = "Welcome to Armoire!";
    }
}
