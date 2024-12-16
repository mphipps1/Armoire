/*   Weather is an item that displays the current temp and conditions.
 *   It uses an API call to https://openweathermap.org/
 * 
 */

using Armoire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using Avalonia.Controls;
using Newtonsoft.Json;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;


namespace Armoire.ViewModels
{
    public partial class WeatherViewModel : ItemViewModel
    {
        private static readonly string WeatherApiKey = "09c421e0e58ea142b0514e9365088eba";
        private static readonly string WeatherApiUrl = "http://api.openweathermap.org/data/2.5/weather";

        [ObservableProperty]
        public string _weatherDesc;

        [ObservableProperty]
        public string _currentTemp;

        [ObservableProperty]
        public Avalonia.Media.Imaging.Bitmap _weatherIconBmp;

        public WeatherViewModel(string parentID, int? drawerHierarchy, ContainerViewModel? container) : base(parentID, drawerHierarchy, container)
        {
            Name = "Battery life remaining: ";
            ExecutablePath = "";
            Parent = container.SourceDrawer;
            Model = new Item(Name, "", parentID.ToString(), Position);

            //Special ID to prevent this item from being added to the database
            Id = "WEATHER";
            WeatherTasks();
        }

        //TODO: add location and unit preferences so the website opens to the current position of the user with the appropriate units
        public override void HandleContentsClick()
        {
            Process.Start(new ProcessStartInfo("https://openweathermap.org/") { UseShellExecute = true });
        }

        //Async function that handles checking the weather every 1.6667 minutes
        public async void WeatherTasks()
        {
            if (string.IsNullOrEmpty(CurrentTemp))
            {
                CurrentTemp = "";
                WeatherDesc = "Getting location data...";
            }
            await UpdateWeather();
        }

        public async Task UpdateWeather()
        {
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            while (true)
            {
                try
                {
                    var weather = await GetWeatherByCoordinates(this);
                    if(weather.Main == null)
                    {
                        break;
                    }

                    CurrentTemp = (((int)(weather.Main.Temp * 9 / 5) + 32)).ToString() + "°F";
                    WeatherDesc = weather.Weather[0].Description.Replace(" ", "\n");
                    await Task.Delay(100000);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not get weather. See the following exception\n" + ex);
                    WeatherDesc = "Could not load weather data.";
                    break;
                }

            }


        }

        private static async Task<WeatherResponse> GetWeatherByCoordinates(WeatherViewModel wvm)
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.TryStart(false, TimeSpan.FromMilliseconds(3000));
            //Making sure that we can get the weatherdata or else this results in an infinate loop
            while (watcher.Status != GeoPositionStatus.Ready)
            {
                if(watcher.Permission == GeoPositionPermission.Denied)
                {
                    // Display an error message in place of the weahter
                    wvm.WeatherDesc = "Could not load weather data.";
                    wvm.CurrentTemp = "Be sure to enable location in Windows Privacy Settings.";
                    return new WeatherResponse();
                }
                Thread.Sleep(200);
            }
            int i = 0;
            GeoCoordinate cord = watcher.Position.Location;
            // Sending API request and getting the JSON object it returns
            using (var client = new HttpClient())
            {
                var weatherUrl = $"{WeatherApiUrl}?lat={cord.Latitude}&lon={cord.Longitude}&appid={WeatherApiKey}&units=metric";
                var response = await client.GetStringAsync(weatherUrl);
                return JsonConvert.DeserializeObject<WeatherResponse>(response);
            }
        }

        //Objects used in parsing the JSON object
        public class WeatherResponse
        {
            public string Name { get; set; }
            public Main? Main { get; set; }
            public Weather[] Weather { get; set; }
        }

        public class Main
        {
            public double Temp { get; set; }
        }

        public class Weather
        {
            public string Description { get; set; }
            public string Icon { get; set; }
        }
    }
}
