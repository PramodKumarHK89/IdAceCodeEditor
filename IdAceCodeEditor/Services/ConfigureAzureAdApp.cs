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
                        scopeResource.ResourceAppId=value;
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
                    scopeResource.ResourceAccess= raList;
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

                string key = string.Format("persistdata:Projects[{0}]", _project.Order);
                _persistData.Add(key + ".App.AppId", content.AppId);

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
                return content;
            }
            catch (Exception ex)
            {
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
