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
            Id = "WIFI";
        }

        public override void HandleContentsClick()
        {
            Process.Start(new ProcessStartInfo("ms-settings:wifi") { UseShellExecute = true });
        }
    }
}
