using Armoire.Interfaces;
using Armoire.Views;
using Avalonia.Input;
using System.Collections.ObjectModel;

namespace Armoire.ViewModels;

public class DrawerDialogViewModel : ViewModelBase {

    public ObservableCollection<ContentsUnitViewModel> drawerContents { get; } = [];

    private DrawerDialog drawerDialog = new DrawerDialog();

    public DrawerDialogViewModel()
    {

    }

    public void Btn_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        drawerDialog.Btn_PointerReleased(sender, e);
    }

    public void ToggleDrawer()
    {
        drawerDialog.ToggleDrawer();
    }
}
