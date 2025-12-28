using ETA.Integrator.Server.Dtos.SerializationDTOs;
using ETA.Integrator.Server.Interface.Services;
using ETA.Integrator.Server.Mapping;
using ETA.Integrator.Server.Models;
using ETA.Integrator.Server.Models.Consumer.ETA;
using ETA.Integrator.Server.Models.Core;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ETA.Integrator.Server.Services
{
    public class DocumentSignerService : IDocumentSignerService
    {
        private readonly DocumentMappingProcessor _documentMappingProcessor;
        public DocumentSignerService(DocumentMappingProcessor documentMappingProcessor) {
            _documentMappingProcessor = documentMappingProcessor;
        }

        
        public List<string> SignMultipleDocumentsMock(SigningPropertiesModel signingProperties)
        {
            List<string> documents = new List<string>();

            foreach (var model in signingProperties.Documents)
            {
                var (document, serializedDocument) = PrepareDocument(new DocumentMappIngPropertiesModel()
                {
                    Document = model,
                    Issuer = signingProperties.Issuer,
                    ItemCode = signingProperties.ItemCode,
                    InvoiceType = signingProperties.InvoiceType,
                    IsProduction = signingProperties.IsProduction,
                    ForNotes = signingProperties.ForNotes,
                    References = signingProperties.InternalIdsWithUUIDs.Where(i => i.Key == model.ReferenceId).Select(i => i.Value).ToList()
                });

                document.Signatures = new List<SignatureModel>();
                SignatureModel signature = new SignatureModel();
                signature.SignatureType = "I";

                var sourceDocumentJson = SerializedDocumentToJson(serializedDocument);

                string cades = "";

                JObject? request = JsonConvert.DeserializeObject<JObject>(sourceDocumentJson, new JsonSerializerSettings()
                {
                    FloatFormatHandling = FloatFormatHandling.String,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateParseHandling = DateParseHandling.None,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                        {
                            ProcessDictionaryKeys = true,
                            OverrideSpecifiedNames = true
                        }
                    }
                });

                if (request == null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SIGNING_ERR",
                        detail: "Request can not be null"
                        );

                //Start serialize
                string canonicalString = Canonicalize(request);
                var documentVersion = request["documentTypeVersion"];

                if (documentVersion == null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status500InternalServerError,
                        message: "SIGNING_ERR",
                        detail: "Document type version doesn't exist"
                        );

                // retrieve cades

                cades = "ANY";

                JObject signaturesObject = new JObject(
                                    new JProperty("signatureType", "I"),
                                    new JProperty("value", cades));

                JArray signaturesArray = [signaturesObject];
                request.Add("signatures", signaturesArray);


                var signedDocument = JsonConvert.SerializeObject(request);

                if (signedDocument == null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status404NotFound,
                        message: "SIGNING_ERR",
                        detail: "Invoice cannot be empty"
                        );

                documents.Add(signedDocument);
            }

            return documents;
        }
       
        public List<string> SignMultipleDocuments(SigningPropertiesModel signingProperties)
        {
            List<string> documents = new List<string>();

            var DllLibPath = @"C:\Windows\System32\eps2003csp11.dll";

            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();

            using (IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, DllLibPath, AppType.MultiThreaded))
            {
                ISlot? slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();

                if (slot is null)
                    throw new ProblemDetailsException(
                        statusCode: StatusCodes.Status404NotFound,
                        message: "SIGNING_ERR",
                        detail: "No slots found"
                        );

                ITokenInfo tokenInfo = slot.GetTokenInfo();

                ISlotInfo slotInfo = slot.GetSlotInfo();

                using (var session = slot.OpenSession(SessionType.ReadWrite))
                {
                    session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(signingProperties.TokenPin));

                    var certificateSearchAttributes = new List<IObjectAttribute>()
                {
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                    session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                };

                    IObjectHandle? certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

                    if (certificate is null)
                        throw new ProblemDetailsException(
                            statusCode: StatusCodes.Status404NotFound,
                            message: "SIGNING_ERR",
                            detail: "Certificate not found"
                            );

                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);

                    // find cert by thumbprint
                    var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, "Egypt Trust CA G6", false);

                    //var foundCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, "2b1cdda84ace68813284519b5fb540c2", true);

                    if (foundCerts.Count == 0)
                        throw new ProblemDetailsException(
                            statusCode: StatusCodes.Status404NotFound,
                            message: "SIGNING_ERR",
                            detail: "Device not found"
                            );

                    var certForSigning = foundCerts[0];
                    store.Close();

                    foreach (var model in signingProperties.Documents)
                    {
                        var (document, serializedDocument) = PrepareDocument(new DocumentMappIngPropertiesModel()
                        {
                            Document = model,
                            Issuer = signingProperties.Issuer,
                            ItemCode = signingProperties.ItemCode,
                            InvoiceType = signingProperties.InvoiceType,
                            IsProduction = signingProperties.IsProduction,
                            ForNotes = signingProperties.ForNotes,
                            References = signingProperties.InternalIdsWithUUIDs.Where(i => i.Key == model.ReferenceId).Select(i => i.Value).ToList()
                        });

                        document.Signatures = new List<SignatureModel>();
                        SignatureModel signature = new SignatureModel();
                        signature.SignatureType = "I";

                        var sourceDocumentJson = SerializedDocumentToJson(serializedDocument);

                        string cades = "";

                        JObject? request = JsonConvert.DeserializeObject<JObject>(sourceDocumentJson, new JsonSerializerSettings()
                        {
                            FloatFormatHandling = FloatFormatHandling.String,
                            FloatParseHandling = FloatParseHandling.Decimal,
                            DateFormatHandling = DateFormatHandling.IsoDateFormat,
                            DateParseHandling = DateParseHandling.None,
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                                {
                                    ProcessDictionaryKeys = true,
                                    OverrideSpecifiedNames = true
                                }
                            }
                        });

                        if (request == null)
                            throw new ProblemDetailsException(
                                statusCode: StatusCodes.Status500InternalServerError,
                                message: "SIGNING_ERR",
                                detail: "Request can not be null"
                                );

                        //Start serialize
                        string canonicalString = Canonicalize(request);
                        var documentVersion = request["documentTypeVersion"];

                        if (documentVersion == null)
                            throw new ProblemDetailsException(
                                statusCode: StatusCodes.Status500InternalServerError,
                                message: "SIGNING_ERR",
                                detail: "Document type version doesn't exist"
                                );

                        // retrieve cades
                        if (documentVersion.Value<string>() == "0.9")
                        {
                            cades = "ANY";
                        }
                        else
                        {
                            cades = SignWithCMS(canonicalString, certForSigning);
                        }

                        JObject signaturesObject = new JObject(
                                            new JProperty("signatureType", "I"),
                                            new JProperty("value", cades));

                        JArray signaturesArray = [signaturesObject];
                        request.Add("signatures", signaturesArray);


                        var signedDocument = JsonConvert.SerializeObject(request);

                        if (signedDocument == null)
                            throw new ProblemDetailsException(
                                statusCode: StatusCodes.Status404NotFound,
                                message: "SIGNING_ERR",
                                detail: "Invoice cannot be empty"
                                );

                        documents.Add(signedDocument);
                    }
                }
            }

            return documents;
        }

        private (InvoiceModel, InvoiceToSerializeDTO) PrepareDocument(DocumentMappIngPropertiesModel mappingProperties)
        {
            var document = _documentMappingProcessor.ProcessDocumentMap(mappingProperties);

            var serializedDocument = _documentMappingProcessor.ProcessSerializedVersionMap(mappingProperties.DocumentType, document, mappingProperties.References);

            return (document, serializedDocument);
        }

        private static string SignWithCMS(string serializedText, X509Certificate2 signingCert)
        {
            byte[] data = Encoding.UTF8.GetBytes(serializedText);

            EssCertIDv2 bouncyCertificate = new EssCertIDv2(
                new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")),
                HashBytes(signingCert.RawData)
            );

            SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[] { bouncyCertificate });
            CmsSigner signer = new CmsSigner(signingCert);

            signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");
            signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
            signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));

            ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
            SignedCms cms = new SignedCms(content, true);
            cms.ComputeSignature(signer);
            var output = cms.Encode();

            return Convert.ToBase64String(output);
        }
        
        private static byte[] HashBytes(byte[] input)
        {
            try
            {
                using (SHA256 sha = SHA256.Create())
                {
                    var output = sha.ComputeHash(input);
                    return output;
                }
            }
            catch (Exception ex)
            {
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SIGNING_ERR",
                    detail: ex.Message
                    );
            }
        }
        
        private static string SerializedDocumentToJson(InvoiceToSerializeDTO dto)
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

            var jsonString = JsonConvert.SerializeObject(dto, settings);

            return jsonString;
        }
       
        private static string Canonicalize(JToken request)
        {
            string serialized = "";

            if (request is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SIGNING_ERR",
                    detail: "Serialize token failed (request)."
                    );

            if (request.Parent is null)
            {
                if (request.First is null)
                    throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SIGNING_ERR",
                    detail: "Serialize token failed (parent)."
                    );

                Canonicalize(request.First);
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
                            serialized += Canonicalize(property);
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
                                serialized += Canonicalize(item);
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
                        serialized += Canonicalize(property);
                    }
                }
            }

            return serialized;
        }
    }
}
