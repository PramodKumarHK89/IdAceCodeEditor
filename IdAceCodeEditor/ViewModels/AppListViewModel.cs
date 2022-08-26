using IdAceCodeEditor.Views;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Security.Credentials;
using Application = Microsoft.Graph.Application;

namespace IdAceCodeEditor
{
    public class AppListViewModel : INotifyPropertyChanged
    {
        string[] CREATESCOPES = new string[] { "Application.ReadWrite.All" };

        public Project _project;
        Dictionary<string, string> _persistData;
        ConfigureAzureAdApp _ConfigureAzureAdApp;
        public AppListViewModel( Project project, ConfigureAzureAdApp configureObj,Dictionary<string, string> persistData)
        {
            _ConfigureAzureAdApp = configureObj;
            _project = project;
            Name = project.Name;
            _persistData = persistData;
        }
        public string Name{ get; set; }
        private ObservableCollection<Application> _Applications;
        public ObservableCollection<Application> Applications {
            get { return _Applications; }
            set
            {
                _Applications = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public Application SelectedApp { get; set; }

        private ICommand _createAppCommand;

        public ICommand CreateAppCommand
        {
            get
            {
                if (_createAppCommand == null)
                {
                    _createAppCommand = new RelayCommand(
                        param => this.CreateApp(param),
                        param => this.CanCreateApp()
                    );
                }
                return _createAppCommand;
            }
        }
        private bool CanCreateApp()
        {
            return true;
            // Verify command can be executed here
        }
        private async void CreateApp(object o)
        {
            var window = (AppList)o;
            AutomaticAppCreationWindow objWindow = new AutomaticAppCreationWindow(_project);
            objWindow.Owner = window;
            if (objWindow.ShowDialog() == true)
            {
                var authResult = await _ConfigureAzureAdApp.AuthenticateWithAzureAd(CREATESCOPES, window);
                var app = await _ConfigureAzureAdApp.CreateAppRegistration();

                await _ConfigureAzureAdApp.PostRegupdates();
                window.app = app;

                Mapping(_project.ReplacementFields);

                ReplaceService objService = new ReplaceService();
                objService.ReplaceManualSettings(_project.AbsoluteProjectPath, _project.ReplacementFields);

                window.DialogResult = true;
                window.Close();
            }

        }
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        param => this.Cancel(param),
                        param => this.CanCancel()
                    );
                }
                return _cancelCommand;
            }
        }

        private bool CanCancel()
        {
            return true;
            // Verify command can be executed here
        }

        private void Cancel(object o)
        {
            var window = (Window)o;
            window.Close();

        }
        private ICommand _configureCommand;
        

        public ICommand ConfigureCommand
        {
            get
            {
                if (_configureCommand == null)
                {
                    _configureCommand = new RelayCommand(
                        param => this.Configure(param),
                        param => this.CanConfigure()
                    );
                }
                return _configureCommand;
            }
        }

        private bool CanConfigure()
        {
            if (SelectedApp == null)
                return false;
            else
                return true;
        }

        private async void Configure(object o)
        {
            var window = (AppList)o;
            var bRet = await CheckExistingAppRegistrationproperties(window);

            if (bRet)
            {
                window.app = SelectedApp;
                
                UpdatePersistDataValues();
                Mapping(_project.ReplacementFields);

                ReplaceService objService = new ReplaceService();
                objService.ReplaceManualSettings(_project.AbsoluteProjectPath, _project.ReplacementFields);

                window.DialogResult = true;
                window.Close();
            }
            return;
        }

        private bool UpdatePersistDataValues()
        {
            string key = string.Format("persistdata:Projects[{0}]", _project.Order);
            _persistData.Add(key + ".App.AppId", SelectedApp.AppId);
            _persistData.Add(key + ".App.Id", SelectedApp.Id);

            if (_project.PortalSettings.AppType.Equals("Api"))
            {
                int index = 0;
                foreach (var item in SelectedApp.Api.Oauth2PermissionScopes)
                {
                    string keyId = string.Format("{0}.PermissionScopes[{1}]", key, index);
                    _persistData.Add(keyId + ".Id", item.Id.ToString());
                    index++;
                }
            }
            return true;
        }
        bool IsRedirectURIPresent(IEnumerable<string> appRegUri, IEnumerable<string> configUri, string appId="")
        {

            foreach (var item in configUri)
            {
                var value = item;
                if (!string.IsNullOrEmpty(appId))
                {
                    value  = String.Format(item, appId);
                }
                if (!appRegUri.Contains(value)) return false;
            }
            return true;
        }

            private async Task<bool> CheckExistingAppRegistrationproperties(Window window)
        {
            bool bRet = false;

            bRet = SelectedApp.SignInAudience.Equals(_project.PortalSettings.SignInAudience);
            if (!bRet)
            {
                MessageBox.Show(string.Format("SignInAudience is not matching. Please update {0} in the app registration", _project.PortalSettings.SignInAudience));
                return bRet;
            }
            bRet = false;
            switch (_project.PortalSettings.AppType)
            {
                case "Web":
                    bRet = IsRedirectURIPresent(SelectedApp.Web.RedirectUris, _project.PortalSettings.RedirectUri.Split(" "));
                    break;
                case "Spa":
                    bRet = IsRedirectURIPresent(SelectedApp.Spa.RedirectUris, _project.PortalSettings.RedirectUri.Split(" "));
                    break;
                case "Desktop":
                    bRet = IsRedirectURIPresent(SelectedApp.PublicClient.RedirectUris, _project.PortalSettings.RedirectUri.Split(" "));
                    break;
                default:
                    bRet = true;
                    break;
            }         
            if (!bRet)
            {
                MessageBox.Show(String.Format("Redirect URI {0} must be configured under the {1} platform.", _project.PortalSettings.RedirectUri, _project.PortalSettings.AppType));
                return bRet;
            }

            bRet = string.IsNullOrEmpty(_project.PortalSettings.BrokeredUri);
            if (!bRet)
            {
                bRet = IsRedirectURIPresent(SelectedApp.PublicClient.RedirectUris, _project.PortalSettings.BrokeredUri.Split(" "), SelectedApp.AppId); 
            }
            if (!bRet)
            {
                MessageBox.Show(String.Format("Redirect URI {0} must be configured under the {1} platform.", _project.PortalSettings.BrokeredUri, _project.PortalSettings.AppType));
                return false;
            }

            bRet = !_project.PortalSettings.IsHybridFlow.Equals("true", StringComparison.InvariantCultureIgnoreCase);
            if (!bRet)
            {
                bRet = (bool)SelectedApp.Web.ImplicitGrantSettings?.EnableAccessTokenIssuance;
            }
            if (!bRet)
            {
                MessageBox.Show("ID token must be enabled under the web platform to enable hybrid flow.");
                return false;
            }
            //some work pending
            bRet = _project.PortalSettings.Certificate == null;
            if (!bRet)
            {
                bRet = SelectedApp.KeyCredentials == null;
                if (!bRet)
                {
                    ListCerts obj = new ListCerts(SelectedApp.KeyCredentials.ToList());
                    if (obj.ShowDialog() == true)
                    {
                        if (_project.PortalSettings.Certificate.Type.Equals("StoreWithThumbprint") ||
                            _project.PortalSettings.Certificate.Type.Equals("StoreWithDN"))
                        {
                            CertService certService = new CertService();
                            string dn;
                            bRet = certService.FindCertByThumprint(obj.ThumbPrint,out dn);
                            if (!bRet)
                            {
                                MessageBox.Show("Unable to find a certificate {0} in store. you may want install the certificate for the sample to work on this machine.");
                            }
                            else
                            {
                                string key = string.Format("persistdata:Projects[{0}]", _project.Order);
                                _persistData.Add(key + ".Certificate.ThumbPrint", obj.ThumbPrint);
                                _persistData.Add(key + ".Certificate.DistinguishedName", string.Format("CN={0}", dn));
                            }
                        }
                        else
                        {
                            ExistingCertWindow obExsiting = new ExistingCertWindow(true, !_project.PortalSettings.Certificate.Type.Equals("PemFile"),
                                !_project.PortalSettings.Certificate.Type.Equals("PfxFile"));
                            if (obExsiting.ShowDialog() == true)
                            {
                                string key = string.Format("persistdata:Projects[{0}]", _project.Order);
                                _persistData.Add(key + ".Certificate.ThumbPrint", obj.ThumbPrint);
                                _persistData.Add(key + ".Certificate.PemFile", obExsiting.PemFileName);
                                _persistData.Add(key + ".Certificate.PfxFile", obExsiting.PfxFileName);
                                bRet = true;
                            }
                            else
                            {
                                bRet = false;
                            }

                        }
                        
                    }
                    else
                    {
                        bRet = false;
                    }                   
                }
            }
            if (!bRet)
            {
                MessageBox.Show("Something wrong with certificate.");
                return false;
            }

            bRet = string.IsNullOrEmpty(_project.PortalSettings.SecretName);
            if (!bRet)
            {
            back: SecretManagement obj = new SecretManagement();
                if (obj.ShowDialog() == true)
                {
                    string secretText;
                    if (obj.mode.Equals("manual"))
                    {
                        secretText = obj.secretText;
                    }
                    else
                    {
                        var authResult = await _ConfigureAzureAdApp.AuthenticateWithAzureAd(CREATESCOPES, window);
                        var respone = await _ConfigureAzureAdApp.CreateSecret(SelectedApp.Id);
                        if (respone == null)
                        {
                            MessageBox.Show("Unable to create secret. Please update the secret value Manually");
                            goto back;
                        }
                        else
                            secretText = respone.SecretText;
                    }
                    string key1 = string.Format("persistdata:Projects[{0}]", _project.Order);
                    _persistData.Add(key1 + ".SecretText", secretText);
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }
            }
            if (!bRet)
            {
                MessageBox.Show("Please get the secret from Azure portal and update it manually");
                return bRet;
            }
            bRet = false;
            foreach (var item in _project.PortalSettings.RequiredResourceAccesss)
            {
                string resourceAppId = "";
                if (item.ResourceAppId.StartsWith("persistdata:"))
                {
                    string value;
                    _persistData.TryGetValue(item.ResourceAppId, out value);
                    resourceAppId = value;
                }
                else
                {
                    resourceAppId = item.ResourceAppId;
                }
                var rr = SelectedApp.RequiredResourceAccess.FirstOrDefault(r => r.ResourceAppId.Equals(resourceAppId));
                if (rr == null)
                {
                    bRet = false;
                    break;
                }

                bRet = true;
                foreach (var ra in item.ResourceAccesss)
                {
                    string id = "";
                    if (ra.Id.StartsWith("persistdata:"))
                    {
                        string value;
                        _persistData.TryGetValue(ra.Id, out value);
                        id = value;
                    }
                    else
                        id = ra.Id;
                   
                    var selectedrr = rr.ResourceAccess.FirstOrDefault(r => r.Id.ToString().Equals(id) && r.Type.Equals(ra.Type));
                    if (rr == null)
                    {
                        bRet = false;
                        break;
                    }
                }
            }
            if (!bRet)
            {
                MessageBox.Show("One of the scopes/API permission is missing in the app registration which is needed by app");
                return bRet;
            }
            bRet = !_project.PortalSettings.AppType.Equals("Api");
            if (!bRet)
            {
                foreach (var scp in _project.PortalSettings.PermissionScopes)
                {
                    bRet = SelectedApp.Api.Oauth2PermissionScopes.Any(api => api.Value.Equals(scp.Value));
                    if (!bRet)
                    {
                        MessageBox.Show(String.Format("App registration is not exposing an API {0}", scp.Value));
                        return bRet;
                    }
                }
            }
            return bRet;
        }
        private void Mapping(List<ReplacementField> replacementFields)
        {
            foreach (var item in replacementFields)
            {
                if (item.Source.StartsWith("persistdata:"))
                {
                    string value;
                    _persistData.TryGetValue(item.Source, out value);
                    if(!string.IsNullOrEmpty(item.Format))
                        item.Value = string.Format(item.Format, value);                   
                    else
                        item.Value = value;
                }                  
                else
                {
                    item.Value = item.Source;
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
