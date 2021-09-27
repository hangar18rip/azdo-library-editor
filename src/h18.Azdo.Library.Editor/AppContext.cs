using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace h18.Azdo.Library.Editor
{
    public class AppContext
    {

        static readonly AppContext _Default = new AppContext();
        public static AppContext Default
        {
            get
            {
                return _Default;
            }
        }

        public ObservableCollection<VariableGroup> AvailableGroups { get; } = new ObservableCollection<VariableGroup>();
        public ObservableCollection<VariableGroup> SelectedGroups { get; } = new ObservableCollection<VariableGroup>();
        public Dictionary<string, VariableGroup> ActiveGroups { get; } = new Dictionary<string, VariableGroup>(StringComparer.OrdinalIgnoreCase);

        public ObservableCollection<ProjectVariable> Variables { get; } = new ObservableCollection<ProjectVariable>();
        public TaskAgentHttpClient Client { get; private set; }


        public async Task<bool> Connect()
        {
            try
            {

                Client = await ConnectionContext.Default.Connection.GetClientAsync<TaskAgentHttpClient>();

                await LoadGroups();
                return true;
            }
            catch
            {
                return false;
            }
        }


        private async Task LoadGroups()
        {
            CleanGroups();

            AvailableGroups.Clear();
            AvailableGroups.AddRange(await Client.GetVariableGroupsAsync(ConnectionContext.Default.Project));

            SelectedGroups.Clear();
            SelectedGroups.AddRange(AppContext.Default.AvailableGroups);
        }

        private void CleanGroups()
        {
            ActiveGroups.Clear();
            CleanVariables();
        }

        //readonly List<DataGridBoundColumn> _Columns = new List<DataGridBoundColumn>();


        private void CleanVariables()
        {
            //foreach (var c in _Columns)
            //{
            //    dgGroups.Columns.Remove(c);
            //}
            //_Columns.Clear();
            //dgGroups.ItemsSource = null;
        }

        public async Task LoadVariables()
        {
            CleanVariables();


            var groups = AppContext.Default.SelectedGroups.OrderBy(e => e.Name);
            var vars = new Dictionary<string, ProjectVariable>(StringComparer.OrdinalIgnoreCase);
            foreach (var g in groups)
            {
                var g2 = await Client.GetVariableGroupAsync(ConnectionContext.Default.Project, g.Id);
                ActiveGroups[g2.Name] = g2;
                Console.WriteLine($"{g2.Name}");

                //var col = new DataGridTextColumn();
                //col.Header = g2.Name;
                //var b = new Binding($"[{g2.Name}]");
                //b.Mode = BindingMode.TwoWay;
                //col.Binding = b;
                //_Columns.Add(col);
                //dgGroups.Columns.Add(col);

                foreach (var k in g2.Variables.Keys)
                {
                    ProjectVariable pv;
                    if (vars.ContainsKey(k))
                    {
                        pv = vars[k];
                    }
                    else
                    {
                        pv = new ProjectVariable { Name = k };
                        vars[k] = pv;
                    }
                    var val = g2.Variables[k];
                    pv.IsSecret = pv.IsSecret | val.IsSecret;
                    pv.IsReadOnly = pv.IsReadOnly | val.IsReadOnly;
                    pv[g2.Name] = val.Value;
                }
            }
            var sorted = vars.Values.OrderBy(i => i.Name);
            Variables.Clear();
            Variables.AddRange(sorted);
        }

        public async Task UpdateVariables()
        {
            //if (dgGroups.ItemsSource == null)
            //{
            //    return;
            //}

            var items = Variables; // dgGroups.ItemsSource as ObservableCollection<ProjectVariable>;
            var vpgs = new Dictionary<string, VariableGroupParameters>(StringComparer.OrdinalIgnoreCase);
            foreach (var g in ActiveGroups.Values)
            {
                vpgs[g.Name] = new VariableGroupParameters { Name = g.Name, Description = g.Description, ProviderData = g.ProviderData, Type = g.Type, VariableGroupProjectReferences = g.VariableGroupProjectReferences };
            }
            foreach (var item in items)
            {
                foreach (var k in item.Keys)
                {
                    var tvar = vpgs[k].Variables;
                    if (tvar.ContainsKey(item.Name))
                    {
                        MessageBox.Show($"The variable '{item.Name}' already exists. Review the list of variable your are defining and retry. The update is cancelled.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        tvar.Add(item.Name, new VariableValue { Value = item[k], IsReadOnly = item.IsReadOnly, IsSecret = item.IsSecret });
                    }
                }
            }

            Console.WriteLine("");
            //var vgp = new VariableGroupParameters();

            foreach (var k in vpgs.Keys)
            {
                var groupId = ActiveGroups[k].Id;
                var vgp = vpgs[k];
                await Client.UpdateVariableGroupAsync(groupId, vgp);
            }

            await LoadVariables();
        }
    }
}
