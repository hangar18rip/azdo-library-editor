using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace azdo_library_editor.app
{
    public class ConnectionContext : INotifyPropertyChanged
    {
        private ConnectionContext()
        {
            var settings = azdo_library_editor.app.Properties.Settings.Default;
            Organization = settings.LastOrganization;
            Project = settings.LastProject;
            Pat = settings.LastPAT;

        }

        public static ConnectionContext Default { get; } = new ConnectionContext();

        public VssConnection Connection { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ConnectionStatusChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CanConnect)));
            }
        }

        private string _Organization;
        public string Organization
        {
            get
            {
                return _Organization;
            }
            set
            {
                if (string.Compare(_Organization, value, false) != 0)
                {
                    _Organization = value;
                    OnPropertyChanged(nameof(Organization));
                }
            }
        }


        private string _Project;
        public string Project
        {
            get
            {
                return _Project;
            }
            set
            {
                if (string.Compare(_Project, value, false) != 0)
                {
                    _Project = value;
                    OnPropertyChanged(nameof(Project));
                }
            }
        }

        private bool _IsConnected;

        public bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
            set
            {
                if (_IsConnected != value)
                {
                    _IsConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    if (ConnectionStatusChanged != null)
                    {
                        ConnectionStatusChanged(this, EventArgs.Empty);
                    }
                }
            }
        }


        private string _Pat;
        public string Pat
        {
            get
            {
                return _Pat;
            }
            set
            {
                if (string.Compare(_Pat, value, false) != 0)
                {
                    _Pat = value;
                    OnPropertyChanged(nameof(Pat));
                }
            }
        }

        public async Task<bool> Connect()
        {
            if (!CanConnect)
            {
                return false;
            }

            try
            {
                IsConnected = false;
                if (Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }

                var creds = new VssBasicCredential("pat", Pat);

                var orgUri = new Uri($"https://dev.azure.com/{Organization}/");
                Connection = new VssConnection(orgUri, creds);
                await Connection.ConnectAsync();

                IsConnected = true;

                var settings = azdo_library_editor.app.Properties.Settings.Default;
                settings.LastOrganization = Organization;
                settings.LastProject = Project;
                settings.LastPAT = Pat;
                settings.Save();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CanConnect
        {
            get
            {
                return IsValid(Organization) && IsValid(Project) && IsValid(Pat);
            }
        }

        private bool IsValid(string value)
        {
            return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        }
    }
}
