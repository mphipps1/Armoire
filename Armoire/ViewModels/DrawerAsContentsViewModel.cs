using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public class DrawerAsContentsViewModel : ContentsUnitViewModel
{
    private static int _count;
    public DrawerViewModel DrawerAsContainer { get; set; }

    public DrawerAsContentsViewModel()
    {
        Name = "drawer " + ++_count;
        IconPath = "/Assets/closedGradientDrawer.svg";
    }

    private DrawerAsContentsViewModel(DrawerAsContentsViewModel copyMe)
    {
        Name = copyMe.Name;
        IconPath = copyMe.IconPath;
        IconKind = copyMe.IconKind;
        DeleteMe = copyMe.DeleteMe;
    }
}
