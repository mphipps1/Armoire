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
            Id = "WEATHER";
        }

        public override void HandleContentsClick()
        {
            Process.Start(new ProcessStartInfo("https://openweathermap.org/") { UseShellExecute = true });
        }

        public async void UpdateWeather()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.TryStart(false, TimeSpan.FromMilliseconds(3000));
            DateTime start = DateTime.Now;
            bool abort = false;
            while (watcher.Status != GeoPositionStatus.Ready && !abort)
            {
                Thread.Sleep(200);
                if (DateTime.Now.Subtract(start).TotalSeconds > 5)
                    abort = true;
            }
            GeoCoordinate cord = watcher.Position.Location;
            var weather = await GetWeatherByCoordinates(cord.Latitude, cord.Longitude);
            CurrentTemp = (((int) (weather.Main.Temp * 9 / 5) + 32)).ToString() + "°F";
            WeatherDesc = weather.Weather[0].Description.Replace(" ", "\n");
            //using var httpClient = new HttpClient();
            //var imageBytes = await httpClient.GetByteArrayAsync($"https://openweathermap.org/img/wn/{weather.Weather[0].Icon}.png");
            //Bitmap bitmap;
            //using (var ms = new MemoryStream(imageBytes))
            //{
            //    bitmap = new Bitmap(ms);
            //}
            //using (MemoryStream memory = new MemoryStream())
            //{
            //    bitmap.Save(memory, ImageFormat.Png);
            //    memory.Position = 0;
            //    WeatherIconBmp = new Avalonia.Media.Imaging.Bitmap(memory);
            //}


        }

        private static async Task<WeatherResponse> GetWeatherByCoordinates(double latitude, double longitude)
        {
            using (var client = new HttpClient())
            {
                var weatherUrl = $"{WeatherApiUrl}?lat={latitude}&lon={longitude}&appid={WeatherApiKey}&units=metric";
                var response = await client.GetStringAsync(weatherUrl);
                return JsonConvert.DeserializeObject<WeatherResponse>(response);
            }
        }
        public class WeatherResponse
        {
            public string Name { get; set; }
            public Main Main { get; set; }
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
