using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace h18.Azdo.Library.Editor
{
    /// <summary>
    /// Interaction logic for GroupPicker.xaml
    /// </summary>
    public partial class GroupPicker : Window
    {
        public GroupPicker(IList<VariableGroup> availableGroups, IList<VariableGroup> selectedGroups)
        {
            InitializeComponent();
            AvailableGroups = availableGroups;
            lvGroups.DataContext = this;
            Loaded += GroupPicker_Loaded;
            foreach (var g in selectedGroups)
            {
                lvGroups.SelectedItems.Add(g);
            }
        }

        private void GroupPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loaded");
        }

        public IList<VariableGroup> AvailableGroups { get; private set; }
        public IList<VariableGroup> SelectedGroups { get; private set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            SelectedGroups = lvGroups.SelectedItems.Cast<VariableGroup>().ToList();
            this.DialogResult = true;
            //this.Close();
        }
    }
}
