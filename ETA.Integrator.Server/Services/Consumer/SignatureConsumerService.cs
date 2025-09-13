using ETA.Integrator.Server.Dtos;
using ETA.Integrator.Server.Interface.Services.Consumer;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;
using MediatR;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using ISession = Net.Pkcs11Interop.HighLevelAPI.ISession;

namespace ETA.Integrator.Server.Services.Consumer
{
    public class SignatureConsumerService : ISignatureConsumerService
    {
        private readonly ILogger<SignatureConsumerService> _logger;
        public SignatureConsumerService(ILogger<SignatureConsumerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void SerializeToken(JToken request, StringBuilder serialized)
        {
            {
                if (request is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "BAD_PARAMS",
                        detail: "Signature/SerializeToken: Serialize token failed (request)."
                        );

                if (request.Parent is null)
                {
                    if (request.First is null)
                        throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "BAD_PARAMS",
                        detail: "Signature/SerializeToken: Serialize token failed (parent)."
                        );

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
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true,
                        OverrideSpecifiedNames = true
                    }
                },
                Formatting = Formatting.Indented
            };

            var jsonString = JsonConvert.SerializeObject(dto , settings);

            return jsonString;
        }
        private string GenerateCanonicalString(string serializedJson)
        {
            if (string.IsNullOrWhiteSpace(serializedJson))
            {
                _logger.LogError("SignatureService/GenerateCanonicalString: Serialized JSON data is required.");
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/GenerateCanonicalString: BAD_PARAMS",
                    detail: "Serialized JSON data is required."
                );
            }

            var request = JsonConvert.DeserializeObject<JToken>(serializedJson, new JsonSerializerSettings
            {
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None
            });

            if (request is null)
            {
                _logger.LogError("SignatureService/GenerateCanonicalString: Request object is null");
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/GenerateCanonicalString: SERIALIZATION_FAILED",
                    detail: "Deserialized object has no value (request)."
                );
            }

            return CanonicalizeToken(request);
        }

        private string CanonicalizeToken(JToken request)
        {
           
            string serialized = "";
            if (request.Parent is null)
            {
                if (request.First == null){
                    return "";
                }
                CanonicalizeToken(request.First);
            }
            else
            {
                if (request.Type == JTokenType.Property)
                {
                    string name = ((JProperty)request).Name.ToUpper();
                    serialized += "\"" + name + "\"";
                    foreach (var property in request)
                    {
                        if (property.Type == JTokenType.Object)
                        {
                            serialized += CanonicalizeToken(property);
                        }
                        if (property.Type == JTokenType.Boolean || property.Type == JTokenType.Integer || property.Type == JTokenType.Float || property.Type == JTokenType.Date)
                        {
                            serialized += "\"" + property.Value<string>() + "\"";
                        }
                        if (property.Type == JTokenType.String)
                        {
                            serialized += JsonConvert.ToString(property.Value<string>());
                        }
                        if (property.Type == JTokenType.Array)
                        {
                            foreach (var item in property.Children())
                            {
                                serialized += "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                                serialized += CanonicalizeToken(item);
                            }
                        }
                    }
                }
                // Added to fix "References"
                if (request.Type == JTokenType.String)
                {
                    serialized += JsonConvert.ToString(request.Value<string>());
                }
            }
            if (request.Type == JTokenType.Object)
            {
                foreach (var property in request.Children())
                {

                    if (property.Type == JTokenType.Object || property.Type == JTokenType.Property)
                    {
                        serialized += CanonicalizeToken(property);
                    }
                }
            }

            return serialized;
        }

        private IPkcs11Library LoadPkcsLibrary()
        {
            try
            {
                var path = Environment.Is64BitProcess ?
                    @"C:\Windows\System32\eps2003csp11.dll" :
                    @"C:\Windows\SysWOW64\eps2003csp11.dll";

                Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
                IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, path, AppType.MultiThreaded);

                if (pkcs11Library is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SignatureService/LoadPkcsLibrary: PKCS_LIB_INTERNAL_ERR",
                        detail: "Library has no value."
                        );
                else
                    return pkcs11Library;
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/LoadPkcsLibrary: " + ex.Message);
                throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SignatureService/LoadPkcsLibrary: PKCS_LIB_LOADING_ERR",
                        detail: ex.Message
                        );
            }
        }
        private ISession OpenSession(IPkcs11Library pkcsLibrary)
        {
            try
            {
                ISlot? slot = pkcsLibrary.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();

                if (slot is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SignatureService/OpenSession: SLOTS_INTERNAL_ERR",
                        detail: $"No slots found. NULL"
                        );
                else
                {
                    ISession session = slot.OpenSession(SessionType.ReadWrite);

                    if (session is null)
                        throw new ProblemDetailsException(
                            statusCode: StatusCodes.Status500InternalServerError,
                            message: "SignatureService/OpenSession: SESSION_INTERNAL_ERR",
                            detail: "Session has no value."
                            );
                    else
                        return session;
                }
            }
            catch (Exception ex)
            {
                if (ex is ProblemDetailsException)
                    throw;

                _logger.LogError("SignatureService/OpenSession: " + ex.Message);
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/OpenSession: SLOTS_INTERNAL_ERR",
                    detail: ex.Message
                    );
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
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/TokenLogin: TOKEN_LOGIN_ERR",
                    detail: ex.Message
                    );
            }
        }
        private IObjectHandle FindCertificate(ISession session)
        {
            try
            {
                var certificateSearchAttributes = new List<IObjectAttribute>() {
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                };

                IObjectHandle? certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

                if (certificate is null)
                    throw new ProblemDetailsException(
                       statusCode: StatusCodes.Status500InternalServerError,
                       message: "SignatureService/FindCertificate: CERT_INTERNAL_ERR",
                       detail: "No certificate found."
                       );
                else
                    return certificate;
            }
            catch (Exception ex)
            {
                if (ex is ProblemDetailsException)
                    throw;

                _logger.LogError("SignatureService/FindCertificate: " + ex.Message);
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/FindCertificate: CERT_INTERNAL_ERR",
                    detail: ex.Message
                    );
            }
        }
        private X509Certificate2 FindCertificateInStore(string tokenCertificate)
        {
            try
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, tokenCertificate, false);
                store.Close();

                if (foundCerts.Count == 0 || foundCerts[0] is null)
                    throw new ProblemDetailsException(
                      statusCode: StatusCodes.Status500InternalServerError,
                      message: "SignatureService/FindCertificateInStore: STORE_CERT_INTERNAL_ERR",
                      detail: "No matching certificate found."
                      );
                else
                    return foundCerts[0];
            }
            catch (Exception ex)
            {
                if (ex is ProblemDetailsException)
                    throw;

                _logger.LogError("SignatureService/FindCertificateInStore: " + ex.Message);
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/FindCertificateInStore: STORE_CERT_INTERNAL_ERR",
                    detail: ex.Message
                    );
            }
        }
        private SignedCms CreateSignedCms(byte[] data, IObjectHandle certificateHandle, X509Certificate2 certificateForSigning)
        {
            try
            {
                ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);

                EssCertIDv2 bouncyCertificate = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), HashBytes(certificateForSigning.RawData));
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
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/CreateSignedCms: CMS_INTERNAL_ERR",
                    detail: ex.Message
                    );
            }
        }
        private byte[] ComputeSignature(SignedCms cms)
        {
            try
            {
                return cms.Encode();
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/ComputeSignature: " + ex.Message);
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/ComputeSignature: SIGNATURE_COMPUTE_ERR",
                    detail: ex.Message
                    );
            }
        }
        private byte[] HashBytes(byte[] input)
        {
            try
            {
                using (SHA256 sha = SHA256.Create())
                {
                    return sha.ComputeHash(input);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SignatureService/HashBytes: " + ex.Message);
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SignatureService/HashBytes: HASH_BYTES_ERR",
                    detail: ex.Message
                    );
            }
        }

        public string SignDocument(InvoiceModel model, string tokenPin)
        {
            model.Signatures = new List<SignatureModel>();
            SignatureModel signature = new SignatureModel();
            signature.SignatureType = "I";

            var serializedJson = SerializeToJson(model.FromInvoiceModel());
            //Debug.WriteLine("==== Serialized JSON ====");
            //Debug.WriteLine(serializedJson);
            //var canonicalString = GenerateCanonicalString(serializedJson);
            //Debug.WriteLine("==== Canonical String ====");
            //Debug.WriteLine(canonicalString);

            //IPkcs11Library pkcsLibrary = LoadPkcsLibrary();

            //ISession session = OpenSession(pkcsLibrary);

            //TokenLogin(session, tokenPin);

            //IObjectHandle certificate = FindCertificate(session);

            //X509Certificate2 certificateForSigning = FindCertificateInStore("Egypt Trust CA G6");

            //SignedCms signedCms = CreateSignedCms(Encoding.UTF8.GetBytes(canonicalString), certificate, certificateForSigning);

            //byte[] encodedCms = ComputeSignature(signedCms);

            //signature.Value = Convert.ToBase64String(encodedCms);

            //model.Signatures.Add(signature);
            var invoiceSigner = new InvoiceSignerService(tokenPin);
            var fullSignedDocument = invoiceSigner.SignDocument(serializedJson);
            return fullSignedDocument;
        }
    }
}
