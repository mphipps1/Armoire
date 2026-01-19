/*  Wifi is an item that launches the windows wifi settings on click
 *  This class should be updated in the future to show a custom UI
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Armoire.Models;

namespace Armoire.ViewModels
{
    public partial class WifiItemViewModel : ItemViewModel
    {
        public WifiItemViewModel(
            string parentID,
            int drawerHeirarchy,
            ContainerViewModel? container
        )
        : base(parentID, drawerHeirarchy, container)
        {
            Name = "Wifi Settings";
            ExecutablePath = "";
            Parent = container.SourceDrawer;
            Model = new Item(Name, "", parentID.ToString(), Position);

            //Special ID to prevent this item from being added to the database
            Id = "WIFI";
        }

        public override void HandleContentsClick()
        {
            // Launching windows settings to the wifi page
            Process.Start(new ProcessStartInfo("ms-settings:wifi") { UseShellExecute = true });
        }
    }
}
