using System.ComponentModel;

namespace h18.Azdo.Library.Editor.Logic
{
    public class LibraryEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public LibraryEntry(string value)
        {
            _Value = value;
        }

        private string _Value;
        public string Value
        {
            get { return _Value; }
            set
            {
                if (string.Equals(_Value, value, System.StringComparison.InvariantCulture))
                {
                    _Value = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(Value)));
                    }
                }
            }
        }
    }
}
