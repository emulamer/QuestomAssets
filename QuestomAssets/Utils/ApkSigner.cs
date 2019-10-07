using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.X509.Store;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Cms;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Utilities.IO.Pem;
using OpenSsl = Org.BouncyCastle.OpenSsl;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.Utils
{
    public class ApkSigner 
    {
        private SHA1 _sha = SHA1Managed.Create();
        private UTF8Encoding _encoding = new UTF8Encoding(false);
        private string _pemData;

        /// <summary>
        /// Returns the PEM data used to sign the APK
        /// </summary>
        public string PemData
        {
            get
            {
                return _pemData;
            }
        }


        /// <summary>
        /// Creates a new instance of ApkSigner
        /// </summary>
        /// <param name="pemCertificateData">The PEM data of the certificate to use with both public and private keys.  If null, a new, automatically created certificate will be used.</param>
        public ApkSigner(string pemCertificateData = null)
        {
            if (pemCertificateData != null)
            {
                _pemData = pemCertificateData;
                //test that the certificate loads before going further
                AsymmetricKeyParameter pk;
                LoadCert(_pemData, out pk);
            }
            else
            {
                _pemData = GenerateNewCertificatePEM();
            }
        }

        public void Sign(IFileProvider fileProvider)
        {
            MemoryStream msManifestFile = new MemoryStream();
            MemoryStream msSigFile = new MemoryStream();
            byte[] keyBlock;
            MemoryStream msSignatureFileBody = new MemoryStream();
            try
            {
                //create the MF file header
                using (StreamWriter swManifest = GetSW(msManifestFile))
                {
                    swManifest.WriteLine("Manifest-Version: 1.0");
                    swManifest.WriteLine("Created-By: emulamer");
                    swManifest.WriteLine();
                }

                //so that we can do it in one pass, write the MF and SF line items at the same time to their respective streams
                foreach (var infFile in fileProvider.FindFiles("*").Where(x => !x.StartsWith("META-INF")))
                {
                    WriteEntryHashes(fileProvider, infFile, msManifestFile, msSignatureFileBody);
                }

                //compute the hash on the entirety of the manifest file for the SF file
                msManifestFile.Seek(0, SeekOrigin.Begin);
                var manifestFileHash = _sha.ComputeHash(msManifestFile);

                //write the SF to memory then copy it out to the actual file- contents will be needed later to use for signing, don't want to hit the zip stream twice

                byte[] sigFileBytes = null;

                using (StreamWriter swSignatureFile = GetSW(msSigFile))
                {
                    swSignatureFile.WriteLine("Signature-Version: 1.0");
                    swSignatureFile.WriteLine($"SHA1-Digest-Manifest: {Convert.ToBase64String(manifestFileHash)}");
                    swSignatureFile.WriteLine("Created-By: emulamer");
                    swSignatureFile.WriteLine();
                }
                msSignatureFileBody.Seek(0, SeekOrigin.Begin);
                msSignatureFileBody.CopyTo(msSigFile);
                msSigFile.Seek(0, SeekOrigin.Begin);
                sigFileBytes = msSigFile.ToArray();

                //get the key block (all the hassle distilled into one line), then write it out to the RSA file
                keyBlock = SignIt(sigFileBytes);

                //delete all the META-INF stuff that exists already
                fileProvider.DeleteFiles("META-INF*");

                //write the 3 files                
                msManifestFile.Seek(0, SeekOrigin.Begin);

                fileProvider.Write("META-INF/MANIFEST.MF", msManifestFile.ToArray(), true, true);

                fileProvider.Write("META-INF/BS.SF", sigFileBytes, true, true);

                fileProvider.Write("META-INF/BS.RSA", keyBlock, true, true);
                fileProvider.Save();
            }
            finally
            {
                if (msManifestFile != null)
                    msManifestFile.Dispose();
                if (msSignatureFileBody != null)
                    msSignatureFileBody.Dispose();
                if (msManifestFile != null)
                    msManifestFile.Dispose();
                if (msSigFile != null)
                    msSigFile.Dispose();
            }

        }        

        /// <summary>
        /// Writes the MANIFEST.MF name and hash and the sigfile.SF hash for the sourceFile
        /// </summary>
        private void WriteEntryHashes(IFileProvider provider, string sourceFile, Stream manifestFileStream, Stream signatureFileStream)
        {

            using (Stream s = provider.GetReadStream(sourceFile))
            {
                var hash = _sha.ComputeHash(s);
                using (MemoryStream msSection = new MemoryStream())
                {
                    string hashOfMFSection = null;
                    using (StreamWriter swSection = GetSW(msSection))
                    {
                        swSection.WriteLine($"Name: {sourceFile}");
                        swSection.WriteLine($"SHA1-Digest: {Convert.ToBase64String(hash)}");
                        swSection.WriteLine("");

                    }
                    msSection.Seek(0, SeekOrigin.Begin);
                    hashOfMFSection = Convert.ToBase64String(_sha.ComputeHash(msSection));
                    msSection.Seek(0, SeekOrigin.Begin);
                    var actualString = UTF8Encoding.UTF8.GetString(msSection.ToArray());
                    using (var swSFFile = GetSW(signatureFileStream))
                    {
                        swSFFile.WriteLine($"Name: {sourceFile}");
                        swSFFile.WriteLine($"SHA1-Digest: {hashOfMFSection}");
                        swSFFile.WriteLine();
                    }

                    msSection.Seek(0, SeekOrigin.Begin);
                    msSection.CopyTo(manifestFileStream);
                }
            }
        }

        /// <summary>
        /// Creates a new X509 certificate and returns its data in PEM format
        /// </summary>
        public string GenerateNewCertificatePEM()
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var certificateGenerator = new X509V3CertificateGenerator();
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSignatureAlgorithm("SHA256WithRSA");
            var subjectDN = new X509Name("cn=Unknown");
            var issuerDN = subjectDN;
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Date.AddYears(-10));
            certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(50));
            var keyGenerationParameters = new KeyGenerationParameters(random, 2048);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            X509Certificate cert = certificateGenerator.Generate(subjectKeyPair.Private);

            using (var writer = new StringWriter())
            {
                var pemWriter = new OpenSsl.PemWriter(writer);

                pemWriter.WriteObject(new PemObject("CERTIFICATE", cert.GetEncoded()));
                pemWriter.WriteObject(subjectKeyPair.Private);
                return writer.ToString();
            }
        }

        private static X509Certificate LoadCert(string pemData, out AsymmetricKeyParameter privateKey)
        {
            X509Certificate cert = null;
            privateKey = null;
            using (var reader = new StringReader(pemData))
            {
                var pemReader = new OpenSsl.PemReader(reader);
                object pemObject = null;
                while ((pemObject = pemReader.ReadObject()) != null)
                {
                    if (pemObject is X509Certificate)
                    {
                        cert = pemObject as X509Certificate;
                    }
                    else if (pemObject is AsymmetricCipherKeyPair)
                    {
                        privateKey = (pemObject as AsymmetricCipherKeyPair).Private;
                    }
                }
            }
            if (cert == null)
                throw new System.Security.SecurityException("Certificate could not be loaded from PEM data.");

            if (privateKey == null)
                throw new System.Security.SecurityException("Private Key could not be loaded from PEM data.");

            return cert;
        }
        /// <summary>
        /// Get a signature block that java will load a JAR with
        /// </summary>
        /// <param name="sfFileData">The data to sign</param>
        /// <returns>The signature block (including certificate) for the data passed in</returns>
        private byte[] SignIt(byte[] sfFileData)
        {
            AsymmetricKeyParameter privateKey = null;

            var cert = LoadCert(_pemData, out privateKey);

            //create things needed to make the CmsSignedDataGenerator work
            var certStore = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(new List<X509Certificate>() { cert }));
            CmsSignedDataGenerator dataGen = new CmsSignedDataGenerator();
            dataGen.AddCertificates(certStore);
            dataGen.AddSigner(privateKey, cert, CmsSignedDataGenerator.EncryptionRsa, CmsSignedDataGenerator.DigestSha256);

            //content is detached- i.e. not included in the signature block itself
            CmsProcessableByteArray detachedContent = new CmsProcessableByteArray(sfFileData);
            var signedContent = dataGen.Generate(detachedContent, false);

            //do lots of stuff to get things in the proper ASN.1 structure for java to parse it properly.  much trial and error.
            var signerInfos = signedContent.GetSignerInfos();
            var signer = signerInfos.GetSigners().Cast<SignerInformation>().First();
            SignerInfo signerInfo = signer.ToSignerInfo();
            Asn1EncodableVector digestAlgorithmsVector = new Asn1EncodableVector();
            digestAlgorithmsVector.Add(new AlgorithmIdentifier(new DerObjectIdentifier("2.16.840.1.101.3.4.2.1"), DerNull.Instance));
            ContentInfo encapContentInfo = new ContentInfo(new DerObjectIdentifier("1.2.840.113549.1.7.1"), null);
            Asn1EncodableVector asnVector = new Asn1EncodableVector();
            asnVector.Add(X509CertificateStructure.GetInstance(Asn1Object.FromByteArray(cert.GetEncoded())));
            Asn1EncodableVector signersVector = new Asn1EncodableVector();
            signersVector.Add(signerInfo.ToAsn1Object());
            SignedData signedData = new SignedData(new DerSet(digestAlgorithmsVector), encapContentInfo, new BerSet(asnVector), null, new DerSet(signersVector));
            ContentInfo contentInfo = new ContentInfo(new DerObjectIdentifier("1.2.840.113549.1.7.2"), signedData);
            return contentInfo.GetDerEncoded();
        }

        private StreamWriter GetSW(Stream stream)
        {
            return new StreamWriter(stream, _encoding, 1024, true);
        }

     
    }
}
