using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels;

public partial class DevDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _buttonContent1 = "hi";

    public DevDialogViewModel() { }
}
