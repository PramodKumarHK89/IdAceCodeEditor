using IdAceCodeEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace IdAceCodeEditor
{
    public class Framework
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public ObservableCollection<Sample> Samples { get; set; }
       
    }
}
