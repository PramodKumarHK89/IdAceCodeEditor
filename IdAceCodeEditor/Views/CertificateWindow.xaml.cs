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
    /// Interaction logic for CertificateWindow.xaml
    /// </summary>
    public partial class CertificateWindow : Window
    {
        public CertificateWindow()
        {
            InitializeComponent();
        }
        public CertificateWindow(Certificate cert)
        {
            InitializeComponent();
            this.DataContext = cert;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cert = (Certificate)this.DataContext;
            
            ExistingCertWindow obExsiting = new ExistingCertWindow(false,
                !cert.Type.Equals("PemFile"),
                !cert.Type.Equals("PfxFile"));
            obExsiting.Owner = this;
            if (obExsiting.ShowDialog() == true)
            {
                cert.CertName = obExsiting.CertFileName;
                cert.PemName = obExsiting.PemFileName;
                cert.PfxName= obExsiting.PfxFileName;
                cert.Password = obExsiting.Password;
                cert.UpdateCertCommand.Execute(this);                
            }
        }
    }
}

