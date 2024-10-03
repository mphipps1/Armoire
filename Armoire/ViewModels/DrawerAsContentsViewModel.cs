namespace Armoire.ViewModels;

public class DrawerAsContentsViewModel : ContentsUnitViewModel
{
    private static int _count;

    public DrawerAsContentsViewModel()
    {
        Name = "drawer " + ++_count;
    }
}
