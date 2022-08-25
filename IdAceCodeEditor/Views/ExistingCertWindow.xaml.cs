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
    /// Interaction logic for ExistingCertWindow.xaml
    /// </summary>
    public partial class ExistingCertWindow : Window
    {
        public ExistingCertWindow()
        {
            InitializeComponent();
        }

        public string CertFileName { get; set; }
        public string PemFileName { get; set; }
        public string PfxFileName { get; set; }
        public string Password { get; set; }
        public ExistingCertWindow(bool isHideCRT = false, bool isHidePem = false, bool isHidePfx = false)
        {
            InitializeComponent();
            this.DataContext = this;
            if (isHideCRT)
            {
                txtCrt.Visibility = Visibility.Collapsed;
                crtLabel.Visibility = Visibility.Collapsed;
                crtButton.Visibility = Visibility.Collapsed;
            }
            if (isHidePem)
            {
                txtPem.Visibility = Visibility.Collapsed;
                pemLabel.Visibility = Visibility.Collapsed;
                pemButton.Visibility = Visibility.Collapsed;
            }
            if (isHidePfx)
            {
                txtPfx.Visibility = Visibility.Collapsed;
                pfxLabel.Visibility = Visibility.Collapsed;
                pfxButton.Visibility = Visibility.Collapsed;
                passLabel.Visibility = Visibility.Collapsed;
                txtpassword.Visibility = Visibility.Collapsed;  
            }
        }

        private void CRT_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".crt"; // Default file extension
            dialog.Filter = "Crt files (.crt)|*.crt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                CertFileName = dialog.FileName;
                txtCrt.Text = CertFileName;
            }
        }

        private void Pem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".pem"; // Default file extension
            dialog.Filter = "pem files (.pem)|*.pem"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                PemFileName = dialog.FileName;
                txtPem.Text = PemFileName;
            }
        }

        private void Pfx_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".pfx"; // Default file extension
            dialog.Filter = "pfx files (.pfx)|*.pfx"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                PfxFileName = dialog.FileName;
                txtPfx.Text = PfxFileName;
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Password = txtpassword.Password;
            PfxFileName = txtPfx.Text;
            PemFileName = txtPem.Text ;
            CertFileName = txtCrt.Text ;
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
