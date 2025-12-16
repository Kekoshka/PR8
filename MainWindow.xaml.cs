using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using PR8.Classes;
using PR8.Context;
using PR8.Elements;

namespace PR8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetUserCounter();
        }

        private async Task<Position> GetPosition(string place)
        {
            using HttpClient client = new HttpClient();
            HttpRequestMessage message = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://catalog.api.2gis.com/3.0/items/geocode?q={place}&fields=items.point&key=1773a1ab-82b0-4b49-b17c-6f971e169960")
            };
            var response = await client.SendAsync(message);
            var text = await response.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(text);
            JsonElement root = doc.RootElement;
            var result = root.GetProperty("result").GetProperty("items")[0].GetProperty("point");

            double lat = result.GetProperty("lat").GetDouble();
            double lon = result.GetProperty("lon").GetDouble();
            Position position = new()
            {
                Id = Guid.NewGuid(),
                Name = place,
                X = lat,
                Y = lon
            };
            return position;
        }
        public async Task<List<Classes.Day>> GetWeatherByPosition(Position position)
        {
            List<Classes.Day> daysList = new List<Classes.Day>();
            using HttpClient client = new HttpClient();
            HttpRequestMessage message = new();
            message.RequestUri = new Uri("https://api.weather.yandex.ru/graphql/query");
            message.Method = HttpMethod.Post;
            
            var query = "{\r\n  weatherByPoint(request: { lat: "+position.X.ToString().Replace(",",".")+", lon: "+ position.Y.ToString().Replace(",", ".") + "}) {\r\n    forecast {\r\n      days(limit: 10) {\r\n        hours {\r\n          time\r\n          temperature\r\n          humidity\r\n          pressure\r\n        }\r\n      }\r\n    }\r\n  }\r\n}";
            var requestBody = new
            {
                query 
            };    
            string jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            message.Content = content;
            message.Headers.Add("X-Yandex-Weather-Key", "demo_yandex_weather_api_key_ca6d09349ba0");
            var response = await client.SendAsync(message);
            var text = await response.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(text);
            JsonElement root = doc.RootElement;
            var days = root.GetProperty("data").GetProperty("weatherByPoint").GetProperty("forecast");
            for(int i = 0; i < 9; i++)
            {
                var day = days.GetProperty("days")[i];
                var dateOnly = day.GetProperty("hours")[0].GetProperty("time").GetDateTime();
                Classes.Day newDay = new()
                {
                    Id = Guid.NewGuid(),
                    Date = DateOnly.FromDateTime(dateOnly),
                    Hours = []
                };
                var hoursCount = day.GetProperty("hours").GetArrayLength();
                for (int j = 0; j < hoursCount; j++)
                {
                    var isGet = day.TryGetProperty("hours", out var hour);
                    var time = hour[j].GetProperty("time").GetDateTime();
                    var temperature = hour[j].GetProperty("temperature");
                    var humuduty = hour[j].GetProperty("humidity");
                    var pressure = hour[j].GetProperty("pressure");
                    Classes.Hour newHour = new()
                    {
                        Id = Guid.NewGuid(),
                        Humidity = humuduty.GetInt32(),
                        Pressure = pressure.GetInt32(),
                        Temperature = temperature.GetInt32(),
                        Time = TimeOnly.FromDateTime(time),
                    };
                    newDay.Hours.Add(newHour);
                }
                daysList.Add(newDay);
            }
            return daysList;
        }

        private async Task TryGetWeatherAsync()
        {
            try
            {
                var positionText = City.Text;
                using var context = new ApplicationContext();
                var existsWeather = context.Weathers.Include(w => w.Days).Include(w => w.Position).FirstOrDefault(w => w.Position.Name == positionText);
                if (existsWeather is null)
                {
                    var position = await GetPosition(positionText);
                    var days = await GetWeatherByPosition(position);
                    Weather newWeather = new()
                    {
                        Days = days,
                        Id = Guid.NewGuid(),
                        Position = position,
                        RequestTime = DateTime.UtcNow
                    };
                    context.Weathers.Add(newWeather);
                    context.SaveChanges();
                    AddUserCounter();
                }
                else if(existsWeather.RequestTime < DateTime.UtcNow.AddMinutes(-30))
                {
                    var days = await GetWeatherByPosition(existsWeather.Position);
                    existsWeather.Days.Clear();
                    existsWeather.Days = days;
                    existsWeather.RequestTime = DateTime.UtcNow;
                    context.SaveChanges();
                    AddUserCounter();
                }
                var weather = context.Weathers.Include(w => w.Days).ThenInclude(d => d.Hours).First(w => w.Position.Name == positionText);
                DaysParent.Children.Clear();
                foreach (var day in weather.Days.OrderBy(d => d.Date))
                {
                    DaysParent.Children.Add(new Elements.Day(day));
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private IPAddress GetIp() => IPAddress.Parse("127.0.0.1");

        private void AddUserCounter()
        {
            using var context = new ApplicationContext();
            var user = context.Users.FirstOrDefault(u => u.UserIp == GetIp());
            if (user is null)
            {
                User newUser = new()
                {
                    Id = Guid.NewGuid(),
                    RequestCount = 1,
                    UserIp = GetIp(),
                };
                context.Users.Add(newUser);
                context.SaveChanges();
                Count.Text = $"Запросов: {newUser.RequestCount}/30";
                return;
            }
            user.RequestCount += 1;
            context.SaveChanges();
            Count.Text = $"Запросов: {user.RequestCount}/30";
        }
        private void GetUserCounter()
        {
            using var context = new ApplicationContext();
            var user = context.Users.FirstOrDefault(u => u.UserIp == GetIp());
            if (user is null)
                return;
            Count.Text = $"Запросов: {user.RequestCount}/30";
        }

        private void TryGetWeather(object sender, RoutedEventArgs e)
        {
            _ = TryGetWeatherAsync();
        }
    }
}