using IdAceCodeEditor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdAceCodeEditor
{
    public class Project
    {
        public string Name { get; set; }        
        public string ProjectPath { get; set; }
        
        public string AbsoluteProjectPath { get; set; }
        public int Order { get; set; }

        public PortalSetting PortalSettings { get; set; }

        public List<ReplacementField> ReplacementFields { get; set; }

        public List<PostRegUpdate> PostRegUpdates { get; set; }       

        private ICommand _replaceSettingsCommand;

        public ICommand ReplaceSettingsCommand
        {
            get
            {
                if (_replaceSettingsCommand == null)
                {
                    _replaceSettingsCommand = new RelayCommand(
                        param => this.ReplaceSettings(param),
                        param => this.CanReplaceSettings()
                    );
                }
                return _replaceSettingsCommand;
            }
        }

        private bool CanReplaceSettings()
        {
            return true;
            // Verify command can be executed here
        }

        private void ReplaceSettings(object o)
        {            
            ReplaceService objService = new ReplaceService();
            objService.ReplaceManualSettings(AbsoluteProjectPath, this.ReplacementFields.ToList());

            var window = (Window)o;
            window.DialogResult = true;
            window.Close();

        }


    }
}
