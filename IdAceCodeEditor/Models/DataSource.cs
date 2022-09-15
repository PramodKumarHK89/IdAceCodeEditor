using IdAceCodeEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdAceCodeEditor
{
    public class DataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ObservableCollection<Framework> Frameworks{ get; set; }

        Framework _SelectedFramework;
        public Framework SelectedFramework
        {
            get { return _SelectedFramework; }
            set
            {
                _SelectedFramework = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Sample> Samples { get; set; }

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
