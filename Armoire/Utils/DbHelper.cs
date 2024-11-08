using Armoire.ViewModels;

namespace Armoire.Utils;

public class DbHelper
{
    public static void SaveDrawer(DrawerAsContentsViewModel dacVm)
    {
        using var context = new AppDbContext();
        var drawerToAdd = dacVm.CreateDrawer();
        OutputHelper.DebugPrintJson(drawerToAdd, "dacVm_something");
        context.TryAddDrawer(dacVm.CreateDrawer());
        context.SaveChanges();
    }
}
