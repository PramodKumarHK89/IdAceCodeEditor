using IdAceCodeEditor.Views;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace IdAceCodeEditor
{

    public class Base64CustomConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToBase64String((byte[])value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
    public class ListCertViewModel
    {
        public List<KeyCredential> KeyCredentials { get; set; }

        public KeyCredential SelectedKeyCredential { get; set; }

        
        private ICommand _updateCertCommand;

        public ICommand UpdateKeyCredentialCommand
        {
            
            get
            {
                
                if (_updateCertCommand == null)
                {
                    _updateCertCommand = new RelayCommand(
                        param => this.UpdateKeyCred(param),
                        param => this.CanUpdateCert()
                    );
                }
                return _updateCertCommand;
            }
        }
        private bool CanUpdateCert()
        {
            return SelectedKeyCredential != null;
            // Verify command can be executed here
        }
        private void UpdateKeyCred(object o)
        {
            var window = (ListCerts)o;
            window.ThumbPrint = Convert.ToBase64String(SelectedKeyCredential.CustomKeyIdentifier);
            window.DialogResult = true;
            window.Close();
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
            window.DialogResult = false;
            window.Close();

        }
    }
}
