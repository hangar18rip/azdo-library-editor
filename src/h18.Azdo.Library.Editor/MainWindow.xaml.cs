using h18.Azdo.Library.Editor.Logic;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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
            DataContext = EditorContext.Default;
            ConnectionContext.Default.ConnectionStatusChanged += Default_ConnectionStatusChanged;

            CellStyle = this.Resources["ContentCellStyle"] as Style;
            ProdCellStyle = this.Resources["ProductionCellStyle"] as Style;
        }

        static readonly List<string> ProductionGroupNames = new List<string> { "prod", "prd", "qua", "qual", "pprd", "pprod" };

        readonly Style CellStyle;
        readonly Style ProdCellStyle;

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
            grdLoading.Visibility = Visibility.Visible;
            await EditorContext.Default.LoadVariables();
            grdLoading.Visibility = Visibility.Collapsed;
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            grdLoading.Visibility = Visibility.Visible;
            await EditorContext.Default.LoadVariables();
            grdLoading.Visibility = Visibility.Collapsed;
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            grdLoading.Visibility = Visibility.Visible;
            await EditorContext.Default.UpdateVariables();
            grdLoading.Visibility = Visibility.Collapsed;
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
            grdLoading.Visibility = Visibility.Visible;
            var gp = new GroupPicker(EditorContext.Default.AvailableGroups, EditorContext.Default.SelectedGroups);
            gp.Owner = this;
            if (gp.ShowDialog() == true)
            {
                EditorContext.Default.SelectedGroups.Clear();
                EditorContext.Default.SelectedGroups.AddRange(gp.SelectedGroups);
                await EditorContext.Default.LoadVariables();
                foreach (var c in _GroupColumns)
                {
                    dgGroups.Columns.Remove(c);
                }
                foreach (var g in EditorContext.Default.SelectedGroups)
                {
                    var col = new DataGridTextColumn();
                    col.Header = g.Name;
                    col.Binding = new Binding { Path = new PropertyPath($"[{g.Name}]", null), Mode = BindingMode.TwoWay };
                    col.Width = 200;
                    if (ProductionGroupNames.Where(e => e.Equals(g.Name, StringComparison.OrdinalIgnoreCase)).Count() > 0)
                    {
                        col.CellStyle = ProdCellStyle;
                        dgGroups.Columns.Add(col);
                    }
                    else
                    {
                        col.CellStyle = CellStyle;
                        dgGroups.Columns.Insert(2, col);
                    }
                    _GroupColumns.Add(col);
                }
            }
            grdLoading.Visibility = Visibility.Collapsed;
        }

        private async void main_Loaded(object sender, RoutedEventArgs e)
        {
            await Connect();
        }

        private async Task Connect()
        {
            grdLoading.Visibility = Visibility.Visible;
            var connect = new Connect();
            connect.Owner = this;
            if (connect.ShowDialog() == true)
            {
                try
                {
                    await EditorContext.Default.Connect();
                    await GetGroups();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while connecting and loading the projects : {ex.Message}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            grdLoading.Visibility = Visibility.Collapsed;
        }


        public GeneratePasswordCommand GeneratePassword { get; } = new GeneratePasswordCommand();

        public class GeneratePasswordCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Console.WriteLine($"{parameter}");
            }
        }
    }
}
