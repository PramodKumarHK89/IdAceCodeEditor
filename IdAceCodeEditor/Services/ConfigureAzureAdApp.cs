using System;
using System.Collections.Generic;
using System.Text;
using Application = Microsoft.Graph.Application;
using System.Linq;
using Microsoft.Identity.Client;
using System.Windows.Interop;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Windows;
using IdAceCodeEditor.Views;

namespace IdAceCodeEditor
{
    public class ConfigureAzureAdApp
    {
        Dictionary<string, string> _persistData;
        Project _project;
        string _token;
        public ConfigureAzureAdApp(Dictionary<string, string> persistData,Project project)
        {
            _project = project;
            _persistData = persistData;
        }

        public async Task<AuthenticationResult> AuthenticateWithAzureAd(string[] scope,Window window)
        {
            try
            {

                AuthenticationResult authResult = null;
                var app = App.PublicClientApp;

                IAccount firstAccount;
                var accounts = await app.GetAccountsAsync();
                firstAccount = accounts.FirstOrDefault();
                try
                {
                    authResult = await app.AcquireTokenInteractive(scope)
                        .WithLoginHint("pramkum@pramkumlab.onmicrosoft.com")
                        .WithAccount(firstAccount)
                        .WithParentActivityOrWindow(new WindowInteropHelper(window).Handle)
                        .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                        .ExecuteAsync();

                    _token = authResult.AccessToken;
                    string key = string.Format("persistdata:Projects[{0}]", _project.Order);
                    string dummy;
                    if(!_persistData.TryGetValue(key + ".TenantId",out dummy))
                        _persistData.Add(key + ".TenantId", authResult.TenantId);
                    var tenantName = new System.Net.Mail.MailAddress(authResult.Account.Username);
                    if(!_persistData.TryGetValue(key + ".Domain", out dummy))
                        _persistData.Add(key + ".Domain", tenantName.Host);
                }
                catch (MsalException msalex)
                {
                    //MessageBox.Show($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }

                return authResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<PasswordCredential> CreateSecret(string appId)
        {
            var passwordCredential = new PasswordCredential
            {
                DisplayName = _project.PortalSettings.SecretName
            };

            try
            {
                var graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }));
                var secretresponse = await graphClient.Applications[appId]
                .AddPassword(passwordCredential)
                .Request()
                .PostAsync();              
                return secretresponse;
            }
            catch (Exception ex)
            {
                return null;
            }                
        }
        public async Task<bool> PostRegupdates()
        {
            try
            {
                var graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }));

                if (_project.PostRegUpdates != null)
                {
                    foreach (var item in _project!.PostRegUpdates)
                    {
                        string source = "", dest = "";
                        if (item.Source.StartsWith("persistdata:"))
                        {
                            _persistData.TryGetValue(item.Source, out source);
                        }
                        if (item.Destination.StartsWith("persistdata:"))
                        {
                            _persistData.TryGetValue(item.Destination, out dest);
                        }
                        var appObject = await graphClient.Applications[source]
                        .Request().GetAsync();

                        if (item.Name.Equals("knownClientApplications"))
                        {
                            appObject.Api.KnownClientApplications = new List<Guid>() { new Guid(dest) };
                        }
                        if (item.Name.Equals("PreAuthorizedApplications"))
                        {
                            appObject.Api.PreAuthorizedApplications = new List<PreAuthorizedApplication>() { new PreAuthorizedApplication() { AppId = dest } };
                        }
                        await graphClient.Applications[appObject.Id]
                      .Request()
                      .UpdateAsync(appObject);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<Application> CreateAppRegistration()
        {
            var httpClient = new System.Net.Http.HttpClient();
            bool isHybrid = false;
            if (_project.PortalSettings.IsHybridFlow.Equals("true", StringComparison.InvariantCultureIgnoreCase)) isHybrid = true;

            try
            {
                var graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }));

                Application application = new Application();
                application.SignInAudience = _project.PortalSettings.SignInAudience;
                application.DisplayName = _project.PortalSettings.DisplayName;

                List<Microsoft.Graph.RequiredResourceAccess> scopeList = new List<Microsoft.Graph.RequiredResourceAccess>();
                foreach (var item in _project.PortalSettings.RequiredResourceAccesss)
                {
                    var scopeResource = new Microsoft.Graph.RequiredResourceAccess();
                    List<Microsoft.Graph.ResourceAccess> raList = new List<Microsoft.Graph.ResourceAccess>();

                    if (item.ResourceAppId.StartsWith("persistdata:"))
                    {
                        string value;
                        _persistData.TryGetValue(item.ResourceAppId, out value);
                        scopeResource.ResourceAppId = value;
                    }
                    else
                        scopeResource.ResourceAppId = item.ResourceAppId;
                    foreach (var scp in item.ResourceAccesss)
                    {
                        var scope = new Microsoft.Graph.ResourceAccess();

                        if (scp.Id.StartsWith("persistdata:"))
                        {
                            string value;
                            _persistData.TryGetValue(scp.Id, out value);
                            scope.Id = new Guid(value);
                        }
                        else
                            scope.Id = new Guid(scp.Id);

                        scope.Type = scp.Type;
                        raList.Add(scope);
                    }
                    scopeResource.ResourceAccess = raList;
                    scopeList.Add(scopeResource);
                }

                application.RequiredResourceAccess = scopeList;

                switch (_project.PortalSettings.AppType)
                {
                    case "Web":
                        application.Web = new WebApplication
                        {
                            RedirectUris = _project.PortalSettings.RedirectUri.Split(" "),
                            ImplicitGrantSettings = new ImplicitGrantSettings { EnableIdTokenIssuance = isHybrid }
                        };
                        break;
                    case "Spa":
                        application.Spa = new SpaApplication
                        {
                            RedirectUris = _project.PortalSettings.RedirectUri.Split(" ")
                        };
                        break;
                    case "Desktop":
                        application.PublicClient = new Microsoft.Graph.PublicClientApplication
                        {
                            RedirectUris = _project.PortalSettings.RedirectUri.Split(" ")

                        };
                        break;
                    default:
                        break;
                }


                var content = await graphClient.Applications
                    .Request()
                    .AddAsync(application);

                if (!string.IsNullOrEmpty(_project.PortalSettings.BrokeredUri))
                {
                    IEnumerable<string> brokerUriList = _project.PortalSettings.BrokeredUri.Split(" ");
                    List<string> brokerUriList2 = new List<string>();
                    foreach (string brokerUri in brokerUriList)
                    {
                        brokerUriList2.Add(String.Format(brokerUri, content.AppId));
                    }
                   // _project.PortalSettings.BrokeredUri = String.Format(_project.PortalSettings.BrokeredUri, content.AppId);
                    content.PublicClient.RedirectUris = content.PublicClient.RedirectUris.Concat(brokerUriList2);

                    await graphClient.Applications[content.Id]
                            .Request()
                            .UpdateAsync(content);
                }
                string key = string.Format("persistdata:Projects[{0}]", _project.Order);
                _persistData.Add(key + ".App.AppId", content.AppId);                
                _persistData.Add(key + ".App.Id", content.Id);

                if (_project.PortalSettings.AppType.Equals("Api"))
                {
                    var listofPermission = new List<Microsoft.Graph.PermissionScope>();
                    int index = 0;
                    foreach (var scp in _project.PortalSettings.PermissionScopes)
                    {
                        var permissionscope = new Microsoft.Graph.PermissionScope();
                        permissionscope.Id = Guid.NewGuid();
                        string keyId = string.Format("{0}.PermissionScopes[{1}]", key, index);
                        _persistData.Add(keyId + ".Id", permissionscope.Id.ToString());
                        foreach (var item in scp.GetType().GetProperties())
                        {
                            permissionscope.GetType().GetProperties().FirstOrDefault(
                             o => o.Name.Equals(item.Name)).SetValue(permissionscope,
                             item.GetValue(scp, null).ToString());
                        }
                        listofPermission.Add(permissionscope);
                        index++;
                    }
                    var o = new ApiApplication();
                    o.Oauth2PermissionScopes = listofPermission;
                    content.Api = o;

                    var appIdUris = new List<string>();
                    appIdUris.Add("api://" + content.AppId);
                    content.IdentifierUris = appIdUris;

                    await graphClient.Applications[content.Id]
                   .Request()
                   .UpdateAsync(content);

                    var servicePrincipal = new ServicePrincipal
                    {
                        AppId = content.AppId,
                    };

                    await graphClient.
                        ServicePrincipals.
                        Request().AddAsync(servicePrincipal);
                }
                if (!string.IsNullOrEmpty(_project.PortalSettings.SecretName))
                {
                    var passwordCredential = new PasswordCredential
                    {
                        DisplayName = _project.PortalSettings.SecretName
                    };

                    var secretresponse = await graphClient.Applications[content.Id]
                        .AddPassword(passwordCredential)
                        .Request()
                        .PostAsync();

                    _persistData.Add(key + ".SecretText", secretresponse.SecretText);
                }
                if (_project.PortalSettings.Certificate!=null)
                {
                    
                    _project.PortalSettings.Certificate.WorkingFolder = _project.AbsoluteProjectPath;
                    CertificateWindow certificateWindow = new CertificateWindow(_project.PortalSettings.Certificate);
                    if (certificateWindow.ShowDialog() == true)
                    {
                        var keyCredential = new KeyCredential
                        {
                            //DisplayName = _project.PortalSettings.Certificate.CertName,
                            Usage = "Verify",
                            Type = "AsymmetricX509Cert",
                            KeyId = Guid.NewGuid(),
                            Key= _project.PortalSettings.Certificate.Key,
                            EndDateTime= _project.PortalSettings.Certificate.EndDateTime,
                            StartDateTime= _project.PortalSettings.Certificate.StartDateTime                            
                        };

                        content.KeyCredentials = new List<KeyCredential>() { keyCredential };
                        var app = await graphClient.Applications[content.Id]
                            .Request()
                           .UpdateAsync(content);

                        _persistData.Add(key + ".Certificate.ThumbPrint",Encoding.Default.GetString(_project.PortalSettings.Certificate.ThumbPrint));
                        _persistData.Add(key + ".Certificate.DistinguishedName", string.Format("CN={0}",_project.PortalSettings.Certificate.Subject));
                        _persistData.Add(key + ".Certificate.PemFile", _project.PortalSettings.Certificate.PemName);
                        _persistData.Add(key + ".Certificate.PfxFile", _project.PortalSettings.Certificate.PfxName);
                    }
                }
                return content;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<List<Application>> ListAppRegistrations(string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            try
            {
                var graphClient = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }));
                var content = await graphClient.Applications
                    .Request()
                    .GetAsync();
                return content?.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
