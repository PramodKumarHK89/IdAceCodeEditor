using IdAceCodeEditor.Views;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Application = Microsoft.Graph.Application;

namespace IdAceCodeEditor
{
    public class AppListViewModel
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
        public List<Application> Applications { get; set; }

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
            if (objWindow.ShowDialog() == true)
            {
                var authResult = await _ConfigureAzureAdApp.AuthenticateWithAzureAd(CREATESCOPES, window);
                var app = await _ConfigureAzureAdApp.CreateAppRegistration();

                window.app = app;

                Mapping(_project.ReplacementFields);

                ReplaceService objService = new ReplaceService();
                objService.ReplaceManualSettings(_project.ProjectPath, _project.ReplacementFields);

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
            window.app = SelectedApp;

            if (!string.IsNullOrEmpty(_project.PortalSettings.SecretName))
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
                    string key1= string.Format("persistdata:Projects[{0}]", _project.Order);
                    _persistData.Add(key1 + ".SecretText", secretText);
                }
                else
                {
                    return;
                }

            }
            string key = string.Format("persistdata:Projects[{0}]", _project.Order);
            _persistData.Add(key + ".App.AppId", SelectedApp.AppId);

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

           
          
          

                Mapping(_project.ReplacementFields);

                ReplaceService objService = new ReplaceService();
                objService.ReplaceManualSettings(_project.ProjectPath, _project.ReplacementFields);

                window.DialogResult = true;
                window.Close();
          
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
    }
}
