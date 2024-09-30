using System.Collections.ObjectModel;

namespace Armoire.ViewModels;

public class DrawerDialogViewModel : ViewModelBase {

    public ObservableCollection<ContentsUnitViewModel> drawerContents { get; } = [];

    public DrawerDialogViewModel()
    {


    }

}
