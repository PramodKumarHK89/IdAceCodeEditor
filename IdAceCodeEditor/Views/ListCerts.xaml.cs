using Microsoft.Graph;
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
    /// Interaction logic for ListCerts.xaml
    /// </summary>
    public partial class ListCerts : Window
    {
        public ListCerts()
        {
            InitializeComponent();
        }
        public string ThumbPrint{ get; set; }

        public ListCerts(List<KeyCredential> KeyCredentials)
        {
            InitializeComponent();
            ListCertViewModel objviewModel = new ListCertViewModel();
            objviewModel.KeyCredentials = KeyCredentials;
            this.DataContext = objviewModel;            
        }
    }
}
