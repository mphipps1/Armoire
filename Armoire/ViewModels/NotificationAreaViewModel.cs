/*  The NotificationArea holds the battery info, sound and wifi settings along with the weather
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Armoire.ViewModels;

public partial class NotificationAreaViewModel : DrawerAsContentsViewModel
{
    public NotificationAreaViewModel(string? parentID, int drawerHeirarchy)
        : base(parentID, drawerHeirarchy, true)
    {
        Name = "Notification Area";
        GeneratedDrawer = new DrawerViewModel(this);
        SetMoveDirections(this);

        //Special ID to prevent this DrawerAsContents from being added to the database
        Id = "NOTIFICATIONS";
    }
}
