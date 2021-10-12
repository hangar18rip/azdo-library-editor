using System.Collections.Generic;

namespace h18.Azdo.Library.Editor.Logic
{
    public class ProjectVariable : Dictionary<string, string>
    {
        public string Name { get; set; }
        public bool IsSecret { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;

    }
}
