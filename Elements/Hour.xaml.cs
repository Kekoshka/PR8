using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR8.Elements
{
    /// <summary>
    /// Логика взаимодействия для Hour.xaml
    /// </summary>
    public partial class Hour : UserControl
    {
        public Hour(Classes.Hour hour)
        {
            InitializeComponent();
            Temperature.Text = hour.Temperature.ToString();
            Pressure.Text = hour.Pressure.ToString();
            Time.Text = hour.Time.ToString();
            Humidity.Text = hour.Humidity.ToString();
        }
    }
}
