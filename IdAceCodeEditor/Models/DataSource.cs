using IdAceCodeEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdAceCodeEditor
{
    public class DataSource
    {
            public ObservableCollection<Framework> Frameworks{ get; set; }

        private ICommand _guideMeCommand;

        public ICommand GuideMeCommand
        {
            get
            {
                if (_guideMeCommand == null)
                {
                    _guideMeCommand = new RelayCommand(
                        param => this.GuideMe(param),
                        param => this.CanGuideMe()
                    );
                }
                return _guideMeCommand;
            }
        }

        private bool CanGuideMe()
        {
            return true;
            // Verify command can be executed here
        }

        private void GuideMe(object o)
        {
            GuideMeWindow guideMe = new GuideMeWindow(this);
            guideMe.Owner = (Window)o;
            guideMe.ShowDialog();

        }
    }
}
