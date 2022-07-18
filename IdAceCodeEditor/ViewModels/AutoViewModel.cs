using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdAceCodeEditor.ViewModels
{
    public class AutoViewModel
    {
        public Project SampleProject{ get; set; }  
        public PortalSetting PortalValue { get; set; }

        public ObservableCollection<AppSettingList> AppListSettings{ get; set; }
        public AutoViewModel(Project sampleProject)
        {
            SampleProject = sampleProject;
            AppListSettings = new ObservableCollection<AppSettingList>();

            foreach (var item in SampleProject.PortalSettings.GetType().GetProperties())
            {
                if (item.PropertyType.Name.Equals("String"))
                {
                    if(item.GetValue(SampleProject.PortalSettings, null)!=null)
                        AppListSettings.Add(new AppSettingList(item.Name, item.GetValue(SampleProject.PortalSettings, null).ToString()));
                }
            }
        }
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
        private void CreateApp(object o)
        {

            foreach (var item in AppListSettings)
            {
                SampleProject.PortalSettings.GetType().GetProperties().FirstOrDefault(
                    o => o.Name.Equals(item.Name)).SetValue(SampleProject.PortalSettings, item.Value);                
            }
            var window = (Window)o;
            window.DialogResult = true;
            window.Close();

        }
    }

    public class AppSettingList
    {
        public AppSettingList(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public String Name { get; set; }
        public String Value { get; set; }
    }
}
