using System.Net.Http;
using System.Text;
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
        }
        private async Task LoadDataAsync(string place)
        {
            using HttpClient client = new HttpClient();
            HttpRequestMessage message = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://catalog.api.2gis.com/3.0/items/geocode?q={place}&fields=items.point&key=1773a1ab-82b0-4b49-b17c-6f971e169960")
            };
            var response = await client.SendAsync(message);
            var json = await response.Content.ReadAsStringAsync();
        }
        private void GetPosition()
        {
        }
    }
}