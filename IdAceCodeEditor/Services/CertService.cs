using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Pkcs;

namespace IdAceCodeEditor
{
    public class CertService
    {
        public X509Certificate2 GenerateCertificate(string workingfolder, string subject, string issuer, string certName, string pfxName, string pemName,
            string password, bool isAddtoStore)
        {
            var keyPair = GetKeyPair();
            var clientCert = GenerateSelfSignedCertificate(string.Format("CN={0}", subject),
                string.Format("CN={0}", issuer),
                password,
                keyPair);

            if (!string.IsNullOrEmpty(certName))
            {
                var crtFile = clientCert.Export(X509ContentType.Cert);
                File.WriteAllBytes(Path.Combine(workingfolder, certName), crtFile);
            }

            if (!string.IsNullOrEmpty(pemName))
            {
                TextWriter textWriter = new StringWriter();
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();
                File.WriteAllText(Path.Combine(workingfolder, pemName), textWriter.ToString());
            }
            if (!string.IsNullOrEmpty(pfxName))
            {
                var pfxFile = clientCert.Export(X509ContentType.Pfx, password);
                File.WriteAllBytes(Path.Combine(workingfolder, pfxName), pfxFile);
            }
            if(isAddtoStore)
            {
                addCertToStore(clientCert, StoreName.My, StoreLocation.CurrentUser);
            }
            // clientCert.
            return clientCert;
        }

        private AsymmetricCipherKeyPair GetKeyPair()
        {
            const int keyStrength = 2048;
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            AsymmetricCipherKeyPair subjectKeyPair;
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random,
                keyStrength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            return subjectKeyPair;
        }

        public bool FindCertByThumprint(string thumbPrint,out string subject)
        {
            subject = "";
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, thumbPrint, true);
            if (fcollection != null && fcollection.Count >= 1)
            {
                foreach (X509Certificate2 x509 in fcollection)
                {
                    subject = x509.SubjectName.ToString();
                }
                return true;
            }
            else return false;
        }
        public bool FindCertByDistinguishedName(string subject)
        {
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindBySubjectDistinguishedName, subject, true);
            if (fcollection != null && fcollection.Count >= 1) return true;
            else return false;
        }
        private bool addCertToStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            bool bRet = false;

            try
            {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

                store.Close();
            }
            catch
            {

            }

            return bRet;

        }
        private X509Certificate2 GenerateSelfSignedCertificate(string subjectName,
            string issuerName,string password,
               AsymmetricCipherKeyPair assymtericKey)
        {
            //const int keyStrength = 2048;

            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            Org.BouncyCastle.Math.BigInteger serialNumber = BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One,
               Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            X509Name subjectDN = new X509Name(subjectName);
            X509Name issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            DateTime notBefore = DateTime.UtcNow.Date.AddHours(-1);
            DateTime notAfter = notBefore.AddYears(1);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            certificateGenerator.SetPublicKey(assymtericKey.Public);

            // Selfsign certificate
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", assymtericKey.Private, random);
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

            // Merge into X509Certificate2
            var x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded(),
                password,
                X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            var pkcs12Store = new Pkcs12Store();
            var certEntry = new X509CertificateEntry(certificate);
            pkcs12Store.SetCertificateEntry(subjectName, certEntry);
            pkcs12Store.SetKeyEntry(subjectName, new AsymmetricKeyEntry(assymtericKey.Private),
                new[] { certEntry });
            X509Certificate2 keyedCert;
            using (MemoryStream pfxStream = new MemoryStream())
            {
                pkcs12Store.Save(pfxStream, null, new SecureRandom());
                pfxStream.Seek(0, SeekOrigin.Begin);
                keyedCert = new X509Certificate2(pfxStream.ToArray(), password,
                     X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }
            return keyedCert;
        }
    }
}
