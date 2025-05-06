﻿using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Extensions;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Models.Consumer.ETA;
using MediatR;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI40;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ISession = Net.Pkcs11Interop.HighLevelAPI.ISession;

namespace ETA.Integrator.Server.Services
{
    public class SignatureService : ISignatureService
    {
        private readonly ILogger<SignatureService> _logger;
        public SignatureService(ILogger<SignatureService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void SerializeToken(JToken request, StringBuilder serialized)
        {
            {
                if (request.Parent is null)
                {
                    SerializeToken(request.First, serialized);
                }
                else
                {
                    if (request.Type == JTokenType.Property)
                    {
                        string name = ((JProperty)request).Name.ToUpper();
                        serialized.Append("\"" + name + "\"");
                        foreach (var property in request)
                        {
                            if (property.Type == JTokenType.Object)
                            {
                                SerializeToken(property, serialized);
                            }
                            if (property.Type == JTokenType.Boolean || property.Type == JTokenType.Integer || property.Type == JTokenType.Float || property.Type == JTokenType.Date)
                            {
                                serialized.Append("\"" + property.Value<string>() + "\"");
                            }
                            if (property.Type == JTokenType.String)
                            {
                                serialized.Append(JsonConvert.ToString(property.Value<string>()));
                            }
                            if (property.Type == JTokenType.Array)
                            {
                                foreach (var item in property.Children())
                                {
                                    serialized.Append("\"" + ((JProperty)request).Name.ToUpper() + "\"");
                                    SerializeToken(item, serialized);
                                }
                            }
                        }
                    }
                    if (request.Type == JTokenType.String)
                    {
                        serialized.Append(JsonConvert.ToString(request.Value<string>()));
                    }
                }

                if (request.Type == JTokenType.Object)
                {
                    foreach (var property in request.Children())
                    {
                        if (property.Type == JTokenType.Object || property.Type == JTokenType.Property)
                        {
                            SerializeToken(property, serialized);
                        }
                    }
                }
            }
        }
        private string SerializeToJson(InvoiceToSerializeDTO dto)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Ensure proper Unicode escaping
            };

            var jsonString = System.Text.Json.JsonSerializer.Serialize(dto, options);

            return jsonString;
        }
        private string GenerateCanonicalString(string serializedJson)
        {
            StringBuilder serialized = new StringBuilder();

            if (string.IsNullOrWhiteSpace(serializedJson))
            {
                _logger.LogError("SignatureService/GenerateCanonicalString: Serialized JSON data is required.");
            }

            else
            {
                var request = JsonConvert.DeserializeObject<JObject>(serializedJson, new JsonSerializerSettings
                {
                    FloatFormatHandling = FloatFormatHandling.String,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateParseHandling = DateParseHandling.None
                });

                if (request is null)
                    _logger.LogError("SignatureService/GenerateCanonicalString: Request object is null");
                else
                {
                    SerializeToken(request, serialized);
                }
            }

            return serialized.ToString();
        }
        private IPkcs11Library LoadPkcsLibrary()
        {
            try
            {
                Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
                IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, @"C:\Windows\System32\eTPKCS11.dll", AppType.MultiThreaded);
                return pkcs11Library;
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/TokenLogin: " + ex.Message);
                throw;
            }
        }
        private Unit TokenLogin(ISession session, string tokenPin)
        {
            try
            {
                CKS sessionState = session.GetSessionInfo().State;
                if (sessionState == CKS.CKS_RO_USER_FUNCTIONS || sessionState == CKS.CKS_RW_USER_FUNCTIONS)
                {
                    _logger.LogInformation("Session is already logged in.");
                    return Unit.Value;
                }
                session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(tokenPin));
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/TokenLogin: " + ex.Message);
                throw;
            }
        }
        private IObjectHandle? FindCertificate(ISession session)
        {
            try
            {
                var certificateSearchAttributes = new List<IObjectAttribute>()
        {
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
        };

                IObjectHandle certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

                if (certificate == null)
                {
                    _logger.LogError("SignatureService/FindCertificate: Certificate not found");
                    return null;
                }

                return certificate;
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/FindCertificate: " + ex.Message);
                throw;
            }
        }
        private X509Certificate2? FindCertificateInStore(string tokenCertificate)
        {
            try
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.MaxAllowed);
                var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, tokenCertificate, false);
                store.Close();

                if (foundCerts.Count == 0)
                {
                    _logger.LogError("SignatureService/FindCertificateInStore: No matching certificate found");
                    return null;
                }

                return foundCerts[0];
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/FindCertificateInStore: " + ex.Message);
                throw;
            }
        }
        private SignedCms CreateSignedCms(byte[] data, IObjectHandle certificateHandle, X509Certificate2 certificateForSigning)
        {
            try
            {
                ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);

                EssCertIDv2 bouncyCertificate = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), this.HashBytes(certificateForSigning.RawData));
                SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[] { bouncyCertificate });

                CmsSigner signer = new CmsSigner(certificateForSigning);
                signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");
                signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));

                SignedCms cms = new SignedCms(content, true);
                cms.ComputeSignature(signer);
                return cms;
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/CreateSignedCms: " + ex.Message);
                throw;
            }
        }
        private byte[] ComputeSignature(SignedCms cms)
        {
            return cms.Encode();
        }
        private byte[] HashBytes(byte[] input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(input);
            }
        }


        public void SignDocument(InvoiceModel model,string tokenPin)
        {
            SignatureModel signature = new SignatureModel();
            signature.SignatureType = "I";

            var serializedJson = SerializeToJson(model.ToDTO());

            var canonicalString = GenerateCanonicalString(serializedJson);

            IPkcs11Library? PkcsLibrary = LoadPkcsLibrary();

            if (PkcsLibrary == null)
            {
                _logger.LogError("SignatureService/SignDocument: PKCS_Library Load Failed");
                return;
            }

            ISlot slot = PkcsLibrary.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();
            if (slot == null)
            {
                _logger.LogError("SignatureService/GetFirstAvailableSlot: No slots found");
                return;
            }

            ISession session = slot.OpenSession(SessionType.ReadWrite);

            Unit loginResult = TokenLogin(session, tokenPin);

            IObjectHandle? certificate = FindCertificate(session);
            if (certificate == null)
                return;
            //TODO: Check Signature Certificate Issuer Name
            X509Certificate2? certificateForSigning = FindCertificateInStore("MCDR CA 2018");
            if (certificateForSigning == null)
                return;

            SignedCms signedCms = CreateSignedCms(Encoding.UTF8.GetBytes(canonicalString), certificate, certificateForSigning);

            byte[] encodedCms = ComputeSignature(signedCms);

            signature.Value = Convert.ToBase64String(encodedCms);

            model.Signatures.Add(signature);

            //var removableDrives = DriveInfo.GetDrives()
            //    .Where(d => d.DriveType == DriveType.Removable && d.IsReady);

            //foreach (var drive in removableDrives)
            //{
            //    Console.WriteLine($"Found removable drive: {drive.Name}");
            //    // Look for your certificate file
            //    //var certPath = Path.Combine(drive.RootDirectory.FullName, "yourCertificate.pfx");

            //    //if (File.Exists(certPath))
            //    //{
            //    //    Console.WriteLine($"Certificate found: {certPath}");

            //    //}
            //}
        }
    }
}
