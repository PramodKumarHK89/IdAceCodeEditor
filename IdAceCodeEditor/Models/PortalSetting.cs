using System;
using System.Collections.Generic;
using System.Text;

namespace IdAceCodeEditor
{
    public class PortalSetting
    {
        public string DisplayName { get; set; }
        public string SignInAudience { get; set; }
        public string AppType { get; set; }
        public string RedirectUri { get; set; }
        public string BrokeredUri { get; set; }
        public string IsHybridFlow { get; set; }
        public string IsDeviceCodeFlow { get; set; }
        public string SecretName{ get; set; }
        public string Certificate { get; set; }
        public List<PermissionScope> PermissionScopes { get; set; }
        public List<RequiredResourceAccess> RequiredResourceAccesss { get; set; }

    }
}
