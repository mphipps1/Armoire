/* This class handles updating the battery percentage displayed in the NotificationArea drawer
 * This class inheretes from ItemViewModel but doesn't support the option to be clicked.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Armoire.Interfaces;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Devices;

namespace Armoire.ViewModels
{
    public partial class BatteryPercentageViewModel : ItemViewModel
    {
        // The BatteryIcon is an Avalonia material icon found at https://pictogrammers.com/library/mdi/
        // This website has the icons formated as battery-10 or battery-charging-10, AvaloniaMaterialIcons need to be formatted as
        // Battery10 or BatteryCharging10
        [ObservableProperty]
        public string _batteryIcon;

        [ObservableProperty]
        public string _batteryPercentage;

        public BatteryPercentageViewModel(
            string parentID,
            int drawerHeirarchy,
            ContainerViewModel? container
        )
            : base(parentID, drawerHeirarchy, container)
        {
            UpdateNotificationArea();
            Name = "Battery life remaining: ";
            ExecutablePath = "";
            if( container != null ) 
                Parent = container.SourceDrawer;
            Model = new Item(Name, "", parentID.ToString(), Position);

            //The Id is BATTERY so it isn't added to the database
            Id = "BATTERY";

            //Setting the icons to be default
            BatteryIcon = "BATTERY100";
            BatteryPercentage = "100";
        }

        // This function uses async and await to infinately check for the battery status
        public async void UpdateNotificationArea()
        {
            var updateBatteryTask = UpdateBattery();

            await updateBatteryTask;
        }

        public async Task UpdateBattery()
        {
            while (true)
            {
                // Check if the battery is plugged in
                bool isMachineRunningOnBattery = (
                        Battery.PowerSource == BatteryPowerSource.Battery 
                );
                // Get the battery power as a percentage
                int batteryPercent = (int)(
                    Battery.ChargeLevel * 100
                );
                
                // Updating the name to reflect how much lifetime the battery has left
                var isRunningOnBattery = ICrossPlatform.Instance.IsOnBattery();
                //var batteryPercent = ICrossPlatform.Instance.BatteryLevel();
                //int secondsRemaining = ICrossPlatform.Instance.BatteryLifeRemainingInSeconds(); 
                //TODO figure out how to get battery life remaining in seconds.
                //Debug.WriteLine(Name);

                // Changing the battery percentage and icon to refelct the current battery state
                // For info about how to format the AvaloniaMaterialIcon, see the top
                BatteryPercentage = (batteryPercent).ToString() + "%";
                if (isRunningOnBattery)
                    BatteryIcon = $"Battery{(((int)(batteryPercent / 10)) * 10).ToString()}";
                else
                    BatteryIcon = BatteryIcon =
                        $"BatteryCharging{(((int)(batteryPercent / 10)) * 10).ToString()}";
                await Task.Delay(1000);
            }
        }
    }
}
