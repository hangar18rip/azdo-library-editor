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
using System.Windows.Shapes;

namespace h18.Azdo.Library.Editor
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        public Connect()
        {
            InitializeComponent();
            this.DataContext = ConnectionContext.Default;
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var connected = await ConnectionContext.Default.Connect();
            if (connected)
            {
                DialogResult = true;
            }
        }
    }
}
