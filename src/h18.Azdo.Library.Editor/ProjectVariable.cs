using System.Collections.Generic;

namespace azdo_library_editor.app
{
    public class ProjectVariable : Dictionary<string, string>
    {
        public string Name { get; set; }
        public bool IsSecret { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;

    }
}
