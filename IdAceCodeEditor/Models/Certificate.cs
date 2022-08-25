using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdAceCodeEditor
{
    public class Certificate
    {
        public string WorkingFolder { get; set; }
        public string Type { get; set; }
        public string CertName { get; set; }
        public string PemName { get; set; }
        public string PfxName { get; set; }
        public string Password { get; set; }
        public bool IsAddToStore{ get; set; }
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public byte[] Key{ get; set; }
        public byte[] ThumbPrint { get; set; }

        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }

        private ICommand _createCertCommand;

        public ICommand CreateCertCommand
        {
            get
            {
                if (_createCertCommand == null)
                {
                    _createCertCommand = new RelayCommand(
                        param => this.CreateCert(param),
                        param => this.CanCreateCert()
                    );
                }
                return _createCertCommand;
            }
        }

        private ICommand _updateCertCommand;

        public ICommand UpdateCertCommand
        {
            get
            {
                if (_updateCertCommand == null)
                {
                    _updateCertCommand = new RelayCommand(
                        param => this.UpdateCert(param),
                        param => this.CanCreateCert()
                    );
                }
                return _updateCertCommand;
            }
        }
        private bool CanCreateCert()
        {
            return true;
            // Verify command can be executed here
        }
        private void UpdateCert(object o)
        {
            var window = (Window)o;
            if (File.Exists(CertName))
            {
                var certX509 = new X509Certificate2(File.ReadAllBytes(CertName));
                StartDateTime = certX509.NotBefore;
                EndDateTime = certX509.NotAfter;
                Key = certX509.GetRawCertData();
                ThumbPrint = Encoding.Default.GetBytes(certX509.Thumbprint);
            }
            else if(File.Exists(PfxName))
            {
                var certX509 = new X509Certificate2(File.ReadAllBytes(PfxName),Password);
                StartDateTime = certX509.NotBefore;
                EndDateTime = certX509.NotAfter;
                Key = certX509.GetRawCertData();
                ThumbPrint = Encoding.Default.GetBytes(certX509.Thumbprint);
            }            
            window.DialogResult = true;
            window.Close();
        }
        private void CreateCert(object o)
        {
            CertService certService = new CertService();
            var certX509 = certService.GenerateCertificate(WorkingFolder, Subject,
                Issuer, CertName,
                PfxName, PemName,
                Password, IsAddToStore);

            StartDateTime = certX509.NotBefore;
            EndDateTime = certX509.NotAfter;
            Key = certX509.GetRawCertData();
            ThumbPrint = Encoding.Default.GetBytes(certX509.Thumbprint);
            var window = (Window)o;
            window.DialogResult = true;
            window.Close();
        }
    }
}
