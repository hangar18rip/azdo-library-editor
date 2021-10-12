using System.Windows;
using h18.Azdo.Library.Editor.Logic;

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
            var settings = h18.Azdo.Library.Editor.Properties.Settings.Default;
            ConnectionContext.Default.Organization = settings.LastOrganization;
            ConnectionContext.Default.Project = settings.LastProject;
            ConnectionContext.Default.Pat = settings.LastPAT;

            this.DataContext = ConnectionContext.Default;
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var connected = await ConnectionContext.Default.Connect();
            if (connected)
            {
                try
                {

                    var settings = h18.Azdo.Library.Editor.Properties.Settings.Default;
                    settings.LastOrganization = ConnectionContext.Default.Organization;
                    settings.LastProject = ConnectionContext.Default.Project;
                    settings.LastPAT = ConnectionContext.Default.Pat;
                    settings.Save();

                    DialogResult = true;

                }
                catch
                {
                    this.Close();
                }
            }
        }
    }
}
