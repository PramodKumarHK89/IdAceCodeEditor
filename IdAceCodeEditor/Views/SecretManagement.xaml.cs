using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IdAceCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for SecretManagement.xaml
    /// </summary>
    public partial class SecretManagement : Window
    {
        public string mode;
        public string secretText;
        public SecretManagement()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mode = "manual";
            secretText = txtSec.Text;
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mode = "auto";
            this.DialogResult = true;
        }
    }
}
