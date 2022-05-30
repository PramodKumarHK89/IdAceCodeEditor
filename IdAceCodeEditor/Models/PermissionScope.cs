using System;
using System.Collections.Generic;
using System.Text;

namespace IdAceCodeEditor
{
    public class PermissionScope
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public string AdminConsentDisplayName { get; set; }
        public string UserConsentDisplayName { get; set; }
        public string AdminConsentDescription { get; set; }
        public string UserConsentDescription { get; set; }
    }
}
