using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace h18.Azdo.Library.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = AppContext.Default;
            ConnectionContext.Default.ConnectionStatusChanged += Default_ConnectionStatusChanged;
        }

        private void Default_ConnectionStatusChanged(object sender, EventArgs e)
        {
            var canConnect = ConnectionContext.Default.CanConnect;
            btnGroups.IsEnabled = canConnect;
            btnLoad.IsEnabled = canConnect;
            btnRefresh.IsEnabled = canConnect;
            btnUpdate.IsEnabled = canConnect;
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await AppContext.Default.LoadVariables();
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            await AppContext.Default.LoadVariables();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            await AppContext.Default.UpdateVariables();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            await Connect();
        }

        private async void btnGroups_Click(object sender, RoutedEventArgs e)
        {
            await GetGroups();
        }

        IList<DataGridTextColumn> _GroupColumns = new List<DataGridTextColumn>();

        private async Task GetGroups()
        {
            var gp = new GroupPicker(AppContext.Default.AvailableGroups, AppContext.Default.SelectedGroups);
            gp.Owner = this;
            if (gp.ShowDialog() == true)
            {
                AppContext.Default.SelectedGroups.Clear();
                AppContext.Default.SelectedGroups.AddRange(gp.SelectedGroups);
                await AppContext.Default.LoadVariables();
                foreach (var c in _GroupColumns)
                {
                    dgGroups.Columns.Remove(c);
                }
                foreach (var g in AppContext.Default.SelectedGroups)
                {
                    var col = new DataGridTextColumn();
                    col.Header = g.Name;
                    col.Binding = new Binding { Path = new PropertyPath($"[{g.Name}]", null), Mode = BindingMode.TwoWay };
                    col.Width = 200;
                    dgGroups.Columns.Add(col);
                    _GroupColumns.Add(col);
                }
            }
        }

        private async void main_Loaded(object sender, RoutedEventArgs e)
        {
            await Connect();
        }

        private async Task Connect()
        {
            var connect = new Connect();
            connect.Owner = this;
            if (connect.ShowDialog() == true)
            {
                try
                {
                    await AppContext.Default.Connect();
                    await GetGroups();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while connecting and loading the projects : {ex.Message}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
