using System;
using System.Collections.Generic;
using System.Text;

namespace IdAceCodeEditor
{
    public class Tag
    {
        private string _name;
        public string Name
        {
            get { return "#" + _name; }   // get method
            set { _name = value; }  // set method
        }
    }
}
