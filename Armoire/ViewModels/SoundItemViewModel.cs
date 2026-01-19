/*  SoundItem is a custom item that launches the sound settings
 *  In the future, this can be updated to show a custom UI instead of launching windows settings
 * 
 */

using Armoire.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armoire.ViewModels
{
    public partial class SoundItemViewModel : ItemViewModel
    {
        public SoundItemViewModel(
            string parentID,
            int drawerHeirarchy,
            ContainerViewModel? container
        )
            : base(parentID, drawerHeirarchy, container)
        {
            Name = "Sound Settings";
            ExecutablePath = "";
            Parent = container.SourceDrawer;
            Model = new Item(Name, "", parentID.ToString(), Position);

            //Special ID to ensure that this item isnt added to the database
            Id = "SOUND";
        }

        public override void HandleContentsClick()
        {
            // Launches windows settings to the sound page
            Process.Start(new ProcessStartInfo("ms-settings:sound") { UseShellExecute = true });
        }
    }
}
