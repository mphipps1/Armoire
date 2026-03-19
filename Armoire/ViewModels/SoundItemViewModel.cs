/*  SoundItem is a custom item that launches the sound settings
 *  In the future, this can be updated to show a custom UI instead of launching windows settings
 * 
 */

using Armoire.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("ms-settings:sound") { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start(new ProcessStartInfo("open", "x-apple.systempreferences:com.apple.preference.sound") { UseShellExecute = false });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: Try common sound settings commands in order of preference
               if(!TryStartProcess("gnome-control-center", "sound"))
                   if(!TryStartProcess("pavucontrol"))
                       if(!TryStartProcess("xfce4-mixer"))
                           TryStartProcess("kcmshell5", "kcm_pulseaudio");
            }
        }
        private bool TryStartProcess(string fileName, string arguments = "")
        {
            try
            {
                Process.Start(new ProcessStartInfo(fileName, arguments) { UseShellExecute = true });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
