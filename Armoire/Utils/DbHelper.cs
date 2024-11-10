using Armoire.ViewModels;

namespace Armoire.Utils;

public class DbHelper
{
    public static void SaveDrawer(DrawerAsContentsViewModel dacVm)
    {
        using var context = new AppDbContext();
        var drawerToAdd = dacVm.CreateDrawer();
        OutputHelper.DebugPrintJson(drawerToAdd, "DbHelper_SaveDrawer_drawerToAdd");
        context.TryAddDrawer(drawerToAdd);
        context.SaveChanges();
    }
}
