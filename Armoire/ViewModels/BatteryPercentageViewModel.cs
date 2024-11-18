using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Armoire.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Armoire.ViewModels
{
    public partial class BatteryPercentageViewModel : ItemViewModel
    {
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
            Parent = container.SourceDrawer;
            Model = new Item(Name, "", parentID.ToString(), Position);
            Id = "BATTERY";
        }

        public async void UpdateNotificationArea()
        {
            var updateBatteryTask = UpdateBattery();

            await updateBatteryTask;
        }

        public async Task UpdateBattery()
        {
            while (true)
            {
                bool isRunningOnBattery = (
                    System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus
                    == PowerLineStatus.Offline
                );
                int batteryPercent = (int)(
                    System.Windows.Forms.SystemInformation.PowerStatus.BatteryLifePercent * 100
                );
                int secondsRemaining = System
                    .Windows
                    .Forms
                    .SystemInformation
                    .PowerStatus
                    .BatteryLifeRemaining;
                Name =
                    "Battery life remaining: "
                    + secondsRemaining / 3600
                    + " hours and "
                    + (secondsRemaining / 3600) % 60
                    + " minutes";
                //Debug.WriteLine(Name);
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
